using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

/// <summary>
/// Represents the FS_RECTF structure used by PDFium for annotation rectangles.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FsRectF
{
    public float Left;
    public float Bottom;
    public float Right;
    public float Top;

    public FsRectF(float left, float bottom, float right, float top)
    {
        Left = left;
        Bottom = bottom;
        Right = right;
        Top = top;
    }
}
