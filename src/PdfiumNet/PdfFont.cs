using System.Runtime.InteropServices;
using PdfiumNet.Exceptions;
using PdfiumNet.Native;

namespace PdfiumNet;

/// <summary>
/// Represents a font loaded into a PDF document.
/// Fonts are owned by the document and disposed when the document is closed.
/// </summary>
public sealed class PdfFont : IDisposable
{
    private IntPtr _handle;
    private readonly bool _ownsHandle;
    private bool _disposed;

    private PdfFont(IntPtr handle, bool ownsHandle)
    {
        _handle = handle;
        _ownsHandle = ownsHandle;
    }

    internal IntPtr Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _handle;
        }
    }

    /// <summary>Font type constants for LoadFont.</summary>
    public const int FontTypeTrueType = 2;
    public const int FontTypeType1 = 1;

    /// <summary>
    /// Loads a standard PDF font (e.g. "Helvetica", "Times-Roman", "Courier").
    /// </summary>
    public static PdfFont LoadStandard(PdfDocument document, string fontName)
    {
        var handle = PdfiumNative.FPDFText_LoadStandardFont(document.Handle, fontName);
        if (handle == IntPtr.Zero)
            throw new PdfiumException($"Failed to load standard font '{fontName}'.");
        return new PdfFont(handle, true);
    }

    /// <summary>
    /// Loads a font from font file data (TrueType or Type1).
    /// </summary>
    public static PdfFont LoadFont(PdfDocument document, ReadOnlySpan<byte> fontData,
        int fontType = FontTypeTrueType, bool cid = true)
    {
        var pinnedData = GCHandle.Alloc(fontData.ToArray(), GCHandleType.Pinned);
        try
        {
            var handle = PdfiumNative.FPDFText_LoadFont(
                document.Handle, pinnedData.AddrOfPinnedObject(),
                (uint)fontData.Length, fontType, cid);
            if (handle == IntPtr.Zero)
                throw new PdfiumException("Failed to load font.");
            return new PdfFont(handle, true);
        }
        finally
        {
            pinnedData.Free();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_ownsHandle && _handle != IntPtr.Zero)
        {
            PdfiumNative.FPDFFont_Close(_handle);
            _handle = IntPtr.Zero;
        }
    }
}
