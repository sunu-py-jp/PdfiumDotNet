namespace PdfiumNet.Export;

/// <summary>
/// Minimal BMP encoder. Writes BITMAPFILEHEADER + BITMAPINFOHEADER + raw pixel data.
/// BMP stores rows bottom-to-top, so rows are written in reverse order.
/// </summary>
internal static class BmpEncoder
{
    private const int FileHeaderSize = 14;
    private const int InfoHeaderSize = 40;
    private const int HeaderSize = FileHeaderSize + InfoHeaderSize;

    /// <summary>
    /// Encodes BGRA pixel data as a 32-bit BMP image.
    /// </summary>
    public static void Encode(Stream output, int width, int height, int stride, byte[] bgraData)
    {
        var rowSize = width * 4; // 32-bit BGRA, no padding needed (already 4-byte aligned)
        var imageSize = rowSize * height;
        var fileSize = HeaderSize + imageSize;

        // BITMAPFILEHEADER (14 bytes)
        output.WriteByte(0x42); // 'B'
        output.WriteByte(0x4D); // 'M'
        WriteLittleEndian32(output, fileSize);
        WriteLittleEndian16(output, 0); // reserved1
        WriteLittleEndian16(output, 0); // reserved2
        WriteLittleEndian32(output, HeaderSize); // offset to pixel data

        // BITMAPINFOHEADER (40 bytes)
        WriteLittleEndian32(output, InfoHeaderSize);
        WriteLittleEndian32(output, width);
        WriteLittleEndian32(output, height); // positive = bottom-up
        WriteLittleEndian16(output, 1);      // planes
        WriteLittleEndian16(output, 32);     // bits per pixel
        WriteLittleEndian32(output, 0);      // compression: BI_RGB
        WriteLittleEndian32(output, imageSize);
        WriteLittleEndian32(output, 2835);   // X pixels per meter (~72 DPI)
        WriteLittleEndian32(output, 2835);   // Y pixels per meter
        WriteLittleEndian32(output, 0);      // colors used
        WriteLittleEndian32(output, 0);      // important colors

        // Pixel data (bottom-to-top row order)
        for (var y = height - 1; y >= 0; y--)
        {
            output.Write(bgraData, y * stride, rowSize);
        }
    }

    private static void WriteLittleEndian32(Stream stream, int value)
    {
        stream.WriteByte((byte)value);
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 24));
    }

    private static void WriteLittleEndian16(Stream stream, int value)
    {
        stream.WriteByte((byte)value);
        stream.WriteByte((byte)(value >> 8));
    }
}
