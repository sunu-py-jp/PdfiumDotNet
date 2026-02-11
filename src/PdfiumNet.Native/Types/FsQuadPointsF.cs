using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

/// <summary>
/// Represents the FS_QUADPOINTSF structure used by PDFium for text markup annotation attachment points.
/// Four points defining a quadrilateral: (x1,y1)=bottom-left, (x2,y2)=bottom-right,
/// (x3,y3)=top-left, (x4,y4)=top-right.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FsQuadPointsF
{
    public float X1, Y1;
    public float X2, Y2;
    public float X3, Y3;
    public float X4, Y4;

    public FsQuadPointsF(float x1, float y1, float x2, float y2,
                          float x3, float y3, float x4, float y4)
    {
        X1 = x1; Y1 = y1;
        X2 = x2; Y2 = y2;
        X3 = x3; Y3 = y3;
        X4 = x4; Y4 = y4;
    }
}
