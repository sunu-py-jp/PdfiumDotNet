using System.Text;
using PdfiumNet;
using PdfiumNet.Drawing;
using PdfiumNet.Geometry;
using PdfiumNet.Text;

PdfiumLibrary.Initialize();

try
{
    var outputDir = Path.Combine(Environment.CurrentDirectory, "output");
    Directory.CreateDirectory(outputDir);

    Console.WriteLine("=== PdfiumNet Sample ===\n");

    // ──────────────────────────────────
    // 1. PDF作成 & 描画
    // ──────────────────────────────────
    Console.WriteLine("1. Creating PDF document...");
    using var doc = PdfDocument.Create();

    var page = doc.AddPage(PdfSize.A4);
    var canvas = page.GetCanvas();
    Console.WriteLine($"   Page size: {page.Width} x {page.Height} points (A4)");

    // テキスト描画
    canvas.SetFillColor(PdfColor.Black);
    canvas.DrawText("PdfiumNet Sample Document", 72, 750, "Helvetica", 24);
    canvas.DrawText("A modern C# wrapper for Google PDFium.", 72, 720, "Helvetica", 12);
    canvas.DrawText("Hello World - This text will be searched later.", 72, 680, "Helvetica", 14);

    // 図形描画
    canvas.SetStrokeColor(PdfColor.Blue);
    canvas.SetStrokeWidth(2);
    canvas.SetFillColor(new PdfColor(200, 220, 255));
    canvas.DrawRectangle(72, 550, 200, 80, DrawMode.FillAndStroke);

    canvas.SetStrokeColor(PdfColor.Red);
    canvas.SetFillColor(new PdfColor(255, 200, 200));
    canvas.DrawCircle(400, 590, 40, DrawMode.FillAndStroke);

    canvas.SetStrokeColor(new PdfColor(128, 0, 128));
    canvas.SetFillColor(new PdfColor(230, 200, 255));
    canvas.DrawPolygon(new[]
    {
        new PdfPoint(72, 400),
        new PdfPoint(172, 480),
        new PdfPoint(272, 400),
    }, DrawMode.FillAndStroke);

    canvas.SetStrokeColor(PdfColor.Green);
    canvas.SetStrokeWidth(1);
    canvas.DrawLine(72, 380, 523, 380);

    page.GenerateContent();

    // 2ページ目
    var page2 = doc.AddPage(PdfSize.A4);
    var canvas2 = page2.GetCanvas();
    canvas2.DrawText("Page 2 - Second page of the sample.", 72, 750, "Helvetica", 16);
    canvas2.DrawText("This page demonstrates multi-page support.", 72, 720, "Helvetica", 12);
    page2.GenerateContent();

    var pdfPath = Path.Combine(outputDir, "sample.pdf");
    doc.Save(pdfPath);
    Console.WriteLine($"   Saved: {pdfPath} ({doc.PageCount} pages)\n");

    // ──────────────────────────────────
    // 2. テキスト抽出 & 検索
    // ──────────────────────────────────
    Console.WriteLine("2. Text extraction & search...");
    using var doc2 = PdfDocument.Open(pdfPath);

    for (var i = 0; i < doc2.PageCount; i++)
    {
        var text = doc2.Pages[i].ExtractText().Trim().ReplaceLineEndings(" | ");
        Console.WriteLine($"   Page {i + 1}: {text}");
    }

    // テキスト検索
    using var textPage = doc2.Pages[0].GetTextPage();
    var results = textPage.Search("Hello");
    Console.WriteLine($"\n   Search 'Hello': {results.Count} match(es)");

    // 座標付き検索
    var boundsResults = textPage.SearchWithBounds("Hello");
    foreach (var r in boundsResults)
    {
        Console.WriteLine($"   - Index={r.StartIndex}, Length={r.Length}");
        foreach (var rect in r.Rectangles)
            Console.WriteLine($"     Rect: Left={rect.Left:F1} Bottom={rect.Bottom:F1} Right={rect.Right:F1} Top={rect.Top:F1}");
    }

    // ──────────────────────────────────
    // 3. 領域テキスト抽出
    // ──────────────────────────────────
    Console.WriteLine("\n3. Region text extraction...");
    var region = new PdfRectangle(0, 670, 600, 760);
    var regionText = textPage.GetTextInRegion(region);
    Console.WriteLine($"   Text in region (0,670)-(600,760): \"{regionText.Trim()}\"");

    var charIdx = textPage.GetCharIndexAtPosition(72, 750, 20, 20);
    Console.WriteLine($"   Char index at (72, 750): {charIdx}");

    // ──────────────────────────────────
    // 4. メタデータ
    // ──────────────────────────────────
    Console.WriteLine("\n4. Metadata...");
    var meta = doc2.Metadata;
    Console.WriteLine($"   Title:    \"{meta.Title}\"");
    Console.WriteLine($"   Author:   \"{meta.Author}\"");
    Console.WriteLine($"   Producer: \"{meta.Producer}\"");
    Console.WriteLine($"   Creator:  \"{meta.Creator}\"");

    // ──────────────────────────────────
    // 5. ドキュメント情報
    // ──────────────────────────────────
    Console.WriteLine("\n5. Document info...");
    Console.WriteLine($"   Page count: {doc2.PageCount}");
    Console.WriteLine($"   File version: {doc2.FileVersion}");
    Console.WriteLine($"   Permissions: {doc2.Permissions} (0x{(uint)doc2.Permissions:X8})");
    Console.WriteLine($"   Security handler revision: {doc2.SecurityHandlerRevision}");
    Console.WriteLine($"   Form type: {doc2.FormType}");
    Console.WriteLine($"   Has forms: {doc2.HasForms}");

    for (var i = 0; i < doc2.PageCount; i++)
    {
        var p = doc2.Pages[i];
        Console.WriteLine($"   Page {i + 1}: {p.Width} x {p.Height} pt, label=\"{p.Label}\", rotation={p.Rotation}");
    }

    // ──────────────────────────────────
    // 6. PNG/BMP レンダリング
    // ──────────────────────────────────
    Console.WriteLine("\n6. Rendering to PNG/BMP...");

    // ページ全体
    var pngPath = Path.Combine(outputDir, "page1.png");
    var png = doc2.Pages[0].RenderToPng(150);
    File.WriteAllBytes(pngPath, png);
    Console.WriteLine($"   Full page PNG: {pngPath} ({png.Length:N0} bytes)");

    var bmpPath = Path.Combine(outputDir, "page1.bmp");
    var bmp = doc2.Pages[0].RenderToBmp(72);
    File.WriteAllBytes(bmpPath, bmp);
    Console.WriteLine($"   Full page BMP: {bmpPath} ({bmp.Length:N0} bytes)");

    // 領域レンダリング
    var cropRegion = new PdfRectangle(50, 530, 480, 770);
    var regionPng = doc2.Pages[0].RenderRegionToPng(cropRegion, 150);
    var regionPngPath = Path.Combine(outputDir, "page1_region.png");
    File.WriteAllBytes(regionPngPath, regionPng);
    Console.WriteLine($"   Region PNG:    {regionPngPath} ({regionPng.Length:N0} bytes)");

    // ──────────────────────────────────
    // 7. ページ操作 (マージ・抽出)
    // ──────────────────────────────────
    Console.WriteLine("\n7. Page operations...");

    // ページ抽出
    using var extracted = doc2.ExtractPages(0);
    var extractedPath = Path.Combine(outputDir, "extracted_page1.pdf");
    extracted.Save(extractedPath);
    Console.WriteLine($"   Extracted page 1: {extractedPath}");

    // マージ
    using var docA = PdfDocument.Create();
    docA.AddPage(PdfSize.A4).GetCanvas().DrawText("Document A", 72, 750);
    docA.Pages[0].GenerateContent();

    using var docB = PdfDocument.Create();
    docB.AddPage(PdfSize.Letter).GetCanvas().DrawText("Document B", 72, 750);
    docB.Pages[0].GenerateContent();

    docA.MergeFrom(docB);
    var mergedPath = Path.Combine(outputDir, "merged.pdf");
    docA.Save(mergedPath);
    Console.WriteLine($"   Merged A+B: {mergedPath} ({docA.PageCount} pages)");

    // ──────────────────────────────────
    // 8. CropBox / MediaBox
    // ──────────────────────────────────
    Console.WriteLine("\n8. CropBox / MediaBox...");
    var mediaBox = doc2.Pages[0].MediaBox;
    Console.WriteLine($"   MediaBox: ({mediaBox.Left}, {mediaBox.Bottom}) - ({mediaBox.Right}, {mediaBox.Top})");

    var cropBox = doc2.Pages[0].CropBox;
    Console.WriteLine($"   CropBox:  {(cropBox.HasValue ? cropBox.Value.ToString() : "(not set)")}");

    // ──────────────────────────────────
    // 9. 添付ファイル
    // ──────────────────────────────────
    Console.WriteLine("\n9. Attachments...");
    using var docAtt = PdfDocument.Create();
    docAtt.AddPage(PdfSize.A4).GenerateContent();
    var att = docAtt.Attachments.Add("hello.txt");
    att.SetFile(Encoding.UTF8.GetBytes("Hello from PdfiumNet!"));
    Console.WriteLine($"   Attachments count: {docAtt.Attachments.Count}");

    var attPath = Path.Combine(outputDir, "with_attachment.pdf");
    docAtt.Save(attPath);
    Console.WriteLine($"   Saved: {attPath}");

    // 読み戻し
    using var docAtt2 = PdfDocument.Open(attPath);
    var retrieved = docAtt2.Attachments[0];
    var content = Encoding.UTF8.GetString(retrieved.GetFile()!);
    Console.WriteLine($"   Retrieved: \"{retrieved.Name}\" -> \"{content}\"");

    // ──────────────────────────────────
    // 10. JavaScript検出 & シグネチャ
    // ──────────────────────────────────
    Console.WriteLine("\n10. Security checks...");
    var jsActions = doc2.GetJavaScriptActions();
    Console.WriteLine($"   JavaScript actions: {jsActions.Count}");
    Console.WriteLine($"   Signatures: {doc2.Signatures.Count}");

    // ──────────────────────────────────
    // 11. ページ平坦化
    // ──────────────────────────────────
    Console.WriteLine("\n11. Flatten...");
    using var docFlat = PdfDocument.Create();
    var flatPage = docFlat.AddPage(PdfSize.A4);
    flatPage.GetCanvas().DrawText("Flattened page", 72, 750);
    flatPage.GenerateContent();
    var flatResult = flatPage.Flatten();
    Console.WriteLine($"   Flatten result: {flatResult}");

    Console.WriteLine("\n=== All samples completed! ===");
    Console.WriteLine($"Output directory: {Path.GetFullPath(outputDir)}");
}
finally
{
    PdfiumLibrary.Destroy();
}
