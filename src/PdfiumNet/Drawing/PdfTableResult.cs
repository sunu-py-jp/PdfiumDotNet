using System.Collections.Generic;

namespace PdfiumNet.Drawing;

/// <summary>
/// Result of rendering a <see cref="PdfTable"/> onto a PDF canvas.
/// </summary>
public sealed class PdfTableResult
{
    internal PdfTableResult(IReadOnlyList<PdfPage> pages, float bottomY)
    {
        Pages = pages;
        BottomY = bottomY;
    }

    /// <summary>
    /// All pages used by the rendered table (including pages created for overflow).
    /// </summary>
    public IReadOnlyList<PdfPage> Pages { get; }

    /// <summary>
    /// The Y coordinate of the bottom edge of the last row on the last page.
    /// </summary>
    public float BottomY { get; }
}
