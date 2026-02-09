using PdfiumNet.Drawing;
using PdfiumNet.Geometry;
using PdfiumNet.Native;

namespace PdfiumNet.Objects;

/// <summary>
/// Base class for page objects (text, path, image).
/// Manages ownership: before insertion into a page, the C# side owns and must dispose;
/// after insertion, PDFium owns the handle.
/// </summary>
public abstract class PdfPageObject : IDisposable
{
    private IntPtr _handle;
    private bool _insertedIntoPage;
    private bool _disposed;

    internal PdfPageObject(IntPtr handle)
    {
        _handle = handle;
    }

    internal IntPtr Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _handle;
        }
    }

    /// <summary>
    /// Gets the type of this page object.
    /// </summary>
    public PdfPageObjectType ObjectType => (PdfPageObjectType)PdfiumNative.FPDFPageObj_GetType(Handle);

    /// <summary>
    /// Gets the bounding box of this object in page coordinates.
    /// </summary>
    public PdfRectangle Bounds
    {
        get
        {
            if (!PdfiumNative.FPDFPageObj_GetBounds(Handle, out var left, out var bottom, out var right, out var top))
                return default;
            return new PdfRectangle(left, bottom, right, top);
        }
    }

    /// <summary>
    /// Sets the fill color of this object.
    /// </summary>
    public void SetFillColor(PdfColor color)
    {
        PdfiumNative.FPDFPageObj_SetFillColor(Handle, color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Gets the fill color of this object.
    /// </summary>
    public PdfColor? GetFillColor()
    {
        if (!PdfiumNative.FPDFPageObj_GetFillColor(Handle, out var r, out var g, out var b, out var a))
            return null;
        return new PdfColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Sets the stroke color of this object.
    /// </summary>
    public void SetStrokeColor(PdfColor color)
    {
        PdfiumNative.FPDFPageObj_SetStrokeColor(Handle, color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Gets the stroke color of this object.
    /// </summary>
    public PdfColor? GetStrokeColor()
    {
        if (!PdfiumNative.FPDFPageObj_GetStrokeColor(Handle, out var r, out var g, out var b, out var a))
            return null;
        return new PdfColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Sets the stroke width.
    /// </summary>
    public void SetStrokeWidth(float width)
    {
        PdfiumNative.FPDFPageObj_SetStrokeWidth(Handle, width);
    }

    /// <summary>
    /// Gets the stroke width.
    /// </summary>
    public float GetStrokeWidth()
    {
        PdfiumNative.FPDFPageObj_GetStrokeWidth(Handle, out var width);
        return width;
    }

    /// <summary>
    /// Applies a transformation matrix to this object.
    /// </summary>
    public void Transform(PdfMatrix matrix)
    {
        PdfiumNative.FPDFPageObj_Transform(Handle, matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
    }

    internal void MarkInserted() => _insertedIntoPage = true;
    internal void MarkRemoved() => _insertedIntoPage = false;

    internal static PdfPageObject Wrap(IntPtr handle)
    {
        var type = (PdfPageObjectType)PdfiumNative.FPDFPageObj_GetType(handle);
        PdfPageObject obj = type switch
        {
            PdfPageObjectType.Text => new PdfTextObject(handle),
            PdfPageObjectType.Path => new PdfPathObject(handle),
            PdfPageObjectType.Image => new PdfImageObject(handle),
            _ => new PdfUnknownObject(handle),
        };
        // Objects retrieved from a page are owned by PDFium
        obj._insertedIntoPage = true;
        return obj;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        // Only destroy if we still own it (not inserted into a page)
        if (!_insertedIntoPage && _handle != IntPtr.Zero)
        {
            PdfiumNative.FPDFPageObj_Destroy(_handle);
        }
        _handle = IntPtr.Zero;
    }
}

public enum PdfPageObjectType
{
    Unknown = 0,
    Text = 1,
    Path = 2,
    Image = 3,
    Shading = 4,
    Form = 5,
}

/// <summary>
/// Represents an unknown/unsupported page object type.
/// </summary>
internal sealed class PdfUnknownObject : PdfPageObject
{
    internal PdfUnknownObject(IntPtr handle) : base(handle) { }
}
