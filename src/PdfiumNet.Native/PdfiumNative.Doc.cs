using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_doc.h — link and destination functions
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFLink_GetLinkAtPoint")]
    public static partial IntPtr FPDFLink_GetLinkAtPoint(IntPtr page, double x, double y);

    [LibraryImport(LibraryName, EntryPoint = "FPDFLink_GetDest")]
    public static partial IntPtr FPDFLink_GetDest(IntPtr document, IntPtr link);

    [LibraryImport(LibraryName, EntryPoint = "FPDFLink_GetAction")]
    public static partial IntPtr FPDFLink_GetAction(IntPtr link);

    [LibraryImport(LibraryName, EntryPoint = "FPDFLink_Enumerate")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFLink_Enumerate(IntPtr page, ref int startPos, out IntPtr linkAnnot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFLink_GetAnnotRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFLink_GetAnnotRect(IntPtr linkAnnot, out float left, out float top, out float right, out float bottom);

    [LibraryImport(LibraryName, EntryPoint = "FPDFLink_CountQuadPoints")]
    public static partial int FPDFLink_CountQuadPoints(IntPtr linkAnnot);
}
