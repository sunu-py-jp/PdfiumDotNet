using System.Reflection;
using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

/// <summary>
/// Handles platform-specific loading of the PDFium native library.
/// Uses .NET's built-in NativeLibrary resolver with fallback paths.
/// </summary>
public static class NativeLibraryLoader
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;
        _registered = true;

        NativeLibrary.SetDllImportResolver(typeof(PdfiumNative).Assembly, ResolvePdfium);
    }

    private static IntPtr ResolvePdfium(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != PdfiumNative.LibraryName)
            return IntPtr.Zero;

        // Try default resolution first (runtimes/{rid}/native/)
        if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out var handle))
            return handle;

        // Try platform-specific names
        var candidates = GetPlatformSpecificNames();
        foreach (var candidate in candidates)
        {
            if (NativeLibrary.TryLoad(candidate, assembly, searchPath, out handle))
                return handle;

            // Try relative to assembly location
            var assemblyDir = Path.GetDirectoryName(assembly.Location);
            if (assemblyDir != null)
            {
                var fullPath = Path.Combine(assemblyDir, candidate);
                if (NativeLibrary.TryLoad(fullPath, out handle))
                    return handle;
            }
        }

        return IntPtr.Zero;
    }

    private static string[] GetPlatformSpecificNames()
    {
        if (OperatingSystem.IsWindows())
            return new[] { "pdfium.dll" };
        if (OperatingSystem.IsMacOS())
            return new[] { "libpdfium.dylib" };
        if (OperatingSystem.IsLinux())
            return new[] { "libpdfium.so" };
        return Array.Empty<string>();
    }
}
