using PdfiumNet.Geometry;
using PdfiumNet.Objects;

namespace PdfiumNet.Drawing;

/// <summary>
/// Fluent builder for creating custom PDF paths.
/// </summary>
public sealed class PdfPathBuilder
{
    private readonly PdfCanvas _canvas;
    private readonly PdfPathObject _pathObject;

    internal PdfPathBuilder(PdfCanvas canvas, PdfPathObject pathObject)
    {
        _canvas = canvas;
        _pathObject = pathObject;
    }

    /// <summary>
    /// Moves the current point to the specified position.
    /// </summary>
    public PdfPathBuilder MoveTo(float x, float y)
    {
        _pathObject.MoveTo(x, y);
        return this;
    }

    /// <summary>
    /// Draws a line from the current point to the specified position.
    /// </summary>
    public PdfPathBuilder LineTo(float x, float y)
    {
        _pathObject.LineTo(x, y);
        return this;
    }

    /// <summary>
    /// Draws a cubic Bezier curve.
    /// </summary>
    public PdfPathBuilder BezierTo(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        _pathObject.BezierTo(x1, y1, x2, y2, x3, y3);
        return this;
    }

    /// <summary>
    /// Closes the current subpath.
    /// </summary>
    public PdfPathBuilder Close()
    {
        _pathObject.ClosePath();
        return this;
    }

    /// <summary>
    /// Completes the path and adds it to the page.
    /// </summary>
    public PdfPathObject End(DrawMode mode = DrawMode.Stroke)
    {
        _pathObject.SetDrawMode(mode);
        _canvas.Page.InsertObject(_pathObject);
        return _pathObject;
    }
}
