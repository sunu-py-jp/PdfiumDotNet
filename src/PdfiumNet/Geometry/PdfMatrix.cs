namespace PdfiumNet.Geometry;

/// <summary>
/// Represents a 2D affine transformation matrix used in PDF operations.
/// The matrix is: [A B 0; C D 0; E F 1]
/// </summary>
public readonly record struct PdfMatrix(float A, float B, float C, float D, float E, float F)
{
    public static readonly PdfMatrix Identity = new(1, 0, 0, 1, 0, 0);

    public static PdfMatrix CreateTranslation(float tx, float ty)
        => new(1, 0, 0, 1, tx, ty);

    public static PdfMatrix CreateScale(float sx, float sy)
        => new(sx, 0, 0, sy, 0, 0);

    public static PdfMatrix CreateRotation(float angleRadians)
    {
        var cos = MathF.Cos(angleRadians);
        var sin = MathF.Sin(angleRadians);
        return new(cos, sin, -sin, cos, 0, 0);
    }

    public PdfMatrix Multiply(PdfMatrix other) => new(
        A * other.A + B * other.C,
        A * other.B + B * other.D,
        C * other.A + D * other.C,
        C * other.B + D * other.D,
        E * other.A + F * other.C + other.E,
        E * other.B + F * other.D + other.F
    );

    public PdfPoint Transform(PdfPoint point) => new(
        A * point.X + C * point.Y + E,
        B * point.X + D * point.Y + F
    );

    public override string ToString() => $"[{A}, {B}, {C}, {D}, {E}, {F}]";
}
