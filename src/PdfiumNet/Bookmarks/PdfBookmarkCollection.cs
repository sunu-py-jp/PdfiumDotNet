using System.Collections;
using PdfiumNet.Native;

namespace PdfiumNet.Bookmarks;

/// <summary>
/// Read-only collection of top-level bookmarks in a PDF document.
/// </summary>
public sealed class PdfBookmarkCollection : IReadOnlyList<PdfBookmark>
{
    private readonly PdfDocument _document;
    private List<PdfBookmark>? _bookmarks;

    internal PdfBookmarkCollection(PdfDocument document)
    {
        _document = document;
    }

    private List<PdfBookmark> LoadBookmarks()
    {
        if (_bookmarks != null)
            return _bookmarks;

        _bookmarks = new List<PdfBookmark>();
        var child = PdfiumNative.FPDFBookmark_GetFirstChild(_document.Handle, IntPtr.Zero);
        while (child != IntPtr.Zero)
        {
            _bookmarks.Add(new PdfBookmark(_document, child));
            child = PdfiumNative.FPDFBookmark_GetNextSibling(_document.Handle, child);
        }
        return _bookmarks;
    }

    public int Count => LoadBookmarks().Count;

    public PdfBookmark this[int index] => LoadBookmarks()[index];

    /// <summary>
    /// Finds a bookmark by its title. Returns null if not found.
    /// </summary>
    public PdfBookmark? Find(string title)
    {
        var handle = PdfiumNative.FPDFBookmark_Find(_document.Handle, title);
        return handle == IntPtr.Zero ? null : new PdfBookmark(_document, handle);
    }

    public IEnumerator<PdfBookmark> GetEnumerator() => LoadBookmarks().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
