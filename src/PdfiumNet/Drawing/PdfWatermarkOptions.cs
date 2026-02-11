namespace PdfiumNet.Drawing;

/// <summary>
/// Options for drawing a watermark on a PDF page.
/// </summary>
public sealed class PdfWatermarkOptions
{
    /// <summary>
    /// The watermark text. Defaults to "CONFIDENTIAL".
    /// </summary>
    public string Text { get; set; } = "CONFIDENTIAL";

    /// <summary>
    /// The font name (standard PDF font). Defaults to "Helvetica".
    /// </summary>
    public string FontName { get; set; } = "Helvetica";

    /// <summary>
    /// The font size in points. Defaults to 60.
    /// </summary>
    public float FontSize { get; set; } = 60;

    /// <summary>
    /// The watermark color (including alpha for transparency). Defaults to semi-transparent red.
    /// </summary>
    public PdfColor Color { get; set; } = new(255, 0, 0, 64);

    /// <summary>
    /// Rotation angle in degrees (counterclockwise). Defaults to 45.
    /// </summary>
    public float RotationDegrees { get; set; } = 45;

    /// <summary>
    /// Optional X position override. If null, the watermark is centered horizontally.
    /// </summary>
    public float? X { get; set; }

    /// <summary>
    /// Optional Y position override. If null, the watermark is centered vertically.
    /// </summary>
    public float? Y { get; set; }
}
