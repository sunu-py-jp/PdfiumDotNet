using System.Runtime.InteropServices;
using PdfiumNet.Exceptions;
using PdfiumNet.Export;
using PdfiumNet.Native;

namespace PdfiumNet;

/// <summary>
/// Represents a bitmap for rendering PDF pages or creating images for insertion.
/// </summary>
public sealed class PdfBitmap : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    private PdfBitmap(IntPtr handle)
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

    /// <summary>Width in pixels.</summary>
    public int Width => PdfiumNative.FPDFBitmap_GetWidth(Handle);

    /// <summary>Height in pixels.</summary>
    public int Height => PdfiumNative.FPDFBitmap_GetHeight(Handle);

    /// <summary>Stride (bytes per row).</summary>
    public int Stride => PdfiumNative.FPDFBitmap_GetStride(Handle);

    /// <summary>Pointer to the raw pixel buffer.</summary>
    public IntPtr Buffer => PdfiumNative.FPDFBitmap_GetBuffer(Handle);

    /// <summary>
    /// Creates a new BGRA bitmap.
    /// </summary>
    public static PdfBitmap Create(int width, int height, bool hasAlpha = true)
    {
        var handle = PdfiumNative.FPDFBitmap_Create(width, height, hasAlpha ? 1 : 0);
        if (handle == IntPtr.Zero)
            throw new PdfiumException($"Failed to create bitmap ({width}x{height}).");
        return new PdfBitmap(handle);
    }

    /// <summary>
    /// Creates a bitmap from an existing pixel buffer.
    /// </summary>
    public static PdfBitmap CreateFromBuffer(int width, int height, BitmapFormat format,
        IntPtr buffer, int stride)
    {
        var handle = PdfiumNative.FPDFBitmap_CreateEx(width, height, (int)format, buffer, stride);
        if (handle == IntPtr.Zero)
            throw new PdfiumException("Failed to create bitmap from buffer.");
        return new PdfBitmap(handle);
    }

    /// <summary>
    /// Fills a rectangle with the specified color (0xAARRGGBB).
    /// </summary>
    public void FillRect(int left, int top, int width, int height, uint color)
    {
        PdfiumNative.FPDFBitmap_FillRect(Handle, left, top, width, height, color);
    }

    /// <summary>
    /// Copies the pixel data to a byte array.
    /// </summary>
    public byte[] ToArray()
    {
        var size = Stride * Height;
        var data = new byte[size];
        Marshal.Copy(Buffer, data, 0, size);
        return data;
    }

    /// <summary>
    /// Saves the bitmap as a PNG image to the specified stream.
    /// </summary>
    public void SaveAsPng(Stream stream)
    {
        PngEncoder.Encode(stream, Width, Height, Stride, ToArray());
    }

    /// <summary>
    /// Saves the bitmap as a BMP image to the specified stream.
    /// </summary>
    public void SaveAsBmp(Stream stream)
    {
        BmpEncoder.Encode(stream, Width, Height, Stride, ToArray());
    }

    /// <summary>
    /// Returns the bitmap encoded as a PNG byte array.
    /// </summary>
    public byte[] ToPng()
    {
        using var ms = new MemoryStream();
        SaveAsPng(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Returns the bitmap encoded as a BMP byte array.
    /// </summary>
    public byte[] ToBmp()
    {
        using var ms = new MemoryStream();
        SaveAsBmp(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Copies the pixel data into the provided span.
    /// </summary>
    public void CopyTo(Span<byte> destination)
    {
        var size = Stride * Height;
        if (destination.Length < size)
            throw new ArgumentException("Destination buffer is too small.");
        unsafe
        {
            new ReadOnlySpan<byte>((void*)Buffer, size).CopyTo(destination);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_handle != IntPtr.Zero)
        {
            PdfiumNative.FPDFBitmap_Destroy(_handle);
            _handle = IntPtr.Zero;
        }
    }
}

public enum BitmapFormat
{
    Unknown = 0,
    Gray = 1,
    Bgr = 2,
    BgrX = 3,
    Bgra = 4,
}
