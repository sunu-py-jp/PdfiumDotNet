using System.Runtime.InteropServices;
using PdfiumNet.Attachments;
using PdfiumNet.Bookmarks;
using PdfiumNet.Exceptions;
using PdfiumNet.Forms;
using PdfiumNet.Geometry;
using PdfiumNet.IO;
using PdfiumNet.Native;
using PdfiumNet.Signatures;

namespace PdfiumNet;

/// <summary>
/// Represents a PDF document. Use factory methods to create or open documents.
/// </summary>
public sealed class PdfDocument : IDisposable
{
    private IntPtr _handle;
    private readonly PdfPageCollection _pages;
    private PdfMetadata? _metadata;
    private PdfBookmarkCollection? _bookmarks;
    private PdfAttachmentCollection? _attachments;
    private PdfSignatureCollection? _signatures;
    private byte[]? _loadedData; // Keep reference to prevent GC during document lifetime
    private bool _disposed;

    private PdfDocument(IntPtr handle)
    {
        _handle = handle;
        _pages = new PdfPageCollection(this);
    }

    internal IntPtr Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _handle;
        }
    }

    /// <summary>
    /// Gets the collection of pages in this document.
    /// </summary>
    public PdfPageCollection Pages => _pages;

    /// <summary>
    /// Gets the document metadata (Title, Author, Subject, etc.).
    /// </summary>
    public PdfMetadata Metadata => _metadata ??= new PdfMetadata(this);

    /// <summary>
    /// Gets the collection of bookmarks (outline) in this document.
    /// </summary>
    public PdfBookmarkCollection Bookmarks => _bookmarks ??= new PdfBookmarkCollection(this);

    /// <summary>
    /// Gets the collection of file attachments in this document.
    /// </summary>
    public PdfAttachmentCollection Attachments => _attachments ??= new PdfAttachmentCollection(this);

    /// <summary>
    /// Gets the collection of digital signatures in this document (read-only).
    /// </summary>
    public PdfSignatureCollection Signatures => _signatures ??= new PdfSignatureCollection(this);

    /// <summary>
    /// Gets the form type of this document.
    /// </summary>
    public PdfFormType FormType => (PdfFormType)PdfiumNative.FPDF_GetFormType(Handle);

    /// <summary>
    /// Gets whether this document contains forms.
    /// </summary>
    public bool HasForms => FormType != PdfFormType.None;

    /// <summary>
    /// Creates a form info object for reading form fields.
    /// The caller is responsible for disposing the returned object.
    /// </summary>
    public PdfFormInfo GetFormInfo() => new PdfFormInfo(this);

    /// <summary>
    /// Gets the number of pages in the document.
    /// </summary>
    public int PageCount => PdfiumNative.FPDF_GetPageCount(Handle);

    /// <summary>
    /// Gets the PDF file version (e.g. 17 for PDF 1.7).
    /// </summary>
    public int FileVersion
    {
        get
        {
            PdfiumNative.FPDF_GetFileVersion(Handle, out var version);
            return version;
        }
    }

    /// <summary>
    /// Creates a new empty PDF document.
    /// </summary>
    public static PdfDocument Create()
    {
        PdfiumLibrary.EnsureInitialized();
        var handle = PdfiumNative.FPDF_CreateNewDocument();
        if (handle == IntPtr.Zero)
            throw new PdfiumException("Failed to create new PDF document.");
        return new PdfDocument(handle);
    }

    /// <summary>
    /// Opens a PDF document from a file path.
    /// </summary>
    public static PdfDocument Open(string path, string? password = null)
    {
        PdfiumLibrary.EnsureInitialized();
        var handle = PdfiumNative.FPDF_LoadDocument(path, password);
        if (handle == IntPtr.Zero)
            throw PdfiumException.FromLastError();
        return new PdfDocument(handle);
    }

    /// <summary>
    /// Opens a PDF document from a byte span.
    /// </summary>
    public static PdfDocument Open(ReadOnlySpan<byte> data, string? password = null)
    {
        PdfiumLibrary.EnsureInitialized();
        var buffer = data.ToArray();
        var pinnedData = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            var handle = PdfiumNative.FPDF_LoadMemDocument(
                pinnedData.AddrOfPinnedObject(), buffer.Length, password);
            if (handle == IntPtr.Zero)
            {
                pinnedData.Free();
                throw PdfiumException.FromLastError();
            }
            var doc = new PdfDocument(handle) { _loadedData = buffer };
            return doc;
        }
        catch
        {
            if (pinnedData.IsAllocated) pinnedData.Free();
            throw;
        }
    }

    /// <summary>
    /// Opens a PDF document from a stream.
    /// </summary>
    public static PdfDocument Open(Stream stream, string? password = null)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Open(ms.ToArray(), password);
    }

    /// <summary>
    /// Adds a new page at the specified index with the given size.
    /// </summary>
    public PdfPage AddPage(int index, PdfSize size)
    {
        var pageHandle = PdfiumNative.FPDFPage_New(Handle, index, size.Width, size.Height);
        if (pageHandle == IntPtr.Zero)
            throw new PdfiumException("Failed to create new page.");
        return new PdfPage(this, pageHandle, index);
    }

    /// <summary>
    /// Adds a new page at the end of the document.
    /// </summary>
    public PdfPage AddPage(PdfSize size) => AddPage(PageCount, size);

    /// <summary>
    /// Removes a page at the specified index.
    /// </summary>
    public void RemovePage(int index)
    {
        if (index < 0 || index >= PageCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        PdfiumNative.FPDFPage_Delete(Handle, index);
        _pages.InvalidateCache();
    }

    /// <summary>
    /// Imports pages from another document.
    /// </summary>
    public bool ImportPages(PdfDocument source, string? pageRange = null, int insertIndex = -1)
    {
        if (insertIndex < 0) insertIndex = PageCount;
        var result = PdfiumNative.FPDF_ImportPages(Handle, source.Handle, pageRange, insertIndex);
        if (result)
            _pages.InvalidateCache();
        return result;
    }

    /// <summary>
    /// Merges all pages from another document into this document at the specified position.
    /// </summary>
    /// <param name="source">The source document to merge pages from.</param>
    /// <param name="insertIndex">The position to insert the pages. Defaults to the end of the document.</param>
    public void MergeFrom(PdfDocument source, int insertIndex = -1)
    {
        if (!ImportPages(source, null, insertIndex))
            throw new PdfiumException("Failed to merge pages from source document.");
    }

    /// <summary>
    /// Extracts specified pages into a new document.
    /// </summary>
    /// <param name="pageIndices">Zero-based indices of pages to extract.</param>
    /// <returns>A new document containing the extracted pages. The caller must dispose it.</returns>
    public PdfDocument ExtractPages(params int[] pageIndices)
    {
        if (pageIndices.Length == 0)
            throw new ArgumentException("At least one page index must be specified.", nameof(pageIndices));

        // PDFium ImportPages uses 1-based page numbers in the range string
        var pageRange = string.Join(",", pageIndices.Select(i => (i + 1).ToString()));

        var newDoc = Create();
        try
        {
            if (!PdfiumNative.FPDF_ImportPages(newDoc.Handle, Handle, pageRange, 0))
            {
                newDoc.Dispose();
                throw new PdfiumException("Failed to extract pages.");
            }
            return newDoc;
        }
        catch
        {
            newDoc.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Gets all JavaScript actions in the document.
    /// Useful for security auditing to detect potentially malicious scripts.
    /// </summary>
    public IReadOnlyList<PdfJavaScriptAction> GetJavaScriptActions()
    {
        var count = PdfiumNative.FPDFDoc_GetJavaScriptActionCount(Handle);
        if (count <= 0)
            return Array.Empty<PdfJavaScriptAction>();

        var actions = new List<PdfJavaScriptAction>(count);
        for (var i = 0; i < count; i++)
        {
            var jsHandle = PdfiumNative.FPDFDoc_GetJavaScriptAction(Handle, i);
            if (jsHandle == IntPtr.Zero) continue;

            try
            {
                var name = NativeStringHelper.ReadUtf16((buf, len) =>
                    PdfiumNative.FPDFJavaScriptAction_GetName(jsHandle, buf, len));
                var script = NativeStringHelper.ReadUtf16((buf, len) =>
                    PdfiumNative.FPDFJavaScriptAction_GetScript(jsHandle, buf, len));

                actions.Add(new PdfJavaScriptAction(name, script));
            }
            finally
            {
                PdfiumNative.FPDFDoc_CloseJavaScriptAction(jsHandle);
            }
        }

        return actions;
    }

    /// <summary>
    /// Saves the document to a file.
    /// </summary>
    public void Save(string path, PdfSaveOptions? options = null)
    {
        using var stream = File.Create(path);
        Save(stream, options);
    }

    /// <summary>
    /// Saves the document to a stream.
    /// </summary>
    public void Save(Stream stream, PdfSaveOptions? options = null)
    {
        options ??= PdfSaveOptions.Default;
        using var fileWrite = new ManagedFileWrite(stream);

        bool success;
        if (options.FileVersion > 0)
        {
            success = PdfiumNative.FPDF_SaveWithVersion(
                Handle, ref fileWrite.NativeStruct, (uint)options.Flags, options.FileVersion);
        }
        else
        {
            success = PdfiumNative.FPDF_SaveAsCopy(
                Handle, ref fileWrite.NativeStruct, (uint)options.Flags);
        }

        if (!success)
            throw new PdfiumException("Failed to save PDF document.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _pages.Dispose();
        if (_handle != IntPtr.Zero)
        {
            PdfiumNative.FPDF_CloseDocument(_handle);
            _handle = IntPtr.Zero;
        }
        _loadedData = null;
    }
}
