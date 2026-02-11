using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // --- Form type ---

    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetFormType")]
    public static partial int FPDF_GetFormType(IntPtr document);

    // --- Form fill environment ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFDOC_InitFormFillEnvironment")]
    public static partial IntPtr FPDFDOC_InitFormFillEnvironment(IntPtr document, IntPtr formInfo);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDOC_ExitFormFillEnvironment")]
    public static partial void FPDFDOC_ExitFormFillEnvironment(IntPtr formHandle);

    // --- Form field properties (via annotation handle) ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetFormFieldType")]
    public static partial int FPDFAnnot_GetFormFieldType(IntPtr formHandle, IntPtr annot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetFormFieldName", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint FPDFAnnot_GetFormFieldName(IntPtr formHandle, IntPtr annot, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetFormFieldValue", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint FPDFAnnot_GetFormFieldValue(IntPtr formHandle, IntPtr annot, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetFormFieldFlags")]
    public static partial int FPDFAnnot_GetFormFieldFlags(IntPtr formHandle, IntPtr annot);

    // --- Form field modification ---

    [LibraryImport(LibraryName, EntryPoint = "FORM_SetFieldText", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FORM_SetFieldText(IntPtr formHandle, IntPtr page, IntPtr annot, string value);

    [LibraryImport(LibraryName, EntryPoint = "FORM_SetIndexSelected")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FORM_SetIndexSelected(IntPtr formHandle, IntPtr page, int index, [MarshalAs(UnmanagedType.Bool)] bool selected);

    [LibraryImport(LibraryName, EntryPoint = "FORM_IsIndexSelected")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FORM_IsIndexSelected(IntPtr formHandle, IntPtr page, int index);
}
