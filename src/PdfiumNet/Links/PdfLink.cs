using System.Runtime.InteropServices;
using PdfiumNet.Geometry;
using PdfiumNet.Native;

namespace PdfiumNet.Links;

/// <summary>
/// Represents a link on a PDF page.
/// </summary>
public sealed class PdfLink
{
    private readonly IntPtr _linkHandle;
    private readonly PdfDocument _document;

    internal PdfLink(PdfDocument document, IntPtr linkHandle, PdfRectangle rect)
    {
        _document = document;
        _linkHandle = linkHandle;
        Rect = rect;
    }

    /// <summary>
    /// Gets the rectangle of the link area on the page.
    /// </summary>
    public PdfRectangle Rect { get; }

    /// <summary>
    /// Gets the destination page index, or -1 if the link does not point to an internal page.
    /// </summary>
    public int DestinationPageIndex
    {
        get
        {
            var dest = PdfiumNative.FPDFLink_GetDest(_document.Handle, _linkHandle);
            if (dest != IntPtr.Zero)
                return PdfiumNative.FPDFDest_GetDestPageIndex(_document.Handle, dest);

            var action = PdfiumNative.FPDFLink_GetAction(_linkHandle);
            if (action == IntPtr.Zero)
                return -1;

            var actionType = PdfiumNative.FPDFAction_GetType(action);
            if (actionType != 1) // PDFACTION_GOTO
                return -1;

            dest = PdfiumNative.FPDFAction_GetDest(_document.Handle, action);
            return dest != IntPtr.Zero
                ? PdfiumNative.FPDFDest_GetDestPageIndex(_document.Handle, dest)
                : -1;
        }
    }

    /// <summary>
    /// Gets the URI if this is an external link, or null if it is not a URI action.
    /// </summary>
    public string? Uri
    {
        get
        {
            var action = PdfiumNative.FPDFLink_GetAction(_linkHandle);
            if (action == IntPtr.Zero)
                return null;

            var actionType = PdfiumNative.FPDFAction_GetType(action);
            if (actionType != 3) // PDFACTION_URI
                return null;

            var size = PdfiumNative.FPDFAction_GetURIPath(_document.Handle, action, IntPtr.Zero, 0);
            if (size == 0)
                return null;

            var buffer = Marshal.AllocHGlobal((int)size);
            try
            {
                PdfiumNative.FPDFAction_GetURIPath(_document.Handle, action, buffer, size);
                return Marshal.PtrToStringUTF8(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
