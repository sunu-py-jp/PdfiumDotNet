namespace PdfiumNet.Geometry;

/// <summary>
/// Represents a point in PDF coordinate space (origin at bottom-left).
/// </summary>
public readonly record struct PdfPoint(float X, float Y)
{
    public static readonly PdfPoint Empty = new(0, 0);

    public static PdfPoint operator +(PdfPoint a, PdfPoint b) => new(a.X + b.X, a.Y + b.Y);
    public static PdfPoint operator -(PdfPoint a, PdfPoint b) => new(a.X - b.X, a.Y - b.Y);

    public override string ToString() => $"({X}, {Y})";
}
