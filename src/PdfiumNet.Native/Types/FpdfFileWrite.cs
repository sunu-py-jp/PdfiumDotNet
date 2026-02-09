using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

/// <summary>
/// Native struct matching FPDF_FILEWRITE. Must be pinned during save operations.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FpdfFileWrite
{
    public int Version;
    public IntPtr WriteBlock;
}
