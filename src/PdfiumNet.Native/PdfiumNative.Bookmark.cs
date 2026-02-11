using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // --- Bookmark ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFBookmark_GetFirstChild")]
    public static partial IntPtr FPDFBookmark_GetFirstChild(IntPtr document, IntPtr bookmark);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBookmark_GetNextSibling")]
    public static partial IntPtr FPDFBookmark_GetNextSibling(IntPtr document, IntPtr bookmark);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBookmark_GetTitle")]
    public static partial uint FPDFBookmark_GetTitle(IntPtr bookmark, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBookmark_Find", StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr FPDFBookmark_Find(IntPtr document, string title);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBookmark_GetDest")]
    public static partial IntPtr FPDFBookmark_GetDest(IntPtr document, IntPtr bookmark);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBookmark_GetAction")]
    public static partial IntPtr FPDFBookmark_GetAction(IntPtr bookmark);

    // --- Destination ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFDest_GetDestPageIndex")]
    public static partial int FPDFDest_GetDestPageIndex(IntPtr document, IntPtr dest);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDest_GetLocationInPage")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFDest_GetLocationInPage(IntPtr dest,
        [MarshalAs(UnmanagedType.Bool)] out bool hasXVal,
        [MarshalAs(UnmanagedType.Bool)] out bool hasYVal,
        [MarshalAs(UnmanagedType.Bool)] out bool hasZoomVal,
        out float x, out float y, out float zoom);

    // --- Action ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAction_GetType")]
    public static partial uint FPDFAction_GetType(IntPtr action);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAction_GetDest")]
    public static partial IntPtr FPDFAction_GetDest(IntPtr document, IntPtr action);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAction_GetURIPath")]
    public static partial uint FPDFAction_GetURIPath(IntPtr document, IntPtr action, IntPtr buffer, uint buflen);
}
