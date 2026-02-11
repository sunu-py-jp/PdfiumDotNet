using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_transformpage.h
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_SetMediaBox")]
    public static partial void FPDFPage_SetMediaBox(IntPtr page, float left, float bottom, float right, float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_SetCropBox")]
    public static partial void FPDFPage_SetCropBox(IntPtr page, float left, float bottom, float right, float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetMediaBox")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_GetMediaBox(IntPtr page, out float left, out float bottom, out float right, out float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetCropBox")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_GetCropBox(IntPtr page, out float left, out float bottom, out float right, out float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_SetTrimBox")]
    public static partial void FPDFPage_SetTrimBox(IntPtr page, float left, float bottom, float right, float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetTrimBox")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_GetTrimBox(IntPtr page, out float left, out float bottom, out float right, out float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_SetArtBox")]
    public static partial void FPDFPage_SetArtBox(IntPtr page, float left, float bottom, float right, float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetArtBox")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_GetArtBox(IntPtr page, out float left, out float bottom, out float right, out float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_SetBleedBox")]
    public static partial void FPDFPage_SetBleedBox(IntPtr page, float left, float bottom, float right, float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetBleedBox")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_GetBleedBox(IntPtr page, out float left, out float bottom, out float right, out float top);
}
