using PdfiumNet.Forms;
using PdfiumNet.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class FormTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(FormTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public FormTests(ITestOutputHelper output)
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
    public void FormType_NewDocument_IsNone()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        Assert.Equal(PdfFormType.None, doc.FormType);
        Assert.False(doc.HasForms);

        _output.WriteLine("New document has no forms as expected.");
    }

    [SkippableFact]
    public void GetFormInfo_NewDocument_NoFields()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        using var formInfo = doc.GetFormInfo();
        Assert.NotNull(formInfo);
        Assert.False(formInfo.HasForms);

        var fields = formInfo.GetFields();
        Assert.Empty(fields);

        _output.WriteLine("New document form info returns no fields as expected.");
    }

    [SkippableFact]
    public void GetFormInfo_DisposeTwice_NoError()
    {
        Skip.IfNot(IsPdfiumAvailable(), "PDFium native library not available");

        using var doc = PdfDocument.Create();
        doc.AddPage(PdfSize.A4).GenerateContent();

        var formInfo = doc.GetFormInfo();
        formInfo.Dispose();
        formInfo.Dispose(); // Should not throw

        _output.WriteLine("Double dispose is safe.");
    }
}
