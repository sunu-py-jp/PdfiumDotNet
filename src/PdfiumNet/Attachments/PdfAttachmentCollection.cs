using System.Collections;
using PdfiumNet.Native;

namespace PdfiumNet.Attachments;

/// <summary>
/// Provides access to the attachments (embedded files) in a PDF document.
/// </summary>
public sealed class PdfAttachmentCollection : IReadOnlyList<PdfAttachment>
{
    private readonly PdfDocument _document;

    internal PdfAttachmentCollection(PdfDocument document)
    {
        _document = document;
    }

    /// <summary>
    /// Gets the number of attachments.
    /// </summary>
    public int Count => PdfiumNative.FPDFDoc_GetAttachmentCount(_document.Handle);

    /// <summary>
    /// Gets an attachment by index.
    /// </summary>
    public PdfAttachment this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var handle = PdfiumNative.FPDFDoc_GetAttachment(_document.Handle, index);
            if (handle == IntPtr.Zero)
                throw new Exceptions.PdfiumException($"Failed to get attachment at index {index}.");

            return new PdfAttachment(_document, handle);
        }
    }

    /// <summary>
    /// Adds a new attachment with the given name.
    /// </summary>
    public PdfAttachment Add(string name)
    {
        var handle = PdfiumNative.FPDFDoc_AddAttachment(_document.Handle, name);
        if (handle == IntPtr.Zero)
            throw new Exceptions.PdfiumException($"Failed to add attachment '{name}'.");
        return new PdfAttachment(_document, handle);
    }

    /// <summary>
    /// Deletes an attachment at the specified index.
    /// </summary>
    public bool Delete(int index)
    {
        return PdfiumNative.FPDFDoc_DeleteAttachment(_document.Handle, index);
    }

    public IEnumerator<PdfAttachment> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
