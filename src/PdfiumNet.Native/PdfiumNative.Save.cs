using System.Runtime.InteropServices;
using PdfiumNet.Native.Types;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // Save flags
    public const uint FPDF_INCREMENTAL = 1;
    public const uint FPDF_NO_INCREMENTAL = 2;
    public const uint FPDF_REMOVE_SECURITY = 3;

    [LibraryImport(LibraryName, EntryPoint = "FPDF_SaveAsCopy")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDF_SaveAsCopy(IntPtr document, ref FpdfFileWrite fileWrite, uint flags);

    [LibraryImport(LibraryName, EntryPoint = "FPDF_SaveWithVersion")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FPDF_SaveWithVersion(IntPtr document, ref FpdfFileWrite fileWrite, uint flags, int fileVersion);
}
