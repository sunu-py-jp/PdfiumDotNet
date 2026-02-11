using System;
using System.Collections.Generic;
using System.Text;
using PdfiumNet.Geometry;

namespace PdfiumNet.Drawing;

/// <summary>
/// Represents tabular data that can be rendered onto a PDF page via <see cref="PdfCanvas.DrawTable"/>.
/// </summary>
public sealed class PdfTable
{
    /// <summary>
    /// Creates a table from a two-dimensional string array.
    /// </summary>
    /// <param name="data">Row-major data. When <paramref name="style"/>.HasHeader is true the first row is treated as the header.</param>
    /// <param name="style">Optional style overrides.</param>
    public PdfTable(string[][] data, PdfTableStyle? style = null)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length == 0) throw new ArgumentException("Data must contain at least one row.", nameof(data));
        Data = data;
        Style = style ?? new PdfTableStyle();
    }

    /// <summary>The raw table data (row-major).</summary>
    public string[][] Data { get; }

    /// <summary>The style used when rendering.</summary>
    public PdfTableStyle Style { get; }

    // ── Factory methods ────────────────────────────────────────────

    /// <summary>
    /// Parses a CSV string into a <see cref="PdfTable"/>. Supports double-quoted fields containing
    /// commas and newlines (RFC 4180).
    /// </summary>
    public static PdfTable FromCsv(string csv, PdfTableStyle? style = null)
    {
        if (csv is null) throw new ArgumentNullException(nameof(csv));
        var rows = ParseCsv(csv, ',');
        return new PdfTable(rows, style);
    }

    /// <summary>
    /// Parses a TSV (tab-separated) string into a <see cref="PdfTable"/>.
    /// </summary>
    public static PdfTable FromTsv(string tsv, PdfTableStyle? style = null)
    {
        if (tsv is null) throw new ArgumentNullException(nameof(tsv));
        var rows = ParseCsv(tsv, '\t');
        return new PdfTable(rows, style);
    }

    // ── Rendering ──────────────────────────────────────────────────

    /// <summary>
    /// Renders the table onto the given canvas starting at (<paramref name="x"/>, <paramref name="y"/>).
    /// When the table overflows the page, new pages are added to the document automatically.
    /// </summary>
    /// <param name="canvas">The canvas (and its backing page/document) to draw on.</param>
    /// <param name="x">Left edge X coordinate.</param>
    /// <param name="y">Top edge Y coordinate (PDF coordinate system: top is higher Y).</param>
    /// <param name="maxWidth">Maximum width available. When null, uses page width minus 2*x.</param>
    /// <returns>A result containing the pages used and the final bottom Y position.</returns>
    internal PdfTableResult Render(PdfCanvas canvas, float x, float y, float? maxWidth)
    {
        var s = Style;
        var data = Data;
        int colCount = 0;
        foreach (var row in data)
            if (row.Length > colCount) colCount = row.Length;

        if (colCount == 0)
            return new PdfTableResult(new[] { canvas.Page }, y);

        var pageWidth = canvas.Page.Width;
        var availableWidth = maxWidth ?? (pageWidth - 2 * x);
        var colWidths = ComputeColumnWidths(s, colCount, availableWidth);
        float tableWidth = 0;
        foreach (var w in colWidths) tableWidth += w;

        var pages = new List<PdfPage> { canvas.Page };
        var currentCanvas = canvas;
        var currentPage = canvas.Page;
        var doc = canvas.Page.Document;

        int headerRows = s.HasHeader ? 1 : 0;
        float curY = y;
        float bottomMargin = 40f; // minimum margin before page bottom

        // Track per-page row ranges for grid drawing
        var pageSegments = new List<PageSegment>();
        int pageFirstRow = 0;
        float pageTopY = y;

        for (int rowIdx = 0; rowIdx < data.Length; rowIdx++)
        {
            bool isHeader = s.HasHeader && rowIdx == 0;
            float rowHeight = isHeader ? s.HeaderRowHeight : s.RowHeight;

            // Check if we need a new page
            if (curY - rowHeight < bottomMargin && rowIdx > 0)
            {
                // Draw grid for current page segment
                pageSegments.Add(new PageSegment(currentCanvas, x, pageTopY, tableWidth, colWidths, curY, pageFirstRow, rowIdx, headerRows > 0));

                // Create new page
                currentPage.GenerateContent();
                currentPage = doc.AddPage(new PdfSize(canvas.Page.Width, canvas.Page.Height));
                pages.Add(currentPage);
                currentCanvas = currentPage.GetCanvas();

                curY = canvas.Page.Height - 40f; // top margin on new page
                pageTopY = curY;
                pageFirstRow = rowIdx;

                // Re-draw header on new page
                if (s.RepeatHeaderOnNewPage && headerRows > 0)
                {
                    DrawRowBackground(currentCanvas, x, curY, tableWidth, s.HeaderRowHeight, s.HeaderBackgroundColor);
                    DrawRowText(currentCanvas, data[0], x, curY, colWidths, s, isHeaderRow: true);
                    curY -= s.HeaderRowHeight;
                }
            }

            // Draw row background
            if (isHeader)
            {
                DrawRowBackground(currentCanvas, x, curY, tableWidth, rowHeight, s.HeaderBackgroundColor);
            }
            else if (s.AlternateRowColor.A > 0)
            {
                // Zebra stripe: even data rows (0-based data index)
                int dataIdx = rowIdx - headerRows;
                if (dataIdx % 2 == 1)
                    DrawRowBackground(currentCanvas, x, curY, tableWidth, rowHeight, s.AlternateRowColor);
            }

            // Draw row text
            DrawRowText(currentCanvas, data[rowIdx], x, curY, colWidths, s, isHeaderRow: isHeader);

            curY -= rowHeight;
        }

        // Draw grid for the last page segment
        pageSegments.Add(new PageSegment(currentCanvas, x, pageTopY, tableWidth, colWidths, curY, pageFirstRow, data.Length, headerRows > 0));

        foreach (var seg in pageSegments)
            DrawGrid(seg);

        return new PdfTableResult(pages, curY);
    }

    // ── Private drawing helpers ────────────────────────────────────

    private static void DrawRowBackground(PdfCanvas cv, float x, float topY, float width, float height, PdfColor color)
    {
        cv.SetFillColor(color);
        cv.SetStrokeColor(color);
        cv.SetStrokeWidth(0.1f);
        cv.DrawRectangle(x + 0.5f, topY - height + 0.5f, width - 1, height - 1, DrawMode.FillAndStroke);
    }

    private void DrawRowText(PdfCanvas cv, string[] row, float x, float topY, float[] colWidths, PdfTableStyle s, bool isHeaderRow)
    {
        var fontName = isHeaderRow ? s.HeaderFontName : s.FontName;
        var fontSize = isHeaderRow ? s.HeaderFontSize : s.FontSize;
        var textColor = isHeaderRow ? s.HeaderTextColor : s.TextColor;
        var rowHeight = isHeaderRow ? s.HeaderRowHeight : s.RowHeight;

        cv.SetFillColor(textColor);
        float cellX = x;
        for (int c = 0; c < colWidths.Length; c++)
        {
            var text = c < row.Length ? row[c] : "";
            if (text.Length > 0)
            {
                var align = GetColumnAlignment(s, c);
                float textX = ComputeTextX(cellX, colWidths[c], text, fontSize, s.PaddingX, align);
                float textY = topY - rowHeight + s.PaddingY;
                cv.DrawText(text, textX, textY, fontName, fontSize);
            }
            cellX += colWidths[c];
        }
    }

    private static PdfTableColumnAlignment GetColumnAlignment(PdfTableStyle s, int col)
    {
        if (s.ColumnAlignments is null || col >= s.ColumnAlignments.Length)
            return PdfTableColumnAlignment.Left;
        return s.ColumnAlignments[col];
    }

    private static float EstimateTextWidth(string text, float fontSize)
    {
        // Rough estimate — PDFium has no text width measurement API for standard fonts
        return text.Length * fontSize * 0.5f;
    }

    private static float ComputeTextX(float cellX, float cellWidth, string text, float fontSize, float paddingX, PdfTableColumnAlignment align)
    {
        switch (align)
        {
            case PdfTableColumnAlignment.Right:
                return cellX + cellWidth - paddingX - EstimateTextWidth(text, fontSize);
            case PdfTableColumnAlignment.Center:
                return cellX + (cellWidth - EstimateTextWidth(text, fontSize)) / 2f;
            default:
                return cellX + paddingX;
        }
    }

    private static float[] ComputeColumnWidths(PdfTableStyle s, int colCount, float availableWidth)
    {
        if (s.ColumnWidths is not null && s.ColumnWidths.Length >= colCount)
        {
            var result = new float[colCount];
            Array.Copy(s.ColumnWidths, result, colCount);
            return result;
        }

        var equal = availableWidth / colCount;
        var widths = new float[colCount];
        for (int i = 0; i < colCount; i++)
            widths[i] = equal;
        return widths;
    }

    // ── Grid drawing ───────────────────────────────────────────────

    private readonly record struct PageSegment(
        PdfCanvas Canvas, float X, float TopY, float TableWidth,
        float[] ColWidths, float BottomY, int FirstRow, int EndRow, bool HasHeader);

    private void DrawGrid(PageSegment seg)
    {
        var s = Style;
        var cv = seg.Canvas;
        float x = seg.X;
        float topY = seg.TopY;
        float bottomY = seg.BottomY;
        var colWidths = seg.ColWidths;
        float tableWidth = seg.TableWidth;

        cv.SetStrokeColor(s.BorderColor);

        // Count rendered rows for this segment
        int renderedRows = seg.EndRow - seg.FirstRow;
        bool hasRepeatedHeader = seg.FirstRow > 0 && s.HasHeader && s.RepeatHeaderOnNewPage;
        int totalVisualRows = renderedRows + (hasRepeatedHeader ? 1 : 0);

        // Horizontal lines
        float lineY = topY;
        for (int r = 0; r <= totalVisualRows; r++)
        {
            bool isOuter = r == 0 || r == totalVisualRows;
            bool isHeaderBottom = (seg.HasHeader && r == 1) || (hasRepeatedHeader && r == 1);
            cv.SetStrokeWidth(isOuter || isHeaderBottom ? s.OuterBorderWidth : s.InnerBorderWidth);
            cv.DrawLine(x, lineY, x + tableWidth, lineY);

            if (r < totalVisualRows)
            {
                bool isHeaderVisualRow;
                if (hasRepeatedHeader)
                    isHeaderVisualRow = r == 0;
                else
                    isHeaderVisualRow = seg.HasHeader && seg.FirstRow == 0 && r == 0;

                lineY -= isHeaderVisualRow ? s.HeaderRowHeight : s.RowHeight;
            }
        }

        // Vertical lines
        float colX = x;
        for (int c = 0; c <= colWidths.Length; c++)
        {
            bool isOuter = c == 0 || c == colWidths.Length;
            cv.SetStrokeWidth(isOuter ? s.OuterBorderWidth : s.InnerBorderWidth);
            cv.DrawLine(colX, topY, colX, bottomY);
            if (c < colWidths.Length) colX += colWidths[c];
        }
    }

    // ── CSV / TSV parser ───────────────────────────────────────────

    private static string[][] ParseCsv(string input, char delimiter)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;
        int i = 0;

        while (i < input.Length)
        {
            char ch = input[i];

            if (inQuotes)
            {
                if (ch == '"')
                {
                    if (i + 1 < input.Length && input[i + 1] == '"')
                    {
                        sb.Append('"');
                        i += 2;
                    }
                    else
                    {
                        inQuotes = false;
                        i++;
                    }
                }
                else
                {
                    sb.Append(ch);
                    i++;
                }
            }
            else
            {
                if (ch == '"')
                {
                    inQuotes = true;
                    i++;
                }
                else if (ch == delimiter)
                {
                    fields.Add(sb.ToString());
                    sb.Clear();
                    i++;
                }
                else if (ch == '\r' || ch == '\n')
                {
                    fields.Add(sb.ToString());
                    sb.Clear();
                    rows.Add(fields.ToArray());
                    fields.Clear();
                    if (ch == '\r' && i + 1 < input.Length && input[i + 1] == '\n')
                        i++;
                    i++;
                }
                else
                {
                    sb.Append(ch);
                    i++;
                }
            }
        }

        // Last field / row
        if (sb.Length > 0 || fields.Count > 0)
        {
            fields.Add(sb.ToString());
            rows.Add(fields.ToArray());
        }

        return rows.ToArray();
    }
}
