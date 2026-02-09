using PdfiumNet;
using PdfiumNet.Drawing;
using PdfiumNet.Geometry;

// Initialize PDFium
PdfiumLibrary.Initialize();

try
{
    Console.WriteLine("=== PdfiumNet Sample ===\n");

    // 1. Create a new PDF document
    Console.WriteLine("1. Creating PDF document...");
    using var doc = PdfDocument.Create();

    // 2. Add a page
    var page = doc.AddPage(PdfSize.A4);
    var canvas = page.GetCanvas();
    Console.WriteLine($"   Page size: {page.Width} x {page.Height} points");

    // 3. Draw text
    Console.WriteLine("2. Drawing text...");
    canvas.SetFillColor(PdfColor.Black);
    canvas.DrawText("PdfiumNet Sample Document", 72, 750, "Helvetica", 24);
    canvas.DrawText("This PDF was created using PdfiumNet.", 72, 710, "Helvetica", 14);
    canvas.DrawText("A modern C# wrapper for Google PDFium.", 72, 690, "Helvetica", 14);

    // 4. Draw shapes
    Console.WriteLine("3. Drawing shapes...");

    // Rectangle
    canvas.SetStrokeColor(PdfColor.Blue);
    canvas.SetStrokeWidth(2);
    canvas.SetFillColor(new PdfColor(200, 220, 255));
    canvas.DrawRectangle(72, 550, 200, 100, DrawMode.FillAndStroke);

    // Circle
    canvas.SetStrokeColor(PdfColor.Red);
    canvas.SetFillColor(new PdfColor(255, 200, 200));
    canvas.DrawCircle(400, 600, 50, DrawMode.FillAndStroke);

    // Line
    canvas.SetStrokeColor(PdfColor.Green);
    canvas.SetStrokeWidth(3);
    canvas.DrawLine(72, 500, 523, 500);

    // 5. Draw custom path (triangle)
    Console.WriteLine("4. Drawing custom path...");
    canvas.SetStrokeColor(new PdfColor(128, 0, 128));
    canvas.SetFillColor(new PdfColor(230, 200, 255));
    canvas.DrawPolygon(new[]
    {
        new PdfPoint(72, 350),
        new PdfPoint(172, 450),
        new PdfPoint(272, 350),
    }, DrawMode.FillAndStroke);

    // 6. Generate content and save
    page.GenerateContent();

    // Add a second page
    var page2 = doc.AddPage(PdfSize.A4);
    var canvas2 = page2.GetCanvas();
    canvas2.DrawText("Page 2", 72, 750, "Helvetica", 18);
    canvas2.DrawText("This is the second page of the sample document.", 72, 720, "Helvetica", 12);
    page2.GenerateContent();

    var outputPath = Path.Combine(Environment.CurrentDirectory, "sample_output.pdf");
    doc.Save(outputPath);
    Console.WriteLine($"\n5. Saved to: {outputPath}");
    Console.WriteLine($"   Pages: {doc.PageCount}");

    // 7. Re-open and extract text
    Console.WriteLine("\n6. Re-opening and extracting text...");
    using var doc2 = PdfDocument.Open(outputPath);
    Console.WriteLine($"   Page count: {doc2.PageCount}");

    for (var i = 0; i < doc2.PageCount; i++)
    {
        var text = doc2.Pages[i].ExtractText();
        Console.WriteLine($"   Page {i + 1} text: {text.Trim().ReplaceLineEndings(" | ")}");
    }

    // 8. Text search
    Console.WriteLine("\n7. Searching for 'PDFium'...");
    using var textPage = doc2.Pages[0].GetTextPage();
    var results = textPage.Search("PDFium");
    Console.WriteLine($"   Found {results.Count} occurrence(s)");
    foreach (var result in results)
    {
        var matchText = textPage.GetText(result.StartIndex, result.Length);
        Console.WriteLine($"   - At index {result.StartIndex}: \"{matchText}\"");
    }

    // 9. Character info
    Console.WriteLine("\n8. Character info (first 20 chars)...");
    var chars = textPage.GetAllCharacterInfo();
    foreach (var ch in chars.Take(20))
    {
        Console.WriteLine($"   [{ch.Index}] '{ch.Character}' at ({ch.Origin.X:F1}, {ch.Origin.Y:F1}) " +
                          $"size={ch.FontSize:F1} font={ch.FontName}");
    }

    Console.WriteLine("\nDone!");
}
finally
{
    PdfiumLibrary.Destroy();
}
