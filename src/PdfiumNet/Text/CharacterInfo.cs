using PdfiumNet.Geometry;

namespace PdfiumNet.Text;

/// <summary>
/// Detailed information about a single character on a PDF page.
/// </summary>
public readonly record struct CharacterInfo
{
    /// <summary>The character index on the text page.</summary>
    public int Index { get; init; }

    /// <summary>The Unicode character.</summary>
    public char Character { get; init; }

    /// <summary>The bounding box of the character in page coordinates.</summary>
    public PdfRectangle BoundingBox { get; init; }

    /// <summary>The origin point of the character.</summary>
    public PdfPoint Origin { get; init; }

    /// <summary>The font size in points.</summary>
    public double FontSize { get; init; }

    /// <summary>The font name, if available.</summary>
    public string? FontName { get; init; }
}
