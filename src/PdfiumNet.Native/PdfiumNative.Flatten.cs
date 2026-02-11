using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

// fpdf_flatten.h
public static partial class PdfiumNative
{
    /// <summary>Flatten operation succeeded; the page was modified.</summary>
    public const int FLATTEN_SUCCESS = 1;
    /// <summary>Nothing to flatten on the page.</summary>
    public const int FLATTEN_NOTHINGTODO = 2;

    /// <summary>Flatten for display (normal viewing).</summary>
    public const int FLAT_NORMALDISPLAY = 0;
    /// <summary>Flatten for print.</summary>
    public const int FLAT_PRINT = 1;

    /// <summary>
    /// Flatten annotations and form fields into the page content.
    /// </summary>
    /// <param name="page">Handle to the page.</param>
    /// <param name="usageFlag">FLAT_NORMALDISPLAY or FLAT_PRINT.</param>
    /// <returns>FLATTEN_SUCCESS, FLATTEN_NOTHINGTODO, or 0 on failure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "FPDFPage_Flatten")]
    public static partial int FPDFPage_Flatten(IntPtr page, int usageFlag);
}
