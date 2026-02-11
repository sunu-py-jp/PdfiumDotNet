using PdfiumNet.Exceptions;
using PdfiumNet.Native;
using PdfiumNet.Text;

namespace PdfiumNet.Objects;

/// <summary>
/// Represents a text object on a PDF page.
/// </summary>
public sealed class PdfTextObject : PdfPageObject
{
    internal PdfTextObject(IntPtr handle) : base(handle) { }

    /// <summary>
    /// Creates a new text object using a standard PDF font.
    /// </summary>
    public static PdfTextObject Create(PdfDocument document, string fontName, float fontSize, string text)
    {
        var handle = PdfiumNative.FPDFPageObj_NewTextObj(document.Handle, fontName, fontSize);
        if (handle == IntPtr.Zero)
            throw new PdfiumException($"Failed to create text object with font '{fontName}'.");

        PdfiumNative.FPDFText_SetText(handle, text);
        return new PdfTextObject(handle);
    }

    /// <summary>
    /// Creates a new text object using a loaded font.
    /// </summary>
    public static PdfTextObject Create(PdfDocument document, PdfFont font, float fontSize, string text)
    {
        var handle = PdfiumNative.FPDFPageObj_CreateTextObj(document.Handle, font.Handle, fontSize);
        if (handle == IntPtr.Zero)
            throw new PdfiumException("Failed to create text object with custom font.");

        PdfiumNative.FPDFText_SetText(handle, text);
        return new PdfTextObject(handle);
    }

    /// <summary>
    /// Gets the text content of this text object.
    /// </summary>
    /// <param name="textPage">A text page obtained from <see cref="PdfPage.GetTextPage"/>.</param>
    /// <returns>The text content, or an empty string if no text is available.</returns>
    public string GetText(PdfTextPage textPage)
    {
        return NativeStringHelper.ReadUtf16((buf, len) =>
            PdfiumNative.FPDFTextObj_GetText(Handle, textPage.Handle, buf, len));
    }

    /// <summary>
    /// Sets the text content.
    /// </summary>
    public void SetText(string text)
    {
        PdfiumNative.FPDFText_SetText(Handle, text);
    }

    /// <summary>
    /// Gets the font size.
    /// </summary>
    public float FontSize
    {
        get
        {
            PdfiumNative.FPDFTextObj_GetFontSize(Handle, out var size);
            return size;
        }
    }
}
