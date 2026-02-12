# PdfiumDotNet

A C# wrapper for [Google PDFium](https://pdfium.googlesource.com/pdfium/) that provides a clean, high-level API for PDF creation, editing, text extraction, rendering, and more.

## Features

- **PDF creation & page management** - Create, add, remove, merge, extract pages
- **Text drawing** with standard/custom fonts
- **Shape drawing** - rectangle, circle, ellipse, line, polygon, custom path
- **Image insertion** and extraction
- **Text extraction & search** - full page or region-based, with bounding rectangles
- **Annotations** - create, modify, remove (text notes, highlights, etc.)
- **Bookmarks** - navigate document outline
- **Form fields** - read form type and field information
- **Metadata** - read/write title, author, subject, etc.
- **Attachments** - add and retrieve embedded files
- **Digital signatures** - enumerate signature fields
- **Rendering** - render pages to bitmap at any DPI
- **PNG/BMP export** - save bitmaps or render pages directly to PNG/BMP (no external dependencies)
- **Document permissions & security** - read permission flags and security handler info
- **Page labels** - access logical page numbering (e.g. "i", "ii", "1", "2")
- **Structure tree** - access tagged PDF structure
- **JavaScript detection** - enumerate JS actions for security auditing
- **Flatten** - flatten annotations/forms into page content
- **CropBox/MediaBox** - get/set page boundaries
- **Watermarks** - draw text watermarks
- **Table drawing** - draw tables with customizable styles

## Requirements

- .NET 10.0+
- PDFium native library (`libpdfium.dylib` / `pdfium.dll` / `libpdfium.so`)

### Getting the native library

```bash
# Download from https://github.com/bblanchon/pdfium-binaries/releases
```

Place the library in `runtimes/{rid}/native/` (e.g. `runtimes/osx-arm64/native/libpdfium.dylib`) or next to your application binary.

## Quick Start

### Create a PDF

```csharp
using PdfiumNet;
using PdfiumNet.Drawing;
using PdfiumNet.Geometry;

PdfiumLibrary.Initialize();

using var doc = PdfDocument.Create();
var page = doc.AddPage(PdfSize.A4);
var canvas = page.GetCanvas();

// Draw text
canvas.SetFillColor(PdfColor.Black);
canvas.DrawText("Hello, World!", 72, 750, "Helvetica", 24);

// Draw shapes
canvas.SetStrokeColor(PdfColor.Blue);
canvas.SetFillColor(new PdfColor(200, 220, 255));
canvas.DrawRectangle(72, 650, 200, 60, DrawMode.FillAndStroke);

page.GenerateContent();
doc.Save("output.pdf");
```

### Extract text

```csharp
using var doc = PdfDocument.Open("document.pdf");
string text = doc.Pages[0].ExtractText();
```

### Search text with bounding rectangles

```csharp
using var doc = PdfDocument.Open("document.pdf");
using var textPage = doc.Pages[0].GetTextPage();

// Simple search
var results = textPage.Search("Hello");

// Search with bounding rectangles (for highlighting)
var resultsWithBounds = textPage.SearchWithBounds("Hello");
foreach (var r in resultsWithBounds)
{
    Console.WriteLine($"Found at index {r.StartIndex}, length {r.Length}");
    foreach (var rect in r.Rectangles)
        Console.WriteLine($"  Rect: {rect}");
}
```

### Extract text from a region

```csharp
using var textPage = doc.Pages[0].GetTextPage();
var region = new PdfRectangle(72, 700, 300, 750);
string regionText = textPage.GetTextInRegion(region);
```

### Render to PNG/BMP

```csharp
using var doc = PdfDocument.Open("document.pdf");
var page = doc.Pages[0];

// Render directly to PNG byte array
byte[] png = page.RenderToPng(dpi: 150);
File.WriteAllBytes("page.png", png);

// Or render to BMP
byte[] bmp = page.RenderToBmp(dpi: 150);

// Or use PdfBitmap for more control
using var bitmap = page.Render(dpi: 150);
using var stream = File.Create("page.png");
bitmap.SaveAsPng(stream);
```

### Document permissions & page labels

```csharp
using var doc = PdfDocument.Open("document.pdf");

// Check permissions
var perms = doc.Permissions;
Console.WriteLine($"Can print: {perms.HasFlag(PdfPermissions.Print)}");
Console.WriteLine($"Security handler revision: {doc.SecurityHandlerRevision}");

// Page labels
for (int i = 0; i < doc.PageCount; i++)
    Console.WriteLine($"Page {i}: label = \"{doc.GetPageLabel(i)}\"");
```

### Merge and extract pages

```csharp
// Merge documents
using var doc1 = PdfDocument.Open("part1.pdf");
using var doc2 = PdfDocument.Open("part2.pdf");
doc1.MergeFrom(doc2);
doc1.Save("merged.pdf");

// Extract specific pages
using var doc = PdfDocument.Open("large.pdf");
using var extracted = doc.ExtractPages(0, 2, 4);
extracted.Save("selected-pages.pdf");
```

### Annotations

```csharp
using var doc = PdfDocument.Open("document.pdf");
var page = doc.Pages[0];

// Add a text annotation
var annot = page.Annotations.Add(PdfAnnotationType.Text);
annot.SetRect(new PdfRectangle(100, 700, 130, 730));
annot.SetColor(new PdfColor(255, 255, 0)); // Yellow

page.GenerateContent();
doc.Save("annotated.pdf");
```

### Attachments

```csharp
using var doc = PdfDocument.Create();
doc.AddPage(PdfSize.A4).GenerateContent();

var attachment = doc.Attachments.Add("data.txt");
attachment.SetFile(System.Text.Encoding.UTF8.GetBytes("Hello!"));

doc.Save("with-attachment.pdf");
```

## Project Structure

```
src/
  PdfiumNet/          # High-level API
  PdfiumNet.Native/   # P/Invoke bindings
samples/
  PdfiumNet.Samples/  # Usage examples
tests/
  PdfiumNet.Tests/    # Unit & integration tests
runtimes/
  osx-arm64/native/   # Native library (macOS ARM64)
```

## Running Tests

```bash
dotnet test
```

Tests that require the PDFium native library are skipped automatically if it's not available.

## License

Apache-2.0
