using PdfiumNet.Drawing;
using PdfiumNet.Exceptions;
using PdfiumNet.Native;

namespace PdfiumNet.Objects;

/// <summary>
/// Represents a path object (lines, shapes) on a PDF page.
/// </summary>
public sealed class PdfPathObject : PdfPageObject
{
    internal PdfPathObject(IntPtr handle) : base(handle) { }

    /// <summary>
    /// Creates a new path object starting at the specified point.
    /// </summary>
    public static PdfPathObject Create(float x, float y)
    {
        var handle = PdfiumNative.FPDFPageObj_CreateNewPath(x, y);
        if (handle == IntPtr.Zero)
            throw new PdfiumException("Failed to create path object.");
        return new PdfPathObject(handle);
    }

    /// <summary>
    /// Creates a rectangle path object.
    /// </summary>
    public static PdfPathObject CreateRectangle(float x, float y, float width, float height)
    {
        var handle = PdfiumNative.FPDFPageObj_CreateNewRect(x, y, width, height);
        if (handle == IntPtr.Zero)
            throw new PdfiumException("Failed to create rectangle object.");
        return new PdfPathObject(handle);
    }

    /// <summary>
    /// Sets the draw mode (fill/stroke).
    /// </summary>
    public void SetDrawMode(DrawMode mode)
    {
        int fillMode = 0;
        bool stroke = mode.HasFlag(DrawMode.Stroke);

        if (mode.HasFlag(DrawMode.FillEvenOdd))
            fillMode = 2; // FPDF_FILLMODE_ALTERNATE
        else if (mode.HasFlag(DrawMode.Fill))
            fillMode = 1; // FPDF_FILLMODE_WINDING

        PdfiumNative.FPDFPath_SetDrawMode(Handle, fillMode, stroke);
    }

    public void MoveTo(float x, float y) => PdfiumNative.FPDFPath_MoveTo(Handle, x, y);
    public void LineTo(float x, float y) => PdfiumNative.FPDFPath_LineTo(Handle, x, y);

    public void BezierTo(float x1, float y1, float x2, float y2, float x3, float y3)
        => PdfiumNative.FPDFPath_BezierTo(Handle, x1, y1, x2, y2, x3, y3);

    public void ClosePath() => PdfiumNative.FPDFPath_Close(Handle);
}
