using PdfiumNet.Geometry;

namespace PdfiumNet.Text;

/// <summary>
/// Represents a text search result on a PDF page.
/// </summary>
public readonly record struct TextSearchResult
{
    /// <summary>The zero-based character index where the match starts.</summary>
    public int StartIndex { get; init; }

    /// <summary>The number of characters in the match.</summary>
    public int Length { get; init; }

    /// <summary>The bounding rectangles of the matched text. Empty unless obtained via SearchWithBounds.</summary>
    public IReadOnlyList<PdfRectangle> Rectangles { get; init; }

    /// <summary>
    /// Gets the rectangles, returning an empty array if null.
    /// </summary>
    internal IReadOnlyList<PdfRectangle> GetRectangles() => Rectangles ?? Array.Empty<PdfRectangle>();
}
