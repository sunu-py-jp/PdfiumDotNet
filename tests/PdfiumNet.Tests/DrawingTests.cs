using PdfiumNet.Drawing;
using Xunit;

namespace PdfiumNet.Tests;

public class DrawingTests
{
    [Fact]
    public void PdfColor_Predefined()
    {
        Assert.Equal((byte)0, PdfColor.Black.R);
        Assert.Equal((byte)0, PdfColor.Black.G);
        Assert.Equal((byte)0, PdfColor.Black.B);
        Assert.Equal((byte)255, PdfColor.Black.A);

        Assert.Equal((byte)255, PdfColor.White.R);
        Assert.Equal((byte)255, PdfColor.White.G);
        Assert.Equal((byte)255, PdfColor.White.B);
    }

    [Fact]
    public void PdfColor_ToArgb()
    {
        var red = PdfColor.Red;
        Assert.Equal(0xFFFF0000u, red.ToArgb());
    }

    [Fact]
    public void PdfColor_FromArgb()
    {
        var color = PdfColor.FromArgb(128, 255, 0, 0);
        Assert.Equal((byte)255, color.R);
        Assert.Equal((byte)0, color.G);
        Assert.Equal((byte)0, color.B);
        Assert.Equal((byte)128, color.A);
    }

    [Fact]
    public void DrawMode_Flags()
    {
        var mode = DrawMode.FillAndStroke;
        Assert.True(mode.HasFlag(DrawMode.Fill));
        Assert.True(mode.HasFlag(DrawMode.Stroke));
        Assert.False(mode.HasFlag(DrawMode.FillEvenOdd));
    }
}
