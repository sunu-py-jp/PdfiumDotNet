using PdfiumNet.Native;

namespace PdfiumNet.StructTree;

/// <summary>
/// Represents a structure element in a PDF structure tree.
/// Structure elements define the logical structure (e.g., paragraphs, headings, tables).
/// </summary>
public sealed class PdfStructElement
{
    private readonly IntPtr _handle;

    internal PdfStructElement(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Gets the type of the structure element (e.g., "P", "H1", "Table", "Span").
    /// </summary>
    public string Type => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDF_StructElement_GetType(_handle, buf, len));

    /// <summary>
    /// Gets the title of the structure element, if set.
    /// </summary>
    public string Title => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDF_StructElement_GetTitle(_handle, buf, len));

    /// <summary>
    /// Gets the alternative text (Alt) of the structure element.
    /// Used for accessibility (e.g., alt text for images).
    /// </summary>
    public string AltText => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDF_StructElement_GetAltText(_handle, buf, len));

    /// <summary>
    /// Gets the actual text of the structure element.
    /// </summary>
    public string ActualText => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDF_StructElement_GetActualText(_handle, buf, len));

    /// <summary>
    /// Gets the language of the structure element.
    /// </summary>
    public string Language => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDF_StructElement_GetLang(_handle, buf, len));

    /// <summary>
    /// Gets the number of children of this element.
    /// </summary>
    public int ChildCount => PdfiumNative.FPDF_StructElement_CountChildren(_handle);

    /// <summary>
    /// Gets a child element at the specified index.
    /// </summary>
    public PdfStructElement? GetChild(int index)
    {
        if (index < 0 || index >= ChildCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        var handle = PdfiumNative.FPDF_StructElement_GetChildAtIndex(_handle, index);
        return handle == IntPtr.Zero ? null : new PdfStructElement(handle);
    }

    /// <summary>
    /// Gets all children of this element.
    /// </summary>
    public IReadOnlyList<PdfStructElement> GetChildren()
    {
        var count = ChildCount;
        var children = new List<PdfStructElement>(count);
        for (var i = 0; i < count; i++)
        {
            var child = GetChild(i);
            if (child != null)
                children.Add(child);
        }
        return children;
    }
}
