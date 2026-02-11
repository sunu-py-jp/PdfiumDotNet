using PdfiumNet.Annotations;
using PdfiumNet.Drawing;
using PdfiumNet.Geometry;
using PdfiumNet.Native.Types;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class AnnotationTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(AnnotationTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public AnnotationTests(ITestOutputHelper output)
    {
        _output = output;
        Directory.CreateDirectory(OutputDir);
    }

    private static bool IsPdfiumAvailable()
    {
        try { PdfiumLibrary.Initialize(); return true; }
        catch { return false; }
    }

    private string GetOutputPath(string fileName) => Path.GetFullPath(Path.Combine(OutputDir, fileName));

    [SkippableFact]
    public void CreateAnnotation_TextNote()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Annotation_TextNote.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);

        using (var annot = page.Annotations.Add(PdfAnnotationSubtype.Text))
        {
            annot.SetRect(100, 700, 130, 730);
            annot.SetColor(new PdfColor(255, 255, 0));
            annot.SetStringValue("Contents", "This is a note annotation.");
            Assert.Equal(PdfAnnotationSubtype.Text, annot.Subtype);
        }

        Assert.Equal(1, page.Annotations.Count);
        page.GenerateContent();
        doc.Save(outputFile);

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void CreateAnnotation_SetColorAndRect()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);

        using var annot = page.Annotations.Add(PdfAnnotationSubtype.Square);
        annot.SetRect(50, 600, 200, 700);
        annot.SetColor(new PdfColor(0, 0, 255));

        var rect = annot.Rect;
        Assert.True(rect.Left >= 49 && rect.Left <= 51);
        Assert.True(rect.Right >= 199 && rect.Right <= 201);

        var color = annot.GetColor();
        Assert.NotNull(color);
        Assert.Equal(0, color.Value.R);
        Assert.Equal(0, color.Value.G);
        Assert.Equal(255, color.Value.B);

        _output.WriteLine("Color and rect set/get verified.");
    }

    [SkippableFact]
    public void EnumerateAnnotations()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);

        using (var a1 = page.Annotations.Add(PdfAnnotationSubtype.Text))
            a1.SetRect(10, 10, 30, 30);
        using (var a2 = page.Annotations.Add(PdfAnnotationSubtype.Square))
            a2.SetRect(50, 50, 100, 100);

        Assert.Equal(2, page.Annotations.Count);

        var subtypes = new List<PdfAnnotationSubtype>();
        foreach (var annot in page.Annotations)
        {
            using (annot)
                subtypes.Add(annot.Subtype);
        }
        Assert.Contains(PdfAnnotationSubtype.Text, subtypes);
        Assert.Contains(PdfAnnotationSubtype.Square, subtypes);

        _output.WriteLine("Annotation enumeration verified.");
    }

    [SkippableFact]
    public void RemoveAnnotation()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);

        using (var annot = page.Annotations.Add(PdfAnnotationSubtype.Text))
            annot.SetRect(10, 10, 30, 30);

        Assert.Equal(1, page.Annotations.Count);
        Assert.True(page.Annotations.Remove(0));
        Assert.Equal(0, page.Annotations.Count);

        _output.WriteLine("Annotation removal verified.");
    }

    [SkippableFact]
    public void HighlightAnnotation_WithAttachmentPoints()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Annotation_Highlight.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("This text should be highlighted.", 72, 700);

        using (var annot = page.Annotations.Add(PdfAnnotationSubtype.Highlight))
        {
            annot.SetRect(72, 695, 350, 715);
            annot.SetColor(new PdfColor(255, 255, 0, 128));

            var quad = new FsQuadPointsF(72, 715, 350, 715, 72, 695, 350, 695);
            annot.AppendAttachmentPoints(quad);
        }

        page.GenerateContent();
        doc.Save(outputFile);

        _output.WriteLine($"PDF saved: {outputFile}");
    }
}
