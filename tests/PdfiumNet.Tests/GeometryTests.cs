using PdfiumNet.Geometry;
using Xunit;

namespace PdfiumNet.Tests;

public class GeometryTests
{
    [Fact]
    public void PdfPoint_Addition()
    {
        var a = new PdfPoint(1, 2);
        var b = new PdfPoint(3, 4);
        var result = a + b;
        Assert.Equal(4, result.X);
        Assert.Equal(6, result.Y);
    }

    [Fact]
    public void PdfPoint_Subtraction()
    {
        var a = new PdfPoint(5, 7);
        var b = new PdfPoint(2, 3);
        var result = a - b;
        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
    }

    [Fact]
    public void PdfSize_StandardSizes()
    {
        Assert.Equal(612, PdfSize.Letter.Width);
        Assert.Equal(792, PdfSize.Letter.Height);
        Assert.True(PdfSize.A4.Width > 595 && PdfSize.A4.Width < 596);
    }

    [Fact]
    public void PdfRectangle_Properties()
    {
        var rect = new PdfRectangle(10, 20, 110, 120);
        Assert.Equal(100, rect.Width);
        Assert.Equal(100, rect.Height);
        Assert.Equal(60, rect.Center.X);
        Assert.Equal(70, rect.Center.Y);
    }

    [Fact]
    public void PdfRectangle_Contains()
    {
        var rect = new PdfRectangle(0, 0, 100, 100);
        Assert.True(rect.Contains(new PdfPoint(50, 50)));
        Assert.False(rect.Contains(new PdfPoint(150, 50)));
    }

    [Fact]
    public void PdfMatrix_Identity()
    {
        var point = new PdfPoint(5, 10);
        var result = PdfMatrix.Identity.Transform(point);
        Assert.Equal(5, result.X);
        Assert.Equal(10, result.Y);
    }

    [Fact]
    public void PdfMatrix_Translation()
    {
        var matrix = PdfMatrix.CreateTranslation(10, 20);
        var result = matrix.Transform(new PdfPoint(5, 5));
        Assert.Equal(15, result.X);
        Assert.Equal(25, result.Y);
    }

    [Fact]
    public void PdfMatrix_Scale()
    {
        var matrix = PdfMatrix.CreateScale(2, 3);
        var result = matrix.Transform(new PdfPoint(5, 10));
        Assert.Equal(10, result.X);
        Assert.Equal(30, result.Y);
    }

    [Fact]
    public void PdfMatrix_Multiply()
    {
        var translate = PdfMatrix.CreateTranslation(10, 0);
        var scale = PdfMatrix.CreateScale(2, 2);
        var combined = scale.Multiply(translate);
        var result = combined.Transform(new PdfPoint(5, 5));
        Assert.Equal(20, result.X);
        Assert.Equal(10, result.Y);
    }
}
