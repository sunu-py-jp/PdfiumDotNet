using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDF_LoadPage")]
    public static partial IntPtr FPDF_LoadPage(IntPtr document, int pageIndex);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_ClosePage")]
    public static partial void FPDF_ClosePage(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetPageWidthF")]
    public static partial float FPDF_GetPageWidthF(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetPageHeightF")]
    public static partial float FPDF_GetPageHeightF(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_New")]
    public static partial IntPtr FPDFPage_New(IntPtr document, int pageIndex, double width, double height);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_Delete")]
    public static partial void FPDFPage_Delete(IntPtr document, int pageIndex);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_SetRotation")]
    public static partial void FPDFPage_SetRotation(IntPtr page, int rotate);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetRotation")]
    public static partial int FPDFPage_GetRotation(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_CountObjects")]
    public static partial int FPDFPage_CountObjects(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetObject")]
    public static partial IntPtr FPDFPage_GetObject(IntPtr page, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_InsertObject")]
    public static partial void FPDFPage_InsertObject(IntPtr page, IntPtr pageObject);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_RemoveObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_RemoveObject(IntPtr page, IntPtr pageObject);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GenerateContent")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_GenerateContent(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_ImportPages")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDF_ImportPages(IntPtr destDoc, IntPtr srcDoc, [MarshalAs(UnmanagedType.LPStr)] string? pageRange, int insertIndex);
}
