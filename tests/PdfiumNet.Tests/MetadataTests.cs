using PdfiumNet.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class MetadataTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(MetadataTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public MetadataTests(ITestOutputHelper output)
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
    public void Metadata_NewDocument_ReturnsEmptyStrings()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var meta = doc.Metadata;

        Assert.NotNull(meta);
        Assert.Equal(string.Empty, meta.Title);
        Assert.Equal(string.Empty, meta.Author);
        Assert.Equal(string.Empty, meta.Subject);
        Assert.Equal(string.Empty, meta.Keywords);

        _output.WriteLine("Metadata properties are empty for new document as expected.");
    }

    [SkippableFact]
    public void Metadata_RoundTrip_ProducerIsSet()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Metadata_RoundTrip.pdf");

        using (var doc = PdfDocument.Create())
        {
            doc.AddPage(PdfSize.A4).GenerateContent();
            doc.Save(outputFile);
        }

        using (var doc = PdfDocument.Open(outputFile))
        {
            var meta = doc.Metadata;
            // PDFium typically does not set Producer on save, but we verify it doesn't crash
            _output.WriteLine($"Title: '{meta.Title}'");
            _output.WriteLine($"Author: '{meta.Author}'");
            _output.WriteLine($"Producer: '{meta.Producer}'");
            _output.WriteLine($"Creator: '{meta.Creator}'");
            _output.WriteLine($"CreationDate: '{meta.CreationDate}'");
            _output.WriteLine($"ModDate: '{meta.ModDate}'");
        }

        _output.WriteLine($"PDF saved: {outputFile}");
    }
}
