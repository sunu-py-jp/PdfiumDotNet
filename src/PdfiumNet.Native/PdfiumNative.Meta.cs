using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    /// <summary>
    /// Get a document meta data tag.
    /// Returns the number of bytes in the tag (including trailing NUL).
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "FPDF_GetMetaText", StringMarshalling = StringMarshalling.Utf8)]
    public static partial uint FPDF_GetMetaText(IntPtr document, string tag, IntPtr buffer, uint buflen);
}
