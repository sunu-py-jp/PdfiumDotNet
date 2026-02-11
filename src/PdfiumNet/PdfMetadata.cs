using System.Runtime.InteropServices;
using PdfiumNet.Native;

namespace PdfiumNet;

/// <summary>
/// Provides read-only access to PDF document metadata.
/// </summary>
public sealed class PdfMetadata
{
    private readonly PdfDocument _document;

    internal PdfMetadata(PdfDocument document)
    {
        _document = document;
    }

    public string Title => GetMetaText("Title");
    public string Author => GetMetaText("Author");
    public string Subject => GetMetaText("Subject");
    public string Keywords => GetMetaText("Keywords");
    public string Creator => GetMetaText("Creator");
    public string Producer => GetMetaText("Producer");
    public string CreationDate => GetMetaText("CreationDate");
    public string ModDate => GetMetaText("ModDate");

    private string GetMetaText(string tag)
    {
        var handle = _document.Handle;
        // First call to get required buffer size (returns bytes including trailing NUL, UTF-16)
        var length = PdfiumNative.FPDF_GetMetaText(handle, tag, IntPtr.Zero, 0);
        if (length <= 2) // 2 bytes = just the NUL terminator in UTF-16
            return string.Empty;

        var buffer = Marshal.AllocHGlobal((int)length);
        try
        {
            PdfiumNative.FPDF_GetMetaText(handle, tag, buffer, length);
            // PDFium returns UTF-16LE string; length includes the trailing NUL (2 bytes)
            return Marshal.PtrToStringUni(buffer) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
