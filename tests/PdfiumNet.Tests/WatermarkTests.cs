using PdfiumNet.Drawing;
using PdfiumNet.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class WatermarkTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(WatermarkTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public WatermarkTests(ITestOutputHelper output)
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
    public void DrawWatermark_DefaultOptions()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Watermark_Default.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();

        canvas.DrawText("This is a normal document page.", 72, 700);
        var watermark = canvas.DrawWatermark();

        Assert.NotNull(watermark);
        page.GenerateContent();
        doc.Save(outputFile);

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void DrawWatermark_CustomOptions()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Watermark_Custom.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();

        canvas.DrawText("Document with custom watermark.", 72, 700);
        canvas.DrawWatermark(new PdfWatermarkOptions
        {
            Text = "DRAFT",
            FontSize = 80,
            Color = new PdfColor(0, 0, 255, 48),
            RotationDegrees = 30
        });

        page.GenerateContent();
        doc.Save(outputFile);

        // Verify text is readable via round-trip
        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("DRAFT", text);

        _output.WriteLine($"PDF saved: {outputFile}");
    }
}
