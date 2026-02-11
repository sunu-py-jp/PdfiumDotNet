using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_thumbnail.h
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetDecodedThumbnailData")]
    public static partial uint FPDFPage_GetDecodedThumbnailData(IntPtr page, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetRawThumbnailData")]
    public static partial uint FPDFPage_GetRawThumbnailData(IntPtr page, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetThumbnailAsBitmap")]
    public static partial IntPtr FPDFPage_GetThumbnailAsBitmap(IntPtr page);
}
