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

                // Try runtimes/{rid}/native/ subdirectory
                foreach (var rid in GetRuntimeIdentifiers())
                {
                    var runtimePath = Path.Combine(assemblyDir, "runtimes", rid, "native", candidate);
                    if (NativeLibrary.TryLoad(runtimePath, out handle))
                        return handle;
                }
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

    private static string[] GetRuntimeIdentifiers()
    {
        var arch = RuntimeInformation.OSArchitecture switch
        {
            Architecture.Arm64 => "arm64",
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            _ => "x64"
        };

        if (OperatingSystem.IsWindows())
            return new[] { $"win-{arch}" };
        if (OperatingSystem.IsMacOS())
            return new[] { $"osx-{arch}" };
        if (OperatingSystem.IsLinux())
            return new[] { $"linux-{arch}" };
        return Array.Empty<string>();
    }
}
