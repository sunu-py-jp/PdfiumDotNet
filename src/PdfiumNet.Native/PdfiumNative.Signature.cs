using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_signature.h
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetSignatureCount")]
    public static partial int FPDF_GetSignatureCount(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetSignatureObject")]
    public static partial IntPtr FPDF_GetSignatureObject(IntPtr document, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFSignatureObj_GetContents")]
    public static partial uint FPDFSignatureObj_GetContents(IntPtr signature, IntPtr buffer, uint length);

    [LibraryImport(LibraryName, EntryPoint = "FPDFSignatureObj_GetByteRange")]
    public static partial uint FPDFSignatureObj_GetByteRange(IntPtr signature, IntPtr buffer, uint length);

    [LibraryImport(LibraryName, EntryPoint = "FPDFSignatureObj_GetSubFilter")]
    public static partial uint FPDFSignatureObj_GetSubFilter(IntPtr signature, IntPtr buffer, uint length);

    [LibraryImport(LibraryName, EntryPoint = "FPDFSignatureObj_GetReason")]
    public static partial uint FPDFSignatureObj_GetReason(IntPtr signature, IntPtr buffer, uint length);

    [LibraryImport(LibraryName, EntryPoint = "FPDFSignatureObj_GetTime")]
    public static partial uint FPDFSignatureObj_GetTime(IntPtr signature, IntPtr buffer, uint length);

    [LibraryImport(LibraryName, EntryPoint = "FPDFSignatureObj_GetDocMDPPermission")]
    public static partial uint FPDFSignatureObj_GetDocMDPPermission(IntPtr signature);
}
