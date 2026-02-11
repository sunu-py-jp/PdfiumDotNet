using PdfiumNet;
using PdfiumNet.Drawing;
using PdfiumNet.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class TableApiTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(TableApiTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public TableApiTests(ITestOutputHelper output)
    {
        _output = output;
        Directory.CreateDirectory(OutputDir);
    }

    private string GetOutputPath(string f) => Path.GetFullPath(Path.Combine(OutputDir, f));

    private static bool TryInit()
    {
        try { PdfiumLibrary.Initialize(); return true; } catch { return false; }
    }

    [SkippableFact]
    public void DrawTable_FromStringArray()
    {
        Skip.IfNot(TryInit(), "no pdfium");

        var data = new[]
        {
            new[] { "No", "Code", "Name", "Price" },
            new[] { "1", "A-1001", "Pen", "150" },
            new[] { "2", "A-1002", "Notebook", "280" },
        };
        var table = new PdfTable(data);

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        var result = canvas.DrawTable(table, 50, 750);
        page.GenerateContent();

        var outputFile = GetOutputPath("TableApi_StringArray.pdf");
        doc.Save(outputFile);

        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("Pen", text);
        Assert.Contains("280", text);
        Assert.Contains("No", text);
        Assert.Single(result.Pages);
        _output.WriteLine($"Saved: {outputFile}");
    }

    [SkippableFact]
    public void DrawTable_FromCsv()
    {
        Skip.IfNot(TryInit(), "no pdfium");

        var csv = "No,Code,Name,Price\n1,A-1001,Pen,150\n2,A-1002,Notebook,280";
        var table = PdfTable.FromCsv(csv);

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawTable(table, 50, 750);
        page.GenerateContent();

        var outputFile = GetOutputPath("TableApi_Csv.pdf");
        doc.Save(outputFile);

        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("Pen", text);
        Assert.Contains("150", text);
        _output.WriteLine($"Saved: {outputFile}");
    }

    [SkippableFact]
    public void DrawTable_FromTsv()
    {
        Skip.IfNot(TryInit(), "no pdfium");

        var tsv = "No\tCode\tName\tPrice\n1\tA-1001\tPen\t150\n2\tA-1002\tNotebook\t280";
        var table = PdfTable.FromTsv(tsv);

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawTable(table, 50, 750);
        page.GenerateContent();

        var outputFile = GetOutputPath("TableApi_Tsv.pdf");
        doc.Save(outputFile);

        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("Notebook", text);
        Assert.Contains("280", text);
        _output.WriteLine($"Saved: {outputFile}");
    }

    [SkippableFact]
    public void DrawTable_CustomStyle()
    {
        Skip.IfNot(TryInit(), "no pdfium");

        var data = new[]
        {
            new[] { "No", "Code", "Name", "Price" },
            new[] { "1", "A-1001", "Pen", "150" },
            new[] { "2", "A-1002", "Notebook", "280" },
            new[] { "3", "B-2001", "Eraser", "80" },
            new[] { "4", "B-2002", "Pencil", "350" },
        };
        var style = new PdfTableStyle
        {
            AlternateRowColor = new PdfColor(245, 245, 250),
            ColumnAlignments = new[]
            {
                PdfTableColumnAlignment.Left,
                PdfTableColumnAlignment.Left,
                PdfTableColumnAlignment.Left,
                PdfTableColumnAlignment.Right,
            },
            ColumnWidths = new float[] { 40, 90, 160, 70 },
        };
        var table = new PdfTable(data, style);

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawTable(table, 50, 750);
        page.GenerateContent();

        var outputFile = GetOutputPath("TableApi_CustomStyle.pdf");
        doc.Save(outputFile);

        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("Eraser", text);
        Assert.Contains("350", text);
        _output.WriteLine($"Saved: {outputFile}");
    }

    [SkippableFact]
    public void DrawTable_CsvWithQuotes()
    {
        Skip.IfNot(TryInit(), "no pdfium");

        var csv = "Name,Description,Price\n\"Pen, Blue\",\"A nice \"\"blue\"\" pen\",150\nNotebook,Standard,280";
        var table = PdfTable.FromCsv(csv);

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        canvas.DrawTable(table, 50, 750);
        page.GenerateContent();

        var outputFile = GetOutputPath("TableApi_CsvQuotes.pdf");
        doc.Save(outputFile);

        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("Pen, Blue", text);
        Assert.Contains("blue", text);
        Assert.Contains("Notebook", text);
        _output.WriteLine($"Saved: {outputFile}");
    }

    [SkippableFact]
    public void DrawTable_NoHeader()
    {
        Skip.IfNot(TryInit(), "no pdfium");

        var data = new[]
        {
            new[] { "1", "A-1001", "Pen", "150" },
            new[] { "2", "A-1002", "Notebook", "280" },
        };
        var style = new PdfTableStyle { HasHeader = false };
        var table = new PdfTable(data, style);

        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var canvas = page.GetCanvas();
        var result = canvas.DrawTable(table, 50, 750);
        page.GenerateContent();

        var outputFile = GetOutputPath("TableApi_NoHeader.pdf");
        doc.Save(outputFile);

        using var doc2 = PdfDocument.Open(outputFile);
        var text = doc2.Pages[0].ExtractText();
        Assert.Contains("Pen", text);
        Assert.Contains("280", text);
        // BottomY should be y - 2 * RowHeight (no header row height used)
        Assert.Equal(750 - 2 * 22, result.BottomY);
        _output.WriteLine($"Saved: {outputFile}");
    }
}
