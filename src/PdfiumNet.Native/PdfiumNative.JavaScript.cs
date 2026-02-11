using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_javascript.h
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_GetJavaScriptActionCount")]
    public static partial int FPDFDoc_GetJavaScriptActionCount(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_GetJavaScriptAction")]
    public static partial IntPtr FPDFDoc_GetJavaScriptAction(IntPtr document, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_CloseJavaScriptAction")]
    public static partial void FPDFDoc_CloseJavaScriptAction(IntPtr javascript);

    [LibraryImport(LibraryName, EntryPoint = "FPDFJavaScriptAction_GetName")]
    public static partial uint FPDFJavaScriptAction_GetName(IntPtr javascript, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFJavaScriptAction_GetScript")]
    public static partial uint FPDFJavaScriptAction_GetScript(IntPtr javascript, IntPtr buffer, uint buflen);
}
