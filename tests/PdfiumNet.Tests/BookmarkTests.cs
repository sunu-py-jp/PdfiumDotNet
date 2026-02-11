using PdfiumNet.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class BookmarkTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(BookmarkTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public BookmarkTests(ITestOutputHelper output)
    {
        _output = output;
        Directory.CreateDirectory(OutputDir);
    }

    private static bool IsPdfiumAvailable()
    {
        try { PdfiumLibrary.Initialize(); return true; }
        catch { return false; }
    }

    [SkippableFact]
    public void Bookmarks_NewDocument_IsEmpty()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var bookmarks = doc.Bookmarks;
        Assert.NotNull(bookmarks);
        Assert.Equal(0, bookmarks.Count);

        _output.WriteLine("New document has no bookmarks as expected.");
    }

    [SkippableFact]
    public void Bookmarks_Find_ReturnsNullForMissing()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var result = doc.Bookmarks.Find("Non-existent bookmark");
        Assert.Null(result);

        _output.WriteLine("Find returns null for non-existent bookmark as expected.");
    }
}
