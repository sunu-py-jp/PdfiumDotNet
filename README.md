# PdfiumDotNet

A C# wrapper for [Google PDFium](https://pdfium.googlesource.com/pdfium/) that provides a clean, high-level API for PDF creation, editing, text extraction, and drawing.

## Features

- PDF creation and page management
- Text drawing with standard/custom fonts
- Shape drawing (rectangle, circle, ellipse, line, polygon, custom path)
- Image insertion
- Text extraction and search
- Save to file, stream, or byte array

## Requirements

- .NET 7.0+
- PDFium native library (`libpdfium.dylib` / `pdfium.dll` / `libpdfium.so`)

### Getting the native library

```bash
# Download from https://github.com/bblanchon/pdfium-binaries/releases
# Or use the included script:
pwsh build/download-pdfium-binaries.ps1
```

Place the library in `runtimes/{rid}/native/` or next to your application binary.

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

canvas.SetStrokeColor(PdfColor.Red);
canvas.DrawCircle(400, 680, 30, DrawMode.Stroke);

page.GenerateContent();
doc.Save("output.pdf");
```

### Open and extract text

```csharp
using var doc = PdfDocument.Open("output.pdf");
string text = doc.Pages[0].ExtractText();
Console.WriteLine(text);
```

### Search text

```csharp
using var doc = PdfDocument.Open("output.pdf");
using var textPage = doc.Pages[0].GetTextPage();
var results = textPage.Search("Hello");
foreach (var r in results)
    Console.WriteLine($"Found at index {r.StartIndex}");
```

### Edit an existing PDF

```csharp
using var doc = PdfDocument.Open("output.pdf");
var canvas = doc.Pages[0].GetCanvas();

// Add a red strikethrough line
canvas.SetStrokeColor(PdfColor.Red);
canvas.SetStrokeWidth(2);
canvas.DrawLine(72, 760, 300, 760);

// Add correction text
canvas.SetFillColor(PdfColor.Red);
canvas.DrawText("CORRECTED", 310, 750, "Helvetica", 12);

doc.Pages[0].GenerateContent();
doc.Save("edited.pdf");
```

### Insert an image

```csharp
using var bitmap = PdfBitmap.Create(100, 100, hasAlpha: true);
bitmap.FillRect(0, 0, 100, 100, 0xFFFF0000); // red square

var canvas = page.GetCanvas();
canvas.DrawImage(bitmap, 72, 500, 100, 100);
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
```

## Running Tests

```bash
dotnet test
```

Tests that require the PDFium native library are skipped automatically if it's not available.

## License

Apache-2.0
