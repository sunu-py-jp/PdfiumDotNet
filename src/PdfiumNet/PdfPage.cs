using System.Runtime.InteropServices;
using PdfiumNet.Annotations;
using PdfiumNet.Drawing;
using PdfiumNet.Exceptions;
using PdfiumNet.Geometry;
using PdfiumNet.Links;
using PdfiumNet.Native;
using PdfiumNet.Objects;
using PdfiumNet.Text;

namespace PdfiumNet;

/// <summary>
/// Represents a single page in a PDF document.
/// </summary>
public sealed class PdfPage : IDisposable
{
    private readonly PdfDocument _document;
    private IntPtr _handle;
    private PdfTextPage? _textPage;
    private PdfPageObjectCollection? _objects;
    private PdfAnnotationCollection? _annotations;
    private bool _disposed;

    internal PdfPage(PdfDocument document, IntPtr handle, int index)
    {
        _document = document;
        _handle = handle;
        Index = index;
    }

    internal IntPtr Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _handle;
        }
    }

    internal PdfDocument Document => _document;

    /// <summary>
    /// Gets the zero-based index of this page.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the label for this page (e.g. "i", "ii", "1", "2").
    /// </summary>
    public string Label => _document.GetPageLabel(Index);

    /// <summary>
    /// Gets the width of the page in points.
    /// </summary>
    public float Width => PdfiumNative.FPDF_GetPageWidthF(Handle);

    /// <summary>
    /// Gets the height of the page in points.
    /// </summary>
    public float Height => PdfiumNative.FPDF_GetPageHeightF(Handle);

    /// <summary>
    /// Gets the size of the page.
    /// </summary>
    public PdfSize Size => new(Width, Height);

    /// <summary>
    /// Gets or sets the page rotation (0, 90, 180, or 270 degrees).
    /// </summary>
    public int Rotation
    {
        get => PdfiumNative.FPDFPage_GetRotation(Handle) * 90;
        set
        {
            var rotateValue = (value / 90) % 4;
            if (rotateValue < 0) rotateValue += 4;
            PdfiumNative.FPDFPage_SetRotation(Handle, rotateValue);
        }
    }

    /// <summary>
    /// Gets the collection of page objects.
    /// </summary>
    public PdfPageObjectCollection Objects => _objects ??= new PdfPageObjectCollection(this);

    /// <summary>
    /// Gets the collection of annotations on this page.
    /// </summary>
    public PdfAnnotationCollection Annotations => _annotations ??= new PdfAnnotationCollection(this);

    /// <summary>
    /// Gets all links on this page.
    /// </summary>
    public IReadOnlyList<PdfLink> GetLinks()
    {
        var links = new List<PdfLink>();
        var startPos = 0;
        while (PdfiumNative.FPDFLink_Enumerate(Handle, ref startPos, out var linkAnnot))
        {
            if (linkAnnot == IntPtr.Zero) continue;

            PdfRectangle rect = default;
            if (PdfiumNative.FPDFLink_GetAnnotRect(linkAnnot, out var left, out var top, out var right, out var bottom))
                rect = new PdfRectangle(left, bottom, right, top);

            links.Add(new PdfLink(_document, linkAnnot, rect));
        }
        return links;
    }

    /// <summary>
    /// Extracts all text from the page.
    /// </summary>
    public string ExtractText()
    {
        using var textPage = new PdfTextPage(this);
        return textPage.GetText();
    }

    /// <summary>
    /// Gets a text page for detailed text operations.
    /// The caller is responsible for disposing the returned object.
    /// </summary>
    public PdfTextPage GetTextPage() => new(this);

    /// <summary>
    /// Gets a canvas for drawing on this page.
    /// </summary>
    public PdfCanvas GetCanvas() => new(this);

    /// <summary>
    /// Inserts a page object into this page.
    /// After insertion, the page object is owned by PDFium.
    /// </summary>
    public void InsertObject(PdfPageObject pageObject)
    {
        PdfiumNative.FPDFPage_InsertObject(Handle, pageObject.Handle);
        pageObject.MarkInserted();
    }

    /// <summary>
    /// Removes a page object from this page.
    /// After removal, the caller is responsible for disposing the object.
    /// </summary>
    public bool RemoveObject(PdfPageObject pageObject)
    {
        var result = PdfiumNative.FPDFPage_RemoveObject(Handle, pageObject.Handle);
        if (result)
            pageObject.MarkRemoved();
        return result;
    }

    /// <summary>
    /// Replaces occurrences of <paramref name="oldText"/> with <paramref name="newText"/>
    /// within each text object on this page.
    /// </summary>
    /// <remarks>
    /// PDF text may be split across multiple text objects (e.g. "Hello" and "World" as separate objects).
    /// This method only replaces within individual text objects and does not match across object boundaries.
    /// Call <see cref="GenerateContent"/> after replacing to persist the changes.
    /// </remarks>
    /// <param name="oldText">The text to search for.</param>
    /// <param name="newText">The replacement text.</param>
    /// <param name="comparison">The string comparison type to use.</param>
    /// <returns>The number of text objects that were modified.</returns>
    public int ReplaceText(string oldText, string newText, StringComparison comparison = StringComparison.Ordinal)
    {
        var count = 0;
        using var textPage = GetTextPage();
        foreach (var obj in Objects)
        {
            if (obj is PdfTextObject textObj)
            {
                var currentText = textObj.GetText(textPage);
                if (string.IsNullOrEmpty(currentText)) continue;

                var replaced = currentText.Replace(oldText, newText, comparison);
                if (!string.Equals(currentText, replaced, StringComparison.Ordinal))
                {
                    textObj.SetText(replaced);
                    count++;
                }
            }
        }
        return count;
    }

    /// <summary>
    /// Gets or sets the MediaBox rectangle for this page.
    /// The MediaBox defines the boundaries of the physical medium on which the page is displayed or printed.
    /// </summary>
    public PdfRectangle MediaBox
    {
        get
        {
            if (PdfiumNative.FPDFPage_GetMediaBox(Handle, out var left, out var bottom, out var right, out var top))
                return new PdfRectangle(left, bottom, right, top);
            return new PdfRectangle(0, 0, Width, Height);
        }
        set => PdfiumNative.FPDFPage_SetMediaBox(Handle, value.Left, value.Bottom, value.Right, value.Top);
    }

    /// <summary>
    /// Gets or sets the CropBox rectangle for this page.
    /// The CropBox defines the region to which the page contents are clipped when displayed or printed.
    /// Returns null if no CropBox is explicitly set.
    /// </summary>
    public PdfRectangle? CropBox
    {
        get
        {
            if (PdfiumNative.FPDFPage_GetCropBox(Handle, out var left, out var bottom, out var right, out var top))
                return new PdfRectangle(left, bottom, right, top);
            return null;
        }
        set
        {
            if (value is { } rect)
                PdfiumNative.FPDFPage_SetCropBox(Handle, rect.Left, rect.Bottom, rect.Right, rect.Top);
        }
    }

    /// <summary>
    /// Flattens annotations and form fields into the page content.
    /// After flattening, annotations become part of the page content and are no longer interactive.
    /// </summary>
    /// <param name="forPrint">If true, flatten for print; otherwise for normal display.</param>
    /// <returns>True if the page was flattened or there was nothing to flatten; false on failure.</returns>
    public bool Flatten(bool forPrint = false)
    {
        var result = PdfiumNative.FPDFPage_Flatten(Handle,
            forPrint ? PdfiumNative.FLAT_PRINT : PdfiumNative.FLAT_NORMALDISPLAY);
        return result == PdfiumNative.FLATTEN_SUCCESS || result == PdfiumNative.FLATTEN_NOTHINGTODO;
    }

    /// <summary>
    /// Gets the decoded thumbnail image data for this page, if available.
    /// </summary>
    /// <returns>The thumbnail image data as a byte array, or null if no thumbnail exists.</returns>
    public byte[]? GetThumbnail()
    {
        var size = PdfiumNative.FPDFPage_GetDecodedThumbnailData(Handle, IntPtr.Zero, 0);
        if (size == 0)
            return null;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            PdfiumNative.FPDFPage_GetDecodedThumbnailData(Handle, buffer, size);
            var result = new byte[size];
            Marshal.Copy(buffer, result, 0, (int)size);
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Generates the page content stream. Must be called after modifying page objects
    /// and before saving the document.
    /// </summary>
    public bool GenerateContent()
    {
        return PdfiumNative.FPDFPage_GenerateContent(Handle);
    }

    /// <summary>
    /// Renders the page to a bitmap at the specified DPI.
    /// </summary>
    public PdfBitmap Render(int dpi = 72, RenderFlags flags = RenderFlags.Annotations | RenderFlags.LcdText)
    {
        var scaleX = dpi / 72.0f;
        var scaleY = dpi / 72.0f;
        var pixelWidth = (int)(Width * scaleX);
        var pixelHeight = (int)(Height * scaleY);

        var bitmap = PdfBitmap.Create(pixelWidth, pixelHeight, true);
        bitmap.FillRect(0, 0, pixelWidth, pixelHeight, 0xFFFFFFFF); // White background

        PdfiumNative.FPDF_RenderPageBitmap(
            bitmap.Handle, Handle,
            0, 0, pixelWidth, pixelHeight,
            0, (int)flags);

        return bitmap;
    }

    /// <summary>
    /// Renders a rectangular region of the page to a bitmap at the specified DPI.
    /// The region is specified in PDF coordinate space (points, origin at bottom-left).
    /// </summary>
    /// <param name="region">The rectangular region to render, in PDF points.</param>
    /// <param name="dpi">The resolution in dots per inch.</param>
    /// <param name="flags">Rendering flags.</param>
    public PdfBitmap RenderRegion(PdfRectangle region, int dpi = 72, RenderFlags flags = RenderFlags.Annotations | RenderFlags.LcdText)
    {
        var scale = dpi / 72.0f;

        // Full page size in pixels
        var pagePixelWidth = (int)(Width * scale);
        var pagePixelHeight = (int)(Height * scale);

        // Region size in pixels
        var regionPixelWidth = (int)(region.Width * scale);
        var regionPixelHeight = (int)(region.Height * scale);

        if (regionPixelWidth <= 0 || regionPixelHeight <= 0)
            throw new ArgumentException("Region must have positive width and height.", nameof(region));

        // Convert PDF coordinates (origin bottom-left) to bitmap coordinates (origin top-left)
        var pixelLeft = (int)(region.Left * scale);
        var pixelTop = (int)((Height - region.Top) * scale);

        var bitmap = PdfBitmap.Create(regionPixelWidth, regionPixelHeight, true);
        bitmap.FillRect(0, 0, regionPixelWidth, regionPixelHeight, 0xFFFFFFFF);

        // Offset the page rendering so that the desired region maps to (0,0) on the bitmap
        PdfiumNative.FPDF_RenderPageBitmap(
            bitmap.Handle, Handle,
            -pixelLeft, -pixelTop, pagePixelWidth, pagePixelHeight,
            0, (int)flags);

        return bitmap;
    }

    /// <summary>
    /// Renders a rectangular region of the page to a PNG byte array.
    /// </summary>
    public byte[] RenderRegionToPng(PdfRectangle region, int dpi = 72, RenderFlags flags = RenderFlags.Annotations | RenderFlags.LcdText)
    {
        using var bitmap = RenderRegion(region, dpi, flags);
        return bitmap.ToPng();
    }

    /// <summary>
    /// Renders a rectangular region of the page to a BMP byte array.
    /// </summary>
    public byte[] RenderRegionToBmp(PdfRectangle region, int dpi = 72, RenderFlags flags = RenderFlags.Annotations | RenderFlags.LcdText)
    {
        using var bitmap = RenderRegion(region, dpi, flags);
        return bitmap.ToBmp();
    }

    /// <summary>
    /// Renders the page to a PNG byte array at the specified DPI.
    /// </summary>
    public byte[] RenderToPng(int dpi = 72, RenderFlags flags = RenderFlags.Annotations | RenderFlags.LcdText)
    {
        using var bitmap = Render(dpi, flags);
        return bitmap.ToPng();
    }

    /// <summary>
    /// Renders the page to a BMP byte array at the specified DPI.
    /// </summary>
    public byte[] RenderToBmp(int dpi = 72, RenderFlags flags = RenderFlags.Annotations | RenderFlags.LcdText)
    {
        using var bitmap = Render(dpi, flags);
        return bitmap.ToBmp();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _textPage?.Dispose();
        _textPage = null;
        if (_handle != IntPtr.Zero)
        {
            PdfiumNative.FPDF_ClosePage(_handle);
            _handle = IntPtr.Zero;
        }
    }
}
