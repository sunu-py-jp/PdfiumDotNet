using PdfiumNet;
using PdfiumNet.Drawing;
using PdfiumNet.Geometry;
using PdfiumNet.Objects;
using Xunit;
using Xunit.Abstractions;

namespace PdfiumNet.Tests;

public class PdfTableEditTests
{
    private static readonly string OutputDir = Path.Combine(
        Path.GetDirectoryName(typeof(PdfTableEditTests).Assembly.Location)!,
        "..", "..", "..", "..", "..", "test-output");

    private readonly ITestOutputHelper _output;

    public PdfTableEditTests(ITestOutputHelper output)
    {
        _output = output;
        Directory.CreateDirectory(OutputDir);
    }

    private string GetOutputPath(string f) => Path.GetFullPath(Path.Combine(OutputDir, f));

    private static bool TryInit()
    {
        try { PdfiumLibrary.Initialize(); return true; } catch { return false; }
    }

    private static readonly float[] CW = { 40, 90, 160, 70, 130 };
    private static readonly string[] CH = { "No", "Code", "Name", "Price", "Note" };
    private static readonly string[][] Rows =
    {
        new[] { "1", "A-1001", "Ballpoint Pen",  "150", "Standard" },
        new[] { "2", "A-1002", "Notebook B5",     "280", "Popular" },
        new[] { "3", "B-2001", "Eraser",          "80",  "" },
        new[] { "4", "B-2002", "Mech Pencil",     "350", "New" },
        new[] { "5", "C-3001", "Clear Folder",    "120", "10pcs" },
        new[] { "6", "C-3002", "Sticky Notes",    "450", "5 colors" },
        new[] { "7", "D-4001", "Correction Tape", "220", "" },
        new[] { "8", "D-4002", "Stapler",         "580", "w/ staples" },
    };
    private const float ML = 50, TT = 750, RH = 22, PX = 5, PY = 6, FS = 9;

    [SkippableFact]
    public void Test1_CreateTablePdf()
    {
        Skip.IfNot(TryInit(), "no pdfium");
        var file = GetOutputPath("Table_01_Original.pdf");
        var pdf = BuildTablePdf();
        File.WriteAllBytes(file, pdf);

        using var v = PdfDocument.Open(file);
        var txt = v.Pages[0].ExtractText();
        Assert.Contains("Ballpoint", txt);
        Assert.Contains("580", txt);
        _output.WriteLine($"Saved: {file}");
    }

    [SkippableFact]
    public void Test2_StrikethroughAndCorrect()
    {
        Skip.IfNot(TryInit(), "no pdfium");
        var file = GetOutputPath("Table_02_Strikethrough.pdf");
        var pdf = BuildTableWithCorrections();
        File.WriteAllBytes(file, pdf);

        using var v = PdfDocument.Open(file);
        var txt = v.Pages[0].ExtractText();
        Assert.Contains("300", txt);
        Assert.Contains("380", txt);
        Assert.Contains("500", txt);
        _output.WriteLine($"Saved: {file}");
    }

    [SkippableFact]
    public void Test3_AddHankoStamp()
    {
        Skip.IfNot(TryInit(), "no pdfium");
        var file = GetOutputPath("Table_03_WithHanko.pdf");

        var src = BuildTableWithCorrections();
        using var doc = PdfDocument.Open(src);
        var cv = doc.Pages[0].GetCanvas();

        using var bmp = CreateHanko(80);
        float hy = TT - RH * (Rows.Length + 1) - 70;
        cv.DrawImage(bmp, 430, hy, 55, 55);
        doc.Pages[0].GenerateContent();

        using var ms = new MemoryStream();
        doc.Save(ms);
        File.WriteAllBytes(file, ms.ToArray());

        using var v = PdfDocument.Open(file);
        var objs = v.Pages[0].Objects;
        bool found = false;
        for (int i = 0; i < objs.Count; i++)
            if (objs[i].ObjectType == PdfPageObjectType.Image) { found = true; break; }
        Assert.True(found, "Should contain hanko image");
        _output.WriteLine($"Saved: {file}");
    }

    // ── Builders ────────────────────────────────────────────────────

    private static byte[] BuildTablePdf()
    {
        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var cv = page.GetCanvas();
        cv.SetFillColor(PdfColor.Black);
        cv.DrawText("Product List", ML, TT + 30, "Helvetica", 18);
        DrawTable(cv);
        page.GenerateContent();
        using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }

