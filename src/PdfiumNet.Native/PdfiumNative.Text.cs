using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFText_LoadPage")]
    public static partial IntPtr FPDFText_LoadPage(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_ClosePage")]
    public static partial void FPDFText_ClosePage(IntPtr textPage);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_CountChars")]
    public static partial int FPDFText_CountChars(IntPtr textPage);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetUnicode")]
    public static partial uint FPDFText_GetUnicode(IntPtr textPage, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetFontSize")]
    public static partial double FPDFText_GetFontSize(IntPtr textPage, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetCharBox")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFText_GetCharBox(IntPtr textPage, int index,
        out double left, out double right, out double bottom, out double top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetCharOrigin")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFText_GetCharOrigin(IntPtr textPage, int index,
        out double x, out double y);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetText")]
    public static partial int FPDFText_GetText(IntPtr textPage, int startIndex, int count, IntPtr result);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_FindStart", StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr FPDFText_FindStart(IntPtr textPage, string findWhat, uint flags, int startIndex);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_FindNext")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFText_FindNext(IntPtr search);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_FindClose")]
    public static partial void FPDFText_FindClose(IntPtr search);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetSchResultIndex")]
    public static partial int FPDFText_GetSchResultIndex(IntPtr search);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetSchCount")]
    public static partial int FPDFText_GetSchCount(IntPtr search);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetFontInfo")]
    public static partial uint FPDFText_GetFontInfo(IntPtr textPage, int index,
        IntPtr buffer, uint bufLen, out int flags);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_CountRects")]
    public static partial int FPDFText_CountRects(IntPtr textPage, int startIndex, int count);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFText_GetRect(IntPtr textPage, int rectIndex,
        out double left, out double top, out double right, out double bottom);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetBoundedText")]
    public static partial int FPDFText_GetBoundedText(IntPtr textPage,
        double left, double top, double right, double bottom,
        IntPtr buffer, int buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_GetCharIndexAtPos")]
    public static partial int FPDFText_GetCharIndexAtPos(IntPtr textPage,
        double x, double y, double xTolerance, double yTolerance);
}
