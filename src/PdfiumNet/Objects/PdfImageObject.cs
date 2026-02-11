using System.Runtime.InteropServices;
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

    /// <summary>
    /// Gets the decoded (uncompressed) image data.
    /// </summary>
    public byte[]? GetImageDataDecoded()
    {
        var size = PdfiumNative.FPDFImageObj_GetImageDataDecoded(Handle, IntPtr.Zero, 0);
        if (size == 0)
            return null;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            PdfiumNative.FPDFImageObj_GetImageDataDecoded(Handle, buffer, size);
            var result = new byte[size];
            Marshal.Copy(buffer, result, 0, (int)size);
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Gets the raw (compressed) image data as stored in the PDF.
    /// </summary>
    public byte[]? GetImageDataRaw()
    {
        var size = PdfiumNative.FPDFImageObj_GetImageDataRaw(Handle, IntPtr.Zero, 0);
        if (size == 0)
            return null;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            PdfiumNative.FPDFImageObj_GetImageDataRaw(Handle, buffer, size);
            var result = new byte[size];
            Marshal.Copy(buffer, result, 0, (int)size);
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
