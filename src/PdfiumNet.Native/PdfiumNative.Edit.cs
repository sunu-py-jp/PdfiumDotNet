using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // --- Page Object common ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_GetType")]
    public static partial int FPDFPageObj_GetType(IntPtr pageObject);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_Destroy")]
    public static partial void FPDFPageObj_Destroy(IntPtr pageObject);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_HasTransparency")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_HasTransparency(IntPtr pageObject);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_GetBounds")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_GetBounds(IntPtr pageObject,
        out float left, out float bottom, out float right, out float top);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_SetFillColor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_SetFillColor(IntPtr pageObject,
        uint r, uint g, uint b, uint a);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_GetFillColor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_GetFillColor(IntPtr pageObject,
        out uint r, out uint g, out uint b, out uint a);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_SetStrokeColor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_SetStrokeColor(IntPtr pageObject,
        uint r, uint g, uint b, uint a);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_GetStrokeColor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_GetStrokeColor(IntPtr pageObject,
        out uint r, out uint g, out uint b, out uint a);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_SetStrokeWidth")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_SetStrokeWidth(IntPtr pageObject, float width);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_GetStrokeWidth")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPageObj_GetStrokeWidth(IntPtr pageObject, out float width);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_Transform")]
    public static partial void FPDFPageObj_Transform(IntPtr pageObject,
        double a, double b, double c, double d, double e, double f);

    // --- Text Object ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_NewTextObj", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr FPDFPageObj_NewTextObj(IntPtr document, string font, float fontSize);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_SetText", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFText_SetText(IntPtr textObject, string text);

    [LibraryImport(LibraryName, EntryPoint = "FPDFTextObj_GetFontSize")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFTextObj_GetFontSize(IntPtr textObject, out float fontSize);

    [LibraryImport(LibraryName, EntryPoint = "FPDFTextObj_GetText")]
    public static partial uint FPDFTextObj_GetText(
        IntPtr textObject, IntPtr textPage, IntPtr buffer, uint length);

    // --- Font ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_LoadFont")]
    public static partial IntPtr FPDFText_LoadFont(IntPtr document, IntPtr data, uint size,
        int fontType, [MarshalAs(UnmanagedType.Bool)] bool cid);

    [LibraryImport(LibraryName, EntryPoint = "FPDFText_LoadStandardFont", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr FPDFText_LoadStandardFont(IntPtr document, string font);

    [LibraryImport(LibraryName, EntryPoint = "FPDFFont_Close")]
    public static partial void FPDFFont_Close(IntPtr font);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_CreateTextObj")]
    public static partial IntPtr FPDFPageObj_CreateTextObj(IntPtr document, IntPtr font, float fontSize);
}
