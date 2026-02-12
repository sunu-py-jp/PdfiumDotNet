using System.IO.Compression;

namespace PdfiumNet.Export;

/// <summary>
/// Minimal PNG encoder with no external dependencies.
/// Uses System.IO.Compression.ZLibStream for IDAT chunk compression.
/// </summary>
internal static class PngEncoder
{
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    /// <summary>
    /// Encodes BGRA pixel data as a PNG image.
    /// </summary>
    public static void Encode(Stream output, int width, int height, int stride, byte[] bgraData)
    {
        output.Write(PngSignature);

        // IHDR chunk
        var ihdr = new byte[13];
        WriteBigEndian(ihdr, 0, width);
        WriteBigEndian(ihdr, 4, height);
        ihdr[8] = 8;  // bit depth
        ihdr[9] = 6;  // color type: RGBA
        ihdr[10] = 0; // compression method
        ihdr[11] = 0; // filter method
        ihdr[12] = 0; // interlace method
        WriteChunk(output, "IHDR"u8, ihdr);

        // IDAT chunk - zlib-compressed filtered scanlines
        using var idatMs = new MemoryStream();
        using (var zlib = new ZLibStream(idatMs, CompressionLevel.Fastest, leaveOpen: true))
        {
            var rowBuffer = new byte[1 + width * 4]; // filter byte + RGBA pixels
            for (var y = 0; y < height; y++)
            {
                rowBuffer[0] = 0; // filter: None
                var srcOffset = y * stride;
                for (var x = 0; x < width; x++)
                {
                    var si = srcOffset + x * 4;
                    var di = 1 + x * 4;
                    // BGRA → RGBA
                    rowBuffer[di] = bgraData[si + 2];     // R
                    rowBuffer[di + 1] = bgraData[si + 1]; // G
                    rowBuffer[di + 2] = bgraData[si];     // B
                    rowBuffer[di + 3] = bgraData[si + 3]; // A
                }
                zlib.Write(rowBuffer, 0, rowBuffer.Length);
            }
        }
        WriteChunk(output, "IDAT"u8, idatMs.ToArray());

        // IEND chunk
        WriteChunk(output, "IEND"u8, ReadOnlySpan<byte>.Empty);
    }

    private static void WriteChunk(Stream output, ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
    {
        Span<byte> lenBytes = stackalloc byte[4];
        WriteBigEndian(lenBytes, 0, data.Length);
        output.Write(lenBytes);

        Span<byte> typeBytes = stackalloc byte[4];
        type.CopyTo(typeBytes);
        output.Write(typeBytes);

        output.Write(data);

        // CRC over type + data
        var crc = 0xFFFFFFFFu;
        foreach (var b in typeBytes)
            crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        foreach (var b in data)
            crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        crc ^= 0xFFFFFFFFu;

        Span<byte> crcBytes = stackalloc byte[4];
        WriteBigEndian(crcBytes, 0, (int)crc);
        output.Write(crcBytes);
    }

    private static void WriteBigEndian(Span<byte> buf, int offset, int value)
    {
        buf[offset] = (byte)(value >> 24);
        buf[offset + 1] = (byte)(value >> 16);
        buf[offset + 2] = (byte)(value >> 8);
        buf[offset + 3] = (byte)value;
    }

    private static readonly uint[] CrcTable = GenerateCrcTable();

    private static uint[] GenerateCrcTable()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            var c = n;
            for (var k = 0; k < 8; k++)
                c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            table[n] = c;
        }
        return table;
    }
}
