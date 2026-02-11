using System.Runtime.InteropServices;
using PdfiumNet.Native.Types;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // --- Annotation page-level ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_CreateAnnot")]
    public static partial IntPtr FPDFPage_CreateAnnot(IntPtr page, int subtype);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetAnnotCount")]
    public static partial int FPDFPage_GetAnnotCount(IntPtr page);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_GetAnnot")]
    public static partial IntPtr FPDFPage_GetAnnot(IntPtr page, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_CloseAnnot")]
    public static partial void FPDFPage_CloseAnnot(IntPtr annot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_RemoveAnnot")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPage_RemoveAnnot(IntPtr page, int index);

    // --- Annotation properties ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetSubtype")]
    public static partial int FPDFAnnot_GetSubtype(IntPtr annot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetColor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetColor(IntPtr annot, int type,
        uint r, uint g, uint b, uint a);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetColor")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_GetColor(IntPtr annot, int type,
        out uint r, out uint g, out uint b, out uint a);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetRect(IntPtr annot, ref FsRectF rect);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_GetRect(IntPtr annot, out FsRectF rect);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetFlags")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetFlags(IntPtr annot, int flags);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetFlags")]
    public static partial int FPDFAnnot_GetFlags(IntPtr annot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetStringValue", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetStringValue(IntPtr annot, string key, string value);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetStringValue", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint FPDFAnnot_GetStringValue(IntPtr annot, string key, IntPtr buffer, uint buflen);

    // --- Attachment points (for text markup annotations) ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_HasAttachmentPoints")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_HasAttachmentPoints(IntPtr annot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetAttachmentPoints")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetAttachmentPoints(IntPtr annot, nint quadIndex, ref FsQuadPointsF quadPoints);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_AppendAttachmentPoints")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_AppendAttachmentPoints(IntPtr annot, ref FsQuadPointsF quadPoints);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_CountAttachmentPoints")]
    public static partial nint FPDFAnnot_CountAttachmentPoints(IntPtr annot);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetAttachmentPoints")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_GetAttachmentPoints(IntPtr annot, nint quadIndex, out FsQuadPointsF quadPoints);

    // --- Ink ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_AddInkStroke")]
    public static partial int FPDFAnnot_AddInkStroke(IntPtr annot, IntPtr points, nint pointCount);

    // --- Border ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetBorder")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetBorder(IntPtr annot, float horizontalRadius, float verticalRadius, float borderWidth);

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_GetBorder")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_GetBorder(IntPtr annot, out float horizontalRadius, out float verticalRadius, out float borderWidth);

    // --- Link ---

    [LibraryImport(LibraryName, EntryPoint = "FPDFAnnot_SetURI", StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFAnnot_SetURI(IntPtr annot, string uri);
}
