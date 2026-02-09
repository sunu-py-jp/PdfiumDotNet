using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_CreateNewPath")]
    public static partial IntPtr FPDFPageObj_CreateNewPath(float x, float y);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPageObj_CreateNewRect")]
    public static partial IntPtr FPDFPageObj_CreateNewRect(float x, float y, float w, float h);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_SetDrawMode")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPath_SetDrawMode(IntPtr path, int fillMode, [MarshalAs(UnmanagedType.Bool)] bool stroke);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_MoveTo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPath_MoveTo(IntPtr path, float x, float y);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_LineTo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPath_LineTo(IntPtr path, float x, float y);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_BezierTo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPath_BezierTo(IntPtr path, float x1, float y1, float x2, float y2, float x3, float y3);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_Close")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPath_Close(IntPtr path);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_CountSegments")]
    public static partial int FPDFPath_CountSegments(IntPtr path);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPath_GetPathSegment")]
    public static partial IntPtr FPDFPath_GetPathSegment(IntPtr path, int index);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPathSegment_GetPoint")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPathSegment_GetPoint(IntPtr segment, out float x, out float y);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPathSegment_GetType")]
    public static partial int FPDFPathSegment_GetType(IntPtr segment);

    [LibraryImport(LibraryName, EntryPoint = "FPDFPathSegment_GetClose")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDFPathSegment_GetClose(IntPtr segment);
}
