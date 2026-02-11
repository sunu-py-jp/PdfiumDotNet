using PdfiumNet.Annotations;
using PdfiumNet.Drawing;
using PdfiumNet.Exceptions;
using PdfiumNet.Geometry;
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
    public PdfBitmap Render(int dpi = 72, int flags = 0)
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
            0, flags | PdfiumNative.FPDF_ANNOT | PdfiumNative.FPDF_LCD_TEXT);

        return bitmap;
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
