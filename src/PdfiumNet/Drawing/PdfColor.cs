namespace PdfiumNet.Drawing;

/// <summary>
/// Represents an RGBA color for PDF drawing operations.
/// </summary>
public readonly record struct PdfColor(byte R, byte G, byte B, byte A = 255)
{
    public static readonly PdfColor Black = new(0, 0, 0);
    public static readonly PdfColor White = new(255, 255, 255);
    public static readonly PdfColor Red = new(255, 0, 0);
    public static readonly PdfColor Green = new(0, 128, 0);
    public static readonly PdfColor Blue = new(0, 0, 255);
    public static readonly PdfColor Transparent = new(0, 0, 0, 0);

    public static PdfColor FromArgb(byte a, byte r, byte g, byte b) => new(r, g, b, a);

    /// <summary>
    /// Returns the color as a 32-bit ARGB value (0xAARRGGBB).
    /// </summary>
    public uint ToArgb() => (uint)((A << 24) | (R << 16) | (G << 8) | B);

    public override string ToString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";
}
