namespace PdfiumNet.Geometry;

/// <summary>
/// Represents a size in PDF points (1 point = 1/72 inch).
/// </summary>
public readonly record struct PdfSize(float Width, float Height)
{
    /// <summary>US Letter size (8.5 x 11 inches).</summary>
    public static readonly PdfSize Letter = new(612, 792);

    /// <summary>A4 size (210 x 297 mm).</summary>
    public static readonly PdfSize A4 = new(595.28f, 841.89f);

    /// <summary>US Legal size (8.5 x 14 inches).</summary>
    public static readonly PdfSize Legal = new(612, 1008);

    public override string ToString() => $"{Width} x {Height}";
}
