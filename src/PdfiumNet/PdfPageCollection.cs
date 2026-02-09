using System.Collections;
using PdfiumNet.Native;

namespace PdfiumNet;

/// <summary>
/// Provides indexed access to pages in a PDF document.
/// Pages are loaded on demand and cached.
/// </summary>
public sealed class PdfPageCollection : IReadOnlyList<PdfPage>, IDisposable
{
    private readonly PdfDocument _document;
    private readonly Dictionary<int, PdfPage> _loadedPages = new();

    internal PdfPageCollection(PdfDocument document)
    {
        _document = document;
    }

    public int Count => _document.PageCount;

    public PdfPage this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_loadedPages.TryGetValue(index, out var page))
                return page;

            var handle = PdfiumNative.FPDF_LoadPage(_document.Handle, index);
            if (handle == IntPtr.Zero)
                throw new Exceptions.PdfiumException($"Failed to load page {index}.");

            page = new PdfPage(_document, handle, index);
            _loadedPages[index] = page;
            return page;
        }
    }

    public IEnumerator<PdfPage> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void InvalidateCache()
    {
        foreach (var page in _loadedPages.Values)
            page.Dispose();
        _loadedPages.Clear();
    }

    public void Dispose()
    {
        foreach (var page in _loadedPages.Values)
            page.Dispose();
        _loadedPages.Clear();
    }
}
