using System.Collections;
using PdfiumNet.Native;

namespace PdfiumNet.Signatures;

/// <summary>
/// Provides read-only access to the digital signatures in a PDF document.
/// </summary>
public sealed class PdfSignatureCollection : IReadOnlyList<PdfSignature>
{
    private readonly PdfDocument _document;

    internal PdfSignatureCollection(PdfDocument document)
    {
        _document = document;
    }

    /// <summary>
    /// Gets the number of signatures.
    /// </summary>
    public int Count => PdfiumNative.FPDF_GetSignatureCount(_document.Handle);

    /// <summary>
    /// Gets a signature by index.
    /// </summary>
    public PdfSignature this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var handle = PdfiumNative.FPDF_GetSignatureObject(_document.Handle, index);
            if (handle == IntPtr.Zero)
                throw new Exceptions.PdfiumException($"Failed to get signature at index {index}.");

            return new PdfSignature(handle);
        }
    }

    public IEnumerator<PdfSignature> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
