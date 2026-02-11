namespace PdfiumNet.Drawing;

/// <summary>
/// Defines the visual style for a <see cref="PdfTable"/>.
/// </summary>
public sealed class PdfTableStyle
{
    /// <summary>Whether the first row is treated as a header. Defaults to true.</summary>
    public bool HasHeader { get; set; } = true;

    /// <summary>Data row height in points. Defaults to 22.</summary>
    public float RowHeight { get; set; } = 22;

    /// <summary>Header row height in points. Defaults to 24.</summary>
    public float HeaderRowHeight { get; set; } = 24;

    /// <summary>Horizontal cell padding in points. Defaults to 5.</summary>
    public float PaddingX { get; set; } = 5;

    /// <summary>Vertical cell padding in points. Defaults to 6.</summary>
    public float PaddingY { get; set; } = 6;

    /// <summary>Font name for data rows. Defaults to "Helvetica".</summary>
    public string FontName { get; set; } = "Helvetica";

    /// <summary>Font size for data rows. Defaults to 9.</summary>
    public float FontSize { get; set; } = 9;

    /// <summary>Font name for the header row. Defaults to "Helvetica-Bold".</summary>
    public string HeaderFontName { get; set; } = "Helvetica-Bold";

    /// <summary>Font size for the header row. Defaults to 10.</summary>
    public float HeaderFontSize { get; set; } = 10;

    /// <summary>Background color for the header row. Defaults to light gray (220,220,220).</summary>
    public PdfColor HeaderBackgroundColor { get; set; } = new(220, 220, 220);

    /// <summary>Text color for the header row. Defaults to Black.</summary>
    public PdfColor HeaderTextColor { get; set; } = PdfColor.Black;

    /// <summary>Text color for data rows. Defaults to Black.</summary>
    public PdfColor TextColor { get; set; } = PdfColor.Black;

    /// <summary>Background color for alternating (even) data rows. Defaults to Transparent (no zebra striping).</summary>
    public PdfColor AlternateRowColor { get; set; } = PdfColor.Transparent;

    /// <summary>Color for grid lines. Defaults to Black.</summary>
    public PdfColor BorderColor { get; set; } = PdfColor.Black;

    /// <summary>Line width for outer borders and the line below the header. Defaults to 1.0.</summary>
    public float OuterBorderWidth { get; set; } = 1.0f;

    /// <summary>Line width for inner grid lines. Defaults to 0.5.</summary>
    public float InnerBorderWidth { get; set; } = 0.5f;

    /// <summary>Fixed column widths in points. When null, columns are distributed equally.</summary>
    public float[]? ColumnWidths { get; set; }

    /// <summary>Per-column text alignment. When null, all columns default to Left.</summary>
    public PdfTableColumnAlignment[]? ColumnAlignments { get; set; }

    /// <summary>Whether to redraw the header row when the table spans to a new page. Defaults to true.</summary>
    public bool RepeatHeaderOnNewPage { get; set; } = true;
}
