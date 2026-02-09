using PdfiumNet.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

/// <summary>
/// Integration tests for PdfDocument.
/// These tests require the PDFium native library to be available.
/// Tests are skipped if the library is not found.
/// Generated PDFs are saved to test-output/ directory for inspection.
/// </summary>
public class PdfDocumentTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(PdfDocumentTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public PdfDocumentTests(ITestOutputHelper output)
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

    [SkippableFact]
    public void CreateAndSave_RoundTrip()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("CreateAndSave_RoundTrip.pdf");

        // Create a document with a page
        using (var doc = PdfDocument.Create())
        {
            var page = doc.AddPage(PdfSize.A4);
            Assert.Equal(1, doc.PageCount);
            Assert.True(page.Width > 595 && page.Width < 596);

            page.GenerateContent();
            doc.Save(outputFile);
        }

        // Re-open and verify
        using (var doc = PdfDocument.Open(outputFile))
        {
            Assert.Equal(1, doc.PageCount);
            var page = doc.Pages[0];
            Assert.True(page.Width > 595 && page.Width < 596);
        }

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void CreateWithTextAndExtract()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("CreateWithTextAndExtract.pdf");

        using (var doc = PdfDocument.Create())
        {
            var page = doc.AddPage(PdfSize.A4);
            var canvas = page.GetCanvas();
            canvas.DrawText("Hello, PDFium!", 72, 720);
            page.GenerateContent();
            doc.Save(outputFile);
        }

        using (var doc = PdfDocument.Open(outputFile))
        {
            var text = doc.Pages[0].ExtractText();
            Assert.Contains("Hello", text);
            Assert.Contains("PDFium", text);
        }

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void AddAndRemovePages()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("AddAndRemovePages.pdf");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4);
        doc.AddPage(PdfSize.Letter);
        Assert.Equal(2, doc.PageCount);

        doc.RemovePage(0);
        Assert.Equal(1, doc.PageCount);

        doc.Pages[0].GenerateContent();
        doc.Save(outputFile);
        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void SaveToStream()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("SaveToStream.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        using var ms = new MemoryStream();
        doc.Save(ms);
        Assert.True(ms.Length > 0);

        // Also save to file for inspection
        File.WriteAllBytes(outputFile, ms.ToArray());

        // Verify the stream contains a valid PDF
        ms.Position = 0;
        using var doc2 = PdfDocument.Open(ms);
        Assert.Equal(1, doc2.PageCount);

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void OpenFromBytes()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("OpenFromBytes.pdf");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4);
        doc.Pages[0].GenerateContent();

        using var ms = new MemoryStream();
        doc.Save(ms);
        var bytes = ms.ToArray();

        // Save to file for inspection
        File.WriteAllBytes(outputFile, bytes);

        using var doc2 = PdfDocument.Open(bytes);
        Assert.Equal(1, doc2.PageCount);

        _output.WriteLine($"PDF saved: {outputFile}");
    }
}

/// <summary>
/// Attribute to skip tests when a condition is not met.
/// Requires Xunit.SkippableFact NuGet package.
/// Falls back to regular Fact if not available.
/// </summary>
public class SkippableFactAttribute : FactAttribute { }

public static class Skip
{
    public static void IfNot(bool condition, string reason)
    {
        if (!condition)
            throw new SkipException(reason);
    }
}

public class SkipException : Exception
{
    public SkipException(string message) : base(message) { }
}
