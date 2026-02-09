using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_NewImageObj")]
    public static partial IntPtr FPDFPageObj_NewImageObj(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDFImageObj_SetBitmap")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFImageObj_SetBitmap(IntPtr pages, int count, IntPtr imageObject, IntPtr bitmap);

    [LibraryImport(LibraryName, EntryPoint = "FPDFImageObj_SetMatrix")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFImageObj_SetMatrix(IntPtr imageObject,
        double a, double b, double c, double d, double e, double f);

    [LibraryImport(LibraryName, EntryPoint = "FPDFImageObj_LoadJpegFile")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFImageObj_LoadJpegFile(IntPtr pages, int count,
        IntPtr imageObject, IntPtr fileAccess);

    [LibraryImport(LibraryName, EntryPoint = "FPDFImageObj_LoadJpegFileInline")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFImageObj_LoadJpegFileInline(IntPtr pages, int count,
        IntPtr imageObject, IntPtr fileAccess);

    [LibraryImport(LibraryName, EntryPoint = "FPDFImageObj_GetImageDataDecoded")]
    public static partial uint FPDFImageObj_GetImageDataDecoded(IntPtr imageObject, IntPtr buffer, uint bufLen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFImageObj_GetImageDataRaw")]
    public static partial uint FPDFImageObj_GetImageDataRaw(IntPtr imageObject, IntPtr buffer, uint bufLen);
}