    private static byte[] BuildTableWithCorrections()
    {
        using var doc = PdfDocument.Create();
        var page = doc.AddPage(PdfSize.A4);
        var cv = page.GetCanvas();
        cv.SetFillColor(PdfColor.Black);
        cv.DrawText("Product List", ML, TT + 30, "Helvetica", 18);
        DrawTable(cv);

        float px = ML + CW[0] + CW[1] + CW[2];
        float pw = CW[3];
        foreach (var (row, np) in new[] { (1, "300"), (3, "380"), (5, "500") })
        {
            float ry = TT - RH - (row + 1) * RH;
            cv.SetStrokeColor(PdfColor.Red);
            cv.SetStrokeWidth(1.5f);
            cv.DrawLine(px + 2, ry + RH / 2, px + pw - 2, ry + RH / 2);
            cv.SetFillColor(PdfColor.Red);
            cv.DrawText(np, px + pw + 4, ry + PY, "Helvetica", FS);
        }

        page.GenerateContent();
        using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }

    private static void DrawTable(PdfCanvas cv)
    {
        float tw = 0;
        foreach (var w in CW) tw += w;
        int nrows = Rows.Length + 1;

        // Header bg
        cv.SetFillColor(new PdfColor(220, 220, 220));
        cv.SetStrokeColor(new PdfColor(220, 220, 220));
        cv.SetStrokeWidth(0.1f);
        cv.DrawRectangle(ML + 0.5f, TT - RH + 0.5f, tw - 1, RH - 1, DrawMode.FillAndStroke);

        // Grid
        cv.SetStrokeColor(PdfColor.Black);
        for (int r = 0; r <= nrows; r++)
        {
            cv.SetStrokeWidth(r <= 1 || r == nrows ? 1f : 0.5f);
            cv.DrawLine(ML, TT - r * RH, ML + tw, TT - r * RH);
        }
        float x = ML;
        for (int c = 0; c <= CW.Length; c++)
        {
            cv.SetStrokeWidth(c == 0 || c == CW.Length ? 1f : 0.5f);
            cv.DrawLine(x, TT, x, TT - nrows * RH);
            if (c < CW.Length) x += CW[c];
        }

        // Header
        cv.SetFillColor(PdfColor.Black);
        x = ML;
        for (int c = 0; c < CH.Length; c++)
        {
            cv.DrawText(CH[c], x + PX, TT - RH + PY, "Helvetica", 10);
            x += CW[c];
        }

        // Data
        for (int r = 0; r < Rows.Length; r++)
        {
            float ry = TT - RH - (r + 1) * RH;
            x = ML;
            for (int c = 0; c < CW.Length; c++)
            {
                cv.SetFillColor(PdfColor.Black);
                if (Rows[r][c].Length > 0)
                    cv.DrawText(Rows[r][c], x + PX, ry + PY, "Helvetica", FS);
                x += CW[c];
            }
        }
    }

    private static PdfBitmap CreateHanko(int sz)
    {
        var bmp = PdfBitmap.Create(sz, sz, true);
        bmp.FillRect(0, 0, sz, sz, 0x00000000);
        float cx = sz / 2f, cy = sz / 2f, R = sz / 2f - 2, ri = R - 3;
        unsafe
        {
            byte* p = (byte*)bmp.Buffer;
            int stride = bmp.Stride;
            for (int y = 0; y < sz; y++)
            for (int x = 0; x < sz; x++)
            {
                float dx = x - cx, dy = y - cy;
                float d = MathF.Sqrt(dx * dx + dy * dy);
                bool draw = d <= R && d >= ri;
                if (d < ri)
                {
                    float od = MathF.Sqrt((x - sz * 0.35f) * (x - sz * 0.35f) + (y - cy) * (y - cy));
                    if (od <= sz * 0.15f && od >= sz * 0.15f - 3) draw = true;
                    float kx0 = sz * 0.58f;
                    if (x >= kx0 && x <= kx0 + 3 && y > sz * 0.35f && y < sz * 0.65f) draw = true;
                    float kx1 = kx0 + 3;
                    if (x > kx1 && x < kx1 + sz * 0.12f)
                    {
                        if (MathF.Abs(y - (cy - (x - kx1))) < 2) draw = true;
                        if (MathF.Abs(y - (cy + (x - kx1))) < 2) draw = true;
                    }
                }
                if (draw)
                {
                    int off = y * stride + x * 4;
                    p[off] = 30; p[off + 1] = 30; p[off + 2] = 200; p[off + 3] = 220;
                }
            }
        }
        return bmp;
    }
}
