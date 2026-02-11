using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

/// <summary>
/// Provides helper methods for reading strings from PDFium's native API.
/// </summary>
internal static class NativeStringHelper
{
    /// <summary>
    /// Reads a UTF-16LE string using the standard PDFium two-call pattern:
    /// first call with null buffer to get size, second call to fill buffer.
    /// </summary>
    /// <param name="invoke">
    /// A delegate that calls the native function.
    /// Parameters: (IntPtr buffer, uint bufferLength) → returns byte count including null terminator.
    /// </param>
    internal static string ReadUtf16(Func<IntPtr, uint, uint> invoke)
    {
        var size = invoke(IntPtr.Zero, 0);
        if (size <= 2) // 2 bytes = just the null terminator in UTF-16
            return string.Empty;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            invoke(buffer, size);
            var charCount = (int)(size / 2) - 1;
            return charCount > 0
                ? Marshal.PtrToStringUni(buffer, charCount) ?? string.Empty
                : string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
