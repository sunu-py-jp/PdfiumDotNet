using PdfiumNet.Attachments;
using PdfiumNet.Geometry;
using PdfiumNet.Links;
using PdfiumNet.Objects;
using PdfiumNet.Signatures;
using PdfiumNet.StructTree;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

/// <summary>
/// Tests for new features: Flatten, MergeFrom/ExtractPages, CropBox/MediaBox,
/// Attachments, Links, ImageExtraction, Signatures, Thumbnails, StructTree, JS detection.
/// </summary>
public class NewFeatureTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(PdfDocumentTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public NewFeatureTests(ITestOutputHelper output)
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

    // --- RenderFlags ---

    [SkippableFact]
    public void Render_WithRenderFlags()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        // Test with grayscale flag
        using var bitmap = page.Render(72, RenderFlags.Annotations | RenderFlags.Grayscale);
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
    }

    [SkippableFact]
    public void Render_WithNoFlags()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        using var bitmap = page.Render(72, RenderFlags.None);
        Assert.True(bitmap.Width > 0);
    }

    // --- Flatten ---

    [SkippableFact]
    public void Flatten_EmptyPage_ReturnsTrue()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        var result = page.Flatten();
        Assert.True(result); // Nothing to flatten = FLATTEN_NOTHINGTODO = success
    }

    // --- MediaBox / CropBox ---

    [SkippableFact]
    public void MediaBox_GetSet()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        var mediaBox = page.MediaBox;
        Assert.True(mediaBox.Width > 0);
        Assert.True(mediaBox.Height > 0);

        // Set a custom MediaBox
        page.MediaBox = new PdfRectangle(0, 0, 300, 400);
        var newMediaBox = page.MediaBox;
        Assert.Equal(300, newMediaBox.Right, 1);
        Assert.Equal(400, newMediaBox.Top, 1);
    }

    [SkippableFact]
    public void CropBox_GetSet()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        // By default, no explicit CropBox
        var cropBox = page.CropBox;
        // Some implementations may return null, some may return MediaBox
        // We just test set/get round-trip

        page.CropBox = new PdfRectangle(10, 10, 200, 300);
        var newCropBox = page.CropBox;
        Assert.NotNull(newCropBox);
        Assert.Equal(10, newCropBox.Value.Left, 1);
        Assert.Equal(200, newCropBox.Value.Right, 1);
    }

    // --- MergeFrom / ExtractPages ---

    [SkippableFact]
    public void MergeFrom_CombinesTwoDocuments()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc1 = PdfDocument.Create();
        doc1.AddPage(PdfSize.A4).GenerateContent();
        doc1.AddPage(PdfSize.A4).GenerateContent();

        using var doc2 = PdfDocument.Create();
        doc2.AddPage(PdfSize.Letter).GenerateContent();

        doc1.MergeFrom(doc2);
        Assert.Equal(3, doc1.PageCount);
    }

    [SkippableFact]
    public void ExtractPages_CreatesNewDocument()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();
        doc.AddPage(PdfSize.Letter).GenerateContent();
        doc.AddPage(PdfSize.A4).GenerateContent();

        using var extracted = doc.ExtractPages(0, 2);
        Assert.Equal(2, extracted.PageCount);
    }

    [SkippableFact]
    public void ExtractPages_SaveRoundTrip()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("ExtractPages_RoundTrip.pdf");

        using var doc = PdfDocument.Create();
        var canvas = doc.AddPage(PdfSize.A4).GetCanvas();
        canvas.DrawText("Page 1", 72, 720);
        doc.Pages[0].GenerateContent();

        doc.AddPage(PdfSize.Letter).GenerateContent();

        using var extracted = doc.ExtractPages(0);
        extracted.Save(outputFile);

        using var reopened = PdfDocument.Open(outputFile);
        Assert.Equal(1, reopened.PageCount);
        _output.WriteLine($"PDF saved: {outputFile}");
    }

    // --- Cache invalidation (R2) ---

    [SkippableFact]
    public void RemovePage_InvalidatesCache()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();
        doc.AddPage(PdfSize.Letter).GenerateContent();

        // Access page 0 to cache it
        var _ = doc.Pages[0];
        Assert.Equal(2, doc.PageCount);

        doc.RemovePage(0);
        Assert.Equal(1, doc.PageCount);

        // Accessing page 0 after removal should give the former page 1
        var page = doc.Pages[0];
        Assert.NotNull(page);
    }

    // --- Attachments ---

    [SkippableFact]
    public void Attachments_AddAndRetrieve()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Attachments_Test.pdf");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        Assert.Equal(0, doc.Attachments.Count);

        var attachment = doc.Attachments.Add("test.txt");
        Assert.Equal("test.txt", attachment.Name);

        var data = System.Text.Encoding.UTF8.GetBytes("Hello, attachment!");
        attachment.SetFile(data);

        Assert.Equal(1, doc.Attachments.Count);

        doc.Save(outputFile);
        _output.WriteLine($"PDF saved: {outputFile}");
    }

    [SkippableFact]
    public void Attachments_GetFile_RoundTrip()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Attachments_RoundTrip.pdf");
        var testData = System.Text.Encoding.UTF8.GetBytes("Test content for attachment");

        using (var doc = PdfDocument.Create())
        {
            doc.AddPage(PdfSize.A4).GenerateContent();
            var att = doc.Attachments.Add("data.bin");
            att.SetFile(testData);
            doc.Save(outputFile);
        }

        using (var doc = PdfDocument.Open(outputFile))
        {
            Assert.Equal(1, doc.Attachments.Count);
            var att = doc.Attachments[0];
            Assert.Equal("data.bin", att.Name);

            var retrieved = att.GetFile();
            Assert.NotNull(retrieved);
            Assert.Equal(testData, retrieved);
        }
    }

    // --- Signatures ---

    [SkippableFact]
    public void Signatures_EmptyDocument_HasNoSignatures()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        Assert.Equal(0, doc.Signatures.Count);
    }

    // --- JavaScript detection ---

    [SkippableFact]
    public void JavaScriptActions_EmptyDocument_HasNone()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var actions = doc.GetJavaScriptActions();
        Assert.Empty(actions);
    }

    // --- Links ---

    [SkippableFact]
    public void GetLinks_EmptyPage_ReturnsEmptyList()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        var links = page.GetLinks();
        Assert.Empty(links);
    }

    // --- Thumbnails ---

    [SkippableFact]
    public void GetThumbnail_NewPage_ReturnsNull()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        // Newly created pages don't have thumbnails
        var thumbnail = page.GetThumbnail();
        Assert.Null(thumbnail);
    }

    // --- Structure Tree ---

    [SkippableFact]
    public void StructTree_EmptyPage()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        page.GenerateContent();

        using var tree = PdfStructTree.GetForPage(page);
        // Empty pages may or may not have a struct tree
        if (tree != null)
        {
            Assert.True(tree.ChildCount >= 0);
        }
    }

    // --- Image extraction ---

    [SkippableFact]
    public void ImageObject_GetImageData()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);

        // Create a simple bitmap and set it as an image object
        using var bitmap = PdfBitmap.Create(100, 100, false);
        bitmap.FillRect(0, 0, 100, 100, 0xFFFF0000); // Red

        var imageObj = PdfImageObject.Create(doc);
        imageObj.SetBitmap(bitmap);
        imageObj.SetBounds(0, 0, 100, 100);
        page.InsertObject(imageObj);
        page.GenerateContent();

        // Now retrieve the image data from the page objects
        foreach (var obj in page.Objects)
        {
            if (obj is PdfImageObject imgObj)
            {
                var decoded = imgObj.GetImageDataDecoded();
                // Should have some data
                if (decoded != null)
                {
                    Assert.True(decoded.Length > 0);
                    _output.WriteLine($"Decoded image data: {decoded.Length} bytes");
                }
                break;
            }
        }
    }

    // --- Flatten with save ---

    [SkippableFact]
    public void Flatten_SaveRoundTrip()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        var outputFile = GetOutputPath("Flatten_RoundTrip.pdf");

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawText("Flattened page", 72, 720);
        page.GenerateContent();

        page.Flatten();

        doc.Save(outputFile);

        using var reopened = PdfDocument.Open(outputFile);
        Assert.Equal(1, reopened.PageCount);
        var text = reopened.Pages[0].ExtractText();
        Assert.Contains("Flattened", text);

        _output.WriteLine($"PDF saved: {outputFile}");
    }
}
