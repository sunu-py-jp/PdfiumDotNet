using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_structtree.h
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructTree_GetForPage")]
    public static partial IntPtr FPDF_StructTree_GetForPage(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructTree_Close")]
    public static partial void FPDF_StructTree_Close(IntPtr structTree);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructTree_CountChildren")]
    public static partial int FPDF_StructTree_CountChildren(IntPtr structTree);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructTree_GetChildAtIndex")]
    public static partial IntPtr FPDF_StructTree_GetChildAtIndex(IntPtr structTree, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_GetType")]
    public static partial uint FPDF_StructElement_GetType(IntPtr structElement, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_GetTitle")]
    public static partial uint FPDF_StructElement_GetTitle(IntPtr structElement, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_GetAltText")]
    public static partial uint FPDF_StructElement_GetAltText(IntPtr structElement, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_GetActualText")]
    public static partial uint FPDF_StructElement_GetActualText(IntPtr structElement, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_GetLang")]
    public static partial uint FPDF_StructElement_GetLang(IntPtr structElement, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_CountChildren")]
    public static partial int FPDF_StructElement_CountChildren(IntPtr structElement);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_StructElement_GetChildAtIndex")]
    public static partial IntPtr FPDF_StructElement_GetChildAtIndex(IntPtr structElement, int index);
}
