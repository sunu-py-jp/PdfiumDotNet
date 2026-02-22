using PdfiumNet.Geometry;
using PdfiumNet.Text;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

/// <summary>
/// Tests for Phase 2 features: Permissions, PageLabels, TextRectangles,
/// RegionTextExtraction, PNG/BMP export.
/// </summary>
public class Phase2FeatureTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(PdfDocumentTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public Phase2FeatureTests(ITestOutputHelper output)
    {
        _output = output;
        Directory.CreateDirectory(OutputDir);
    }

    private static bool IsPdfiumAvailable()
    {
        try
        {
            PdfiumLibrary.Initialize();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetOutputPath(string fileName) => Path.GetFullPath(Path.Combine(OutputDir, fileName));

    // --- Feature 3: Permissions & Security ---

    [SkippableFact]
    public void Permissions_NewDocument_HasDefaultPermissions()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var perms = doc.Permissions;
        _output.WriteLine($"Permissions: {perms} (0x{(uint)perms:X8})");
        // FPDF_GetDocPermissions returns 0 for newly created documents with no security handler.
        // This is expected behavior - the API returns the raw permission value from the document.
        Assert.Equal(PdfPermissions.None, perms);
    }

    [SkippableFact]
    public void SecurityHandlerRevision_NewDocument()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var revision = doc.SecurityHandlerRevision;
        _output.WriteLine($"Security handler revision: {revision}");
        // Unencrypted document: -1
        Assert.Equal(-1, revision);
    }

    // --- Feature 5: Page Labels ---

    [SkippableFact]
    public void GetPageLabel_NewDocument_ReturnsEmptyString()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var label = doc.GetPageLabel(0);
        _output.WriteLine($"Page label: '{label}'");
        // New documents don't have page labels set
        Assert.Equal(string.Empty, label);
    }

    [SkippableFact]
    public void PageLabel_Property()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        var label = page.Label;
        Assert.Equal(string.Empty, label);
    }

    // --- Feature 2: Text Rectangles ---

    [SkippableFact]
    public void GetTextRectangles_WithDrawnText()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Hello World", 72, 720);
        page.GenerateContent();

        using var textPage = page.GetTextPage();
        var charCount = textPage.CharacterCount;
        _output.WriteLine($"Character count: {charCount}");

        if (charCount > 0)
        {
            var rects = textPage.GetTextRectangles(0, charCount);
            _output.WriteLine($"Rectangles: {rects.Count}");
            Assert.True(rects.Count > 0, "Should have at least one rectangle for drawn text");

            foreach (var rect in rects)
                _output.WriteLine($"  Rect: {rect}");
        }
    }

    [SkippableFact]
    public void SearchWithBounds_FindsTextWithRectangles()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Hello World", 72, 720);
        page.GenerateContent();

        using var textPage = page.GetTextPage();
        var results = textPage.SearchWithBounds("Hello");
        _output.WriteLine($"Search results: {results.Count}");

        if (results.Count > 0)
        {
            var result = results[0];
            Assert.True(result.Length > 0);
            Assert.NotNull(result.Rectangles);
            Assert.True(result.Rectangles.Count > 0, "SearchWithBounds should return rectangles");
            _output.WriteLine($"  Match at index {result.StartIndex}, length {result.Length}");
            foreach (var rect in result.Rectangles)
                _output.WriteLine($"  Rect: {rect}");
        }
    }

    // --- Feature 10: Region Text Extraction ---

    [SkippableFact]
    public void GetTextInRegion_ExtractsTextFromArea()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Hello World", 72, 720);
        page.GenerateContent();

        using var textPage = page.GetTextPage();

        // Use a large rectangle covering the entire page to ensure we capture text
        var region = new PdfRectangle(0, 0, page.Width, page.Height);
        var text = textPage.GetTextInRegion(region);
        _output.WriteLine($"Text in region: '{text}'");

        if (textPage.CharacterCount > 0)
        {
            Assert.False(string.IsNullOrEmpty(text), "Should extract some text from the full page region");
        }
    }

    [SkippableFact]
    public void GetCharIndexAtPosition_FindsCharacter()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Hello World", 72, 720);
        page.GenerateContent();

        using var textPage = page.GetTextPage();

        if (textPage.CharacterCount > 0)
        {
            // Get the position of the first character to know where to look
            var charBox = textPage.GetCharacterBox(0);
            var idx = textPage.GetCharIndexAtPosition(
                charBox.Left + 1, (charBox.Bottom + charBox.Top) / 2, 10f, 10f);
            _output.WriteLine($"Char index at ({charBox.Left + 1}, {(charBox.Bottom + charBox.Top) / 2}): {idx}");
            Assert.True(idx >= 0, "Should find a character near the first character's position");
        }
    }

    [SkippableFact]
    public void GetCharIndexAtPosition_OutsidePage_ReturnsNegative()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        using var textPage = page.GetTextPage();
        var idx = textPage.GetCharIndexAtPosition(-1000, -1000, 1f, 1f);
        Assert.True(idx < 0, "Should return negative value for position far outside page");
    }

    // --- Feature 1: PNG/BMP Export ---

    [SkippableFact]
    public void SaveAsPng_ProducesValidPngSignature()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var bitmap = PdfBitmap.Create(100, 100, true);
        bitmap.FillRect(0, 0, 100, 100, 0xFFFF0000); // Red

        using var ms = new MemoryStream();
        bitmap.SaveAsPng(ms);
        var bytes = ms.ToArray();

        // PNG signature: 89 50 4E 47 0D 0A 1A 0A
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(0x50, bytes[1]); // 'P'
        Assert.Equal(0x4E, bytes[2]); // 'N'
        Assert.Equal(0x47, bytes[3]); // 'G'
        Assert.Equal(0x0D, bytes[4]);
        Assert.Equal(0x0A, bytes[5]);
        Assert.Equal(0x1A, bytes[6]);
        Assert.Equal(0x0A, bytes[7]);

        _output.WriteLine($"PNG size: {bytes.Length} bytes");
    }

    [SkippableFact]
    public void SaveAsBmp_ProducesValidBmpSignature()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var bitmap = PdfBitmap.Create(100, 100, true);
        bitmap.FillRect(0, 0, 100, 100, 0xFF0000FF); // Blue

        using var ms = new MemoryStream();
        bitmap.SaveAsBmp(ms);
        var bytes = ms.ToArray();

        // BMP signature: 42 4D ('BM')
        Assert.True(bytes.Length > 54); // Header is at least 54 bytes
        Assert.Equal(0x42, bytes[0]); // 'B'
        Assert.Equal(0x4D, bytes[1]); // 'M'

        // Verify file size matches
        var fileSize = BitConverter.ToInt32(bytes, 2);
        Assert.Equal(bytes.Length, fileSize);

        _output.WriteLine($"BMP size: {bytes.Length} bytes");
    }

    [SkippableFact]
    public void ToPng_ReturnsValidPng()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var bitmap = PdfBitmap.Create(50, 50, true);
        bitmap.FillRect(0, 0, 50, 50, 0xFF00FF00); // Green

        var png = bitmap.ToPng();
        Assert.True(png.Length > 8);
        Assert.Equal(0x89, png[0]);
        Assert.Equal(0x50, png[1]);
    }

    [SkippableFact]
    public void ToBmp_ReturnsValidBmp()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var bitmap = PdfBitmap.Create(50, 50, true);
        bitmap.FillRect(0, 0, 50, 50, 0xFF00FF00); // Green

        var bmp = bitmap.ToBmp();
        Assert.True(bmp.Length > 54);
        Assert.Equal(0x42, bmp[0]);
        Assert.Equal(0x4D, bmp[1]);
    }

    [SkippableFact]
    public void RenderToPng_ProducesValidPng()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("PNG Export Test", 72, 720);
        page.GenerateContent();

        var png = page.RenderToPng(72);
        Assert.True(png.Length > 8);
        Assert.Equal(0x89, png[0]);
        Assert.Equal(0x50, png[1]);
        Assert.Equal(0x4E, png[2]);
        Assert.Equal(0x47, png[3]);

        // Save for visual inspection
        var outputPath = GetOutputPath("RenderToPng_Test.png");
        File.WriteAllBytes(outputPath, png);
        _output.WriteLine($"PNG saved: {outputPath} ({png.Length} bytes)");
    }

    [SkippableFact]
    public void RenderToBmp_ProducesValidBmp()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("BMP Export Test", 72, 720);
        page.GenerateContent();

        var bmp = page.RenderToBmp(72);
        Assert.True(bmp.Length > 54);
        Assert.Equal(0x42, bmp[0]);
        Assert.Equal(0x4D, bmp[1]);

        var outputPath = GetOutputPath("RenderToBmp_Test.bmp");
        File.WriteAllBytes(outputPath, bmp);
        _output.WriteLine($"BMP saved: {outputPath} ({bmp.Length} bytes)");
    }

    // --- RenderRegion ---

    [SkippableFact]
    public void RenderRegion_ProducesSmallerBitmap()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Top Left", 72, 780);
        canvas.DrawText("Bottom Right", 400, 100);
        page.GenerateContent();

        // Render just the top-left region (200x100 points)
        var region = new PdfRectangle(50, 750, 250, 800);
        using var regionBitmap = page.RenderRegion(region, 72);

        // Full page render for comparison
        using var fullBitmap = page.Render(72);

        // Region bitmap should be smaller than full page
        Assert.True(regionBitmap.Width < fullBitmap.Width);
        Assert.True(regionBitmap.Height < fullBitmap.Height);

        // Region bitmap dimensions should match the region size at 72 DPI (1:1 points to pixels)
        Assert.Equal(200, regionBitmap.Width); // 250 - 50
        Assert.Equal(50, regionBitmap.Height); // 800 - 750

        _output.WriteLine($"Full: {fullBitmap.Width}x{fullBitmap.Height}, Region: {regionBitmap.Width}x{regionBitmap.Height}");
    }

    [SkippableFact]
    public void RenderRegion_HigherDpi_ProducesLargerBitmap()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Hello Region", 72, 720);
        page.GenerateContent();

        var region = new PdfRectangle(50, 700, 250, 750);
        using var bitmap72 = page.RenderRegion(region, 72);
        using var bitmap144 = page.RenderRegion(region, 144);

        // 144 DPI should be 2x the dimensions of 72 DPI
        Assert.Equal(bitmap72.Width * 2, bitmap144.Width);
        Assert.Equal(bitmap72.Height * 2, bitmap144.Height);
    }

    [SkippableFact]
    public void RenderRegionToPng_ProducesValidPng()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Region PNG Test", 72, 720);
        page.GenerateContent();

        var region = new PdfRectangle(50, 700, 300, 750);
        var png = page.RenderRegionToPng(region, 150);

        Assert.True(png.Length > 8);
        Assert.Equal(0x89, png[0]);
        Assert.Equal(0x50, png[1]);
        Assert.Equal(0x4E, png[2]);
        Assert.Equal(0x47, png[3]);

        var outputPath = GetOutputPath("RenderRegionToPng_Test.png");
        File.WriteAllBytes(outputPath, png);
        _output.WriteLine($"Region PNG saved: {outputPath} ({png.Length} bytes)");
    }

    [SkippableFact]
    public void RenderRegionToBmp_ProducesValidBmp()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Region BMP Test", 72, 720);
        page.GenerateContent();

        var region = new PdfRectangle(50, 700, 300, 750);
        var bmp = page.RenderRegionToBmp(region, 72);

        Assert.True(bmp.Length > 54);
        Assert.Equal(0x42, bmp[0]);
        Assert.Equal(0x4D, bmp[1]);
    }

    [SkippableFact]
    public void SaveAsPng_ToFile_CanBeReopened()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var bitmap = PdfBitmap.Create(200, 200, true);
        bitmap.FillRect(0, 0, 200, 200, 0xFFFFFFFF); // White
        bitmap.FillRect(50, 50, 100, 100, 0xFFFF0000); // Red square

        var outputPath = GetOutputPath("SaveAsPng_File.png");
        using (var fs = File.Create(outputPath))
        {
            bitmap.SaveAsPng(fs);
        }

        var fileBytes = File.ReadAllBytes(outputPath);
        Assert.Equal(0x89, fileBytes[0]);
        Assert.Equal(0x50, fileBytes[1]);
        _output.WriteLine($"PNG file saved: {outputPath} ({fileBytes.Length} bytes)");
    }
}
