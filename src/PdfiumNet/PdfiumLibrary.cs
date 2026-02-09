using PdfiumNet.Native;

namespace PdfiumNet;

/// <summary>
/// Manages PDFium library initialization and cleanup.
/// Must be initialized before using any PDFium functionality.
/// Thread safety: PDFium is not thread-safe for concurrent access to the same document.
/// </summary>
public static class PdfiumLibrary
{
    private static int _initCount;
    private static readonly object _lock = new();

    /// <summary>
    /// Initializes the PDFium library. Can be called multiple times safely.
    /// </summary>
    public static void Initialize()
    {
        lock (_lock)
        {
            if (_initCount == 0)
            {
                NativeLibraryLoader.Register();
                PdfiumNative.FPDF_InitLibrary();
            }
            _initCount++;
        }
    }

    /// <summary>
    /// Ensures the library is initialized, calling Initialize() if needed.
    /// </summary>
    internal static void EnsureInitialized()
    {
        if (_initCount == 0)
            Initialize();
    }

    /// <summary>
    /// Releases the PDFium library. Should be called when done using PDFium.
    /// </summary>
    public static void Destroy()
    {
        lock (_lock)
        {
            _initCount--;
            if (_initCount <= 0)
            {
                PdfiumNative.FPDF_DestroyLibrary();
                _initCount = 0;
            }
        }
    }
}
