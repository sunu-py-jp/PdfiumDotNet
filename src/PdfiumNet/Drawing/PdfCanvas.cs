using PdfiumNet.Geometry;
using PdfiumNet.Objects;

namespace PdfiumNet.Drawing;

/// <summary>
/// High-level drawing API for adding content to a PDF page.
/// </summary>
public sealed class PdfCanvas
{
    private PdfColor _fillColor = PdfColor.Black;
    private PdfColor _strokeColor = PdfColor.Black;
    private float _strokeWidth = 1.0f;

    internal PdfCanvas(PdfPage page)
    {
        Page = page;
    }

    internal PdfPage Page { get; }

    /// <summary>
    /// Sets the fill color for subsequent drawing operations.
    /// </summary>
    public PdfCanvas SetFillColor(PdfColor color)
    {
        _fillColor = color;
        return this;
    }

    /// <summary>
    /// Sets the stroke color for subsequent drawing operations.
    /// </summary>
    public PdfCanvas SetStrokeColor(PdfColor color)
    {
        _strokeColor = color;
        return this;
    }

    /// <summary>
    /// Sets the stroke width for subsequent drawing operations.
    /// </summary>
    public PdfCanvas SetStrokeWidth(float width)
    {
        _strokeWidth = width;
        return this;
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    public PdfPathObject DrawLine(float x1, float y1, float x2, float y2)
    {
        var path = PdfPathObject.Create(x1, y1);
        path.LineTo(x2, y2);
        ApplyStrokeStyle(path);
        path.SetDrawMode(DrawMode.Stroke);
        Page.InsertObject(path);
        return path;
    }

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    public PdfPathObject DrawRectangle(float x, float y, float width, float height,
        DrawMode mode = DrawMode.Stroke)
    {
        var path = PdfPathObject.CreateRectangle(x, y, width, height);
        ApplyStyle(path, mode);
        path.SetDrawMode(mode);
        Page.InsertObject(path);
        return path;
    }

    /// <summary>
    /// Draws a circle approximated with Bezier curves.
    /// </summary>
    public PdfPathObject DrawCircle(float cx, float cy, float radius,
        DrawMode mode = DrawMode.Stroke)
    {
        return DrawEllipse(cx, cy, radius, radius, mode);
    }

    /// <summary>
    /// Draws an ellipse approximated with Bezier curves.
    /// </summary>
    public PdfPathObject DrawEllipse(float cx, float cy, float rx, float ry,
        DrawMode mode = DrawMode.Stroke)
    {
        // Approximate ellipse with 4 cubic Bezier curves
        // Magic number for circle approximation: 0.5522847498
        const float k = 0.5522847498f;
        var kx = k * rx;
        var ky = k * ry;

        var path = PdfPathObject.Create(cx + rx, cy);
        path.BezierTo(cx + rx, cy + ky, cx + kx, cy + ry, cx, cy + ry);
        path.BezierTo(cx - kx, cy + ry, cx - rx, cy + ky, cx - rx, cy);
        path.BezierTo(cx - rx, cy - ky, cx - kx, cy - ry, cx, cy - ry);
        path.BezierTo(cx + kx, cy - ry, cx + rx, cy - ky, cx + rx, cy);
        path.ClosePath();

        ApplyStyle(path, mode);
        path.SetDrawMode(mode);
        Page.InsertObject(path);
        return path;
    }

    /// <summary>
    /// Draws a polygon from an array of points.
    /// </summary>
    public PdfPathObject DrawPolygon(PdfPoint[] points, DrawMode mode = DrawMode.Stroke)
    {
        if (points.Length < 3)
            throw new ArgumentException("A polygon requires at least 3 points.", nameof(points));

        var path = PdfPathObject.Create(points[0].X, points[0].Y);
        for (var i = 1; i < points.Length; i++)
            path.LineTo(points[i].X, points[i].Y);
        path.ClosePath();

        ApplyStyle(path, mode);
        path.SetDrawMode(mode);
        Page.InsertObject(path);
        return path;
    }

    /// <summary>
    /// Draws text on the page using a standard PDF font.
    /// </summary>
    public PdfTextObject DrawText(string text, float x, float y,
        string fontName = "Helvetica", float fontSize = 12)
    {
        var textObj = PdfTextObject.Create(Page.Document, fontName, fontSize, text);
        textObj.SetFillColor(_fillColor);
        textObj.Transform(PdfMatrix.CreateTranslation(x, y));
        Page.InsertObject(textObj);
        return textObj;
    }

    /// <summary>
    /// Draws text on the page using a loaded font.
    /// </summary>
    public PdfTextObject DrawText(string text, float x, float y, PdfFont font, float fontSize = 12)
    {
        var textObj = PdfTextObject.Create(Page.Document, font, fontSize, text);
        textObj.SetFillColor(_fillColor);
        textObj.Transform(PdfMatrix.CreateTranslation(x, y));
        Page.InsertObject(textObj);
        return textObj;
    }

    /// <summary>
    /// Draws an image from a bitmap.
    /// </summary>
    public PdfImageObject DrawImage(PdfBitmap bitmap, float x, float y, float width, float height)
    {
        var imageObj = PdfImageObject.Create(Page.Document);
        imageObj.SetBitmap(bitmap);
        imageObj.SetBounds(x, y, width, height);
        Page.InsertObject(imageObj);
        return imageObj;
    }

    /// <summary>
    /// Begins building a custom path.
    /// </summary>
    public PdfPathBuilder BeginPath(float startX, float startY)
    {
        var path = PdfPathObject.Create(startX, startY);
        ApplyStyle(path, DrawMode.Stroke);
        return new PdfPathBuilder(this, path);
    }

    private void ApplyStrokeStyle(PdfPathObject path)
    {
        path.SetStrokeColor(_strokeColor);
        path.SetStrokeWidth(_strokeWidth);
    }

    private void ApplyStyle(PdfPathObject path, DrawMode mode)
    {
        if (mode.HasFlag(DrawMode.Stroke))
        {
            path.SetStrokeColor(_strokeColor);
            path.SetStrokeWidth(_strokeWidth);
        }
        if (mode.HasFlag(DrawMode.Fill) || mode.HasFlag(DrawMode.FillEvenOdd))
        {
            path.SetFillColor(_fillColor);
        }
    }
}
