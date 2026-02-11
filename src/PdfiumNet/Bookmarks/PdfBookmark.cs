using PdfiumNet.Native;

namespace PdfiumNet.Bookmarks;

/// <summary>
/// Represents a bookmark (outline item) in a PDF document.
/// Bookmark handles are internal pointers owned by the document and do not need disposal.
/// </summary>
public sealed class PdfBookmark
{
    private readonly IntPtr _handle;
    private readonly PdfDocument _document;

    internal PdfBookmark(PdfDocument document, IntPtr handle)
    {
        _document = document;
        _handle = handle;
    }

    /// <summary>
    /// Gets the title of the bookmark.
    /// </summary>
    public string Title
    {
        get => NativeStringHelper.ReadUtf16((buf, len) =>
            PdfiumNative.FPDFBookmark_GetTitle(_handle, buf, len));
    }

    /// <summary>
    /// Gets the zero-based destination page index, or -1 if no destination.
    /// </summary>
    public int DestinationPageIndex
    {
        get
        {
            var dest = PdfiumNative.FPDFBookmark_GetDest(_document.Handle, _handle);
            if (dest != IntPtr.Zero)
                return PdfiumNative.FPDFDest_GetDestPageIndex(_document.Handle, dest);

            // Try via action
            var action = PdfiumNative.FPDFBookmark_GetAction(_handle);
            if (action == IntPtr.Zero)
                return -1;

            var actionType = PdfiumNative.FPDFAction_GetType(action);
            if (actionType != 1) // PDFACTION_GOTO
                return -1;

            dest = PdfiumNative.FPDFAction_GetDest(_document.Handle, action);
            if (dest == IntPtr.Zero)
                return -1;

            return PdfiumNative.FPDFDest_GetDestPageIndex(_document.Handle, dest);
        }
    }

    /// <summary>
    /// Gets the child bookmarks of this bookmark.
    /// </summary>
    public IReadOnlyList<PdfBookmark> Children
    {
        get
        {
            var children = new List<PdfBookmark>();
            var child = PdfiumNative.FPDFBookmark_GetFirstChild(_document.Handle, _handle);
            while (child != IntPtr.Zero)
            {
                children.Add(new PdfBookmark(_document, child));
                child = PdfiumNative.FPDFBookmark_GetNextSibling(_document.Handle, child);
            }
            return children;
        }
    }
}
