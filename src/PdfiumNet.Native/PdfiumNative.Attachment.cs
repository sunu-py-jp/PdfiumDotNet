using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_attachment.h
public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_GetAttachmentCount")]
    public static partial int FPDFDoc_GetAttachmentCount(IntPtr document);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_AddAttachment", StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr FPDFDoc_AddAttachment(IntPtr document, string name);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_GetAttachment")]
    public static partial IntPtr FPDFDoc_GetAttachment(IntPtr document, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFDoc_DeleteAttachment")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFDoc_DeleteAttachment(IntPtr document, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_GetName")]
    public static partial uint FPDFAttachment_GetName(IntPtr attachment, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_HasKey", StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAttachment_HasKey(IntPtr attachment, string key);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_GetValueType", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int FPDFAttachment_GetValueType(IntPtr attachment, string key);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_SetStringValue", StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAttachment_SetStringValue(IntPtr attachment,
        string key, [MarshalAs(UnmanagedType.LPWStr)] string value);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_GetStringValue", StringMarshalling = StringMarshalling.Utf8)]
    public static partial uint FPDFAttachment_GetStringValue(IntPtr attachment,
        string key, IntPtr buffer, uint buflen);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_SetFile")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAttachment_SetFile(IntPtr attachment, IntPtr document,
        IntPtr contents, uint len);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAttachment_GetFile")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAttachment_GetFile(IntPtr attachment,
        IntPtr buffer, uint buflen, out uint outBuflen);
}
