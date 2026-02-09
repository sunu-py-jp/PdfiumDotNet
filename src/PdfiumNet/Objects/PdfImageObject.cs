using PdfiumNet.Exceptions;
using PdfiumNet.Geometry;
using PdfiumNet.Native;

namespace PdfiumNet.Objects;

/// <summary>
/// Represents an image object on a PDF page.
/// </summary>
public sealed class PdfImageObject : PdfPageObject
{
    internal PdfImageObject(IntPtr handle) : base(handle) { }

    /// <summary>
    /// Creates a new image object.
    /// </summary>
    public static PdfImageObject Create(PdfDocument document)
    {
        var handle = PdfiumNative.FPDFPageObj_NewImageObj(document.Handle);
        if (handle == IntPtr.Zero)
            throw new PdfiumException("Failed to create image object.");
        return new PdfImageObject(handle);
    }

    /// <summary>
    /// Sets the image from a PdfBitmap.
    /// </summary>
    public void SetBitmap(PdfBitmap bitmap)
    {
        if (!PdfiumNative.FPDFImageObj_SetBitmap(IntPtr.Zero, 0, Handle, bitmap.Handle))
            throw new PdfiumException("Failed to set image bitmap.");
    }

    /// <summary>
    /// Sets the image transformation matrix.
    /// The default image is 1x1 unit at origin; use this to position and scale.
    /// </summary>
    public void SetMatrix(PdfMatrix matrix)
    {
        PdfiumNative.FPDFImageObj_SetMatrix(Handle, matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
    }

    /// <summary>
    /// Sets the image position and size.
    /// </summary>
    public void SetBounds(float x, float y, float width, float height)
    {
        SetMatrix(new PdfMatrix(width, 0, 0, height, x, y));
    }
}
