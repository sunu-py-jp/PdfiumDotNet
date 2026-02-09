using System.Collections;
using PdfiumNet.Native;

namespace PdfiumNet.Objects;

/// <summary>
/// Provides access to the page objects on a PDF page.
/// </summary>
public sealed class PdfPageObjectCollection : IReadOnlyList<PdfPageObject>
{
    private readonly PdfPage _page;

    internal PdfPageObjectCollection(PdfPage page)
    {
        _page = page;
    }

    public int Count => PdfiumNative.FPDFPage_CountObjects(_page.Handle);

    public PdfPageObject this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            var handle = PdfiumNative.FPDFPage_GetObject(_page.Handle, index);
            return PdfPageObject.Wrap(handle);
        }
    }

    public IEnumerator<PdfPageObject> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
