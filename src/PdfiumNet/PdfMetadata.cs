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
        return NativeStringHelper.ReadUtf16((buf, len) =>
            PdfiumNative.FPDF_GetMetaText(handle, tag, buf, len));
    }
}
