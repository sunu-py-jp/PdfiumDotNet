using PdfiumNet.Geometry;
using PdfiumNet.Objects;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class TextReplaceTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(TextReplaceTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public TextReplaceTests(ITestOutputHelper output)
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
    public void ReplaceText_SingleObject()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("ReplaceText_SingleObject.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("HelloWorld", 72, 720);
        page.GenerateContent();

        var count = page.ReplaceText("HelloWorld", "ByeWorld");
        page.GenerateContent();
        doc.Save(outputFile);

        Assert.Equal(1, count);

        // Verify the text was replaced
        var text = page.ExtractText();
        Assert.Contains("ByeWorld", text);
        Assert.DoesNotContain("HelloWorld", text);

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void ReplaceText_MultipleObjects()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("ReplaceText_MultipleObjects.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("FooBar", 72, 720);
        canvas.DrawText("FooBaz", 72, 680);
        page.GenerateContent();

        var count = page.ReplaceText("Foo", "Qux");
        page.GenerateContent();
        doc.Save(outputFile);

        Assert.Equal(2, count);

        var text = page.ExtractText();
        Assert.Contains("QuxBar", text);
        Assert.Contains("QuxBaz", text);
        Assert.DoesNotContain("Foo", text);

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void ReplaceText_NoMatch()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Hello", 72, 720);
        page.GenerateContent();

        var count = page.ReplaceText("NotFound", "Replaced");

        Assert.Equal(0, count);
    }

    [SkippableFact]
    public void ReplaceText_CaseInsensitive()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("ReplaceText_CaseInsensitive.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("HelloWorld", 72, 720);
        page.GenerateContent();

        var count = page.ReplaceText("helloworld", "Replaced", StringComparison.OrdinalIgnoreCase);
        page.GenerateContent();
        doc.Save(outputFile);

        Assert.Equal(1, count);

        var text = page.ExtractText();
        Assert.Contains("Replaced", text);

        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void GetText_ReturnsObjectText()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("TestGetText", 72, 720);
        page.GenerateContent();

        using var textPage = page.GetTextPage();
        var found = false;
        foreach (var obj in page.Objects)
        {
            if (obj is PdfTextObject textObj)
            {
                var text = textObj.GetText(textPage);
                if (text.Contains("TestGetText"))
                {
                    found = true;
                    break;
                }
            }
        }

        Assert.True(found, "Expected to find 'TestGetText' in a text object");
    }
}
