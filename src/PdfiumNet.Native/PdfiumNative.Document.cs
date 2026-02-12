using System.Runtime.InteropServices;
using PdfiumNet.Native.Types;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // --- Library lifecycle ---

    [LibraryImport(LibraryName, EntryPoint = "FPDF_InitLibrary")]
    public static partial void FPDF_InitLibrary();

    [LibraryImport(LibraryName, EntryPoint = "FPDF_DestroyLibrary")]
    public static partial void FPDF_DestroyLibrary();

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetLastError")]
    public static partial uint FPDF_GetLastError();

    // --- Document ---

    [LibraryImport(LibraryName, EntryPoint = "FPDF_CreateNewDocument")]
    public static partial IntPtr FPDF_CreateNewDocument();

    [LibraryImport(LibraryName, EntryPoint = "FPDF_LoadDocument", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr FPDF_LoadDocument(string filePath, string? password);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_LoadMemDocument")]
    public static partial IntPtr FPDF_LoadMemDocument(IntPtr dataBuffer, int size, [MarshalAs(UnmanagedType.LPStr)] string? password);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_CloseDocument")]
    public static partial void FPDF_CloseDocument(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetPageCount")]
    public static partial int FPDF_GetPageCount(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetFileVersion")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDF_GetFileVersion(IntPtr document, out int fileVersion);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetDocPermissions")]
    public static partial uint FPDF_GetDocPermissions(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetSecurityHandlerRevision")]
    public static partial int FPDF_GetSecurityHandlerRevision(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetPageLabel")]
    public static partial uint FPDF_GetPageLabel(IntPtr document, int pageIndex, IntPtr buffer, uint buflen);
}
