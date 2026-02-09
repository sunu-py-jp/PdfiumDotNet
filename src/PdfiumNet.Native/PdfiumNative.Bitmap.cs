using System.Runtime.InteropServices;

namespace PdfiumNet.Native;

public static partial class PdfiumNative
{
    // Bitmap formats
    public const int FPDFBitmap_Unknown = 0;
    public const int FPDFBitmap_Gray = 1;
    public const int FPDFBitmap_BGR = 2;
    public const int FPDFBitmap_BGRx = 3;
    public const int FPDFBitmap_BGRA = 4;

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_Create")]
    public static partial IntPtr FPDFBitmap_Create(int width, int height, int alpha);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_CreateEx")]
    public static partial IntPtr FPDFBitmap_CreateEx(int width, int height, int format,
        IntPtr firstScan, int stride);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_FillRect")]
    public static partial void FPDFBitmap_FillRect(IntPtr bitmap, int left, int top,
        int width, int height, uint color);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_GetBuffer")]
    public static partial IntPtr FPDFBitmap_GetBuffer(IntPtr bitmap);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_GetWidth")]
    public static partial int FPDFBitmap_GetWidth(IntPtr bitmap);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_GetHeight")]
    public static partial int FPDFBitmap_GetHeight(IntPtr bitmap);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_GetStride")]
    public static partial int FPDFBitmap_GetStride(IntPtr bitmap);

    [LibraryImport(LibraryName, EntryPoint = "FPDFBitmap_Destroy")]
    public static partial void FPDFBitmap_Destroy(IntPtr bitmap);

    // Rendering
    [LibraryImport(LibraryName, EntryPoint = "FPDF_RenderPageBitmap")]
    public static partial void FPDF_RenderPageBitmap(IntPtr bitmap, IntPtr page,
        int startX, int startY, int sizeX, int sizeY, int rotate, int flags);

    // Render flags
    public const int FPDF_ANNOT = 0x01;
    public const int FPDF_LCD_TEXT = 0x02;
    public const int FPDF_NO_NATIVETEXT = 0x04;
    public const int FPDF_GRAYSCALE = 0x08;
    public const int FPDF_PRINTING = 0x800;
}
