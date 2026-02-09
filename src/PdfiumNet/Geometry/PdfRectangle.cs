namespace PdfiumNet.Geometry;

/// <summary>
/// Represents a rectangle in PDF coordinate space.
/// Left/Bottom is the lower-left corner, Right/Top is the upper-right corner.
/// </summary>
public readonly record struct PdfRectangle(float Left, float Bottom, float Right, float Top)
{
    public float Width => Right - Left;
    public float Height => Top - Bottom;
    public PdfPoint Center => new((Left + Right) / 2, (Bottom + Top) / 2);

    public static PdfRectangle FromLTWH(float left, float top, float width, float height)
        => new(left, top - height, left + width, top);

    public bool Contains(PdfPoint point)
        => point.X >= Left && point.X <= Right && point.Y >= Bottom && point.Y <= Top;

    public override string ToString() => $"[({Left}, {Bottom}) - ({Right}, {Top})]";
}
