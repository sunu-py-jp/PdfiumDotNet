using System.Runtime.InteropServices;
using PdfiumNet.Exceptions;
using PdfiumNet.Geometry;
using PdfiumNet.Native;

namespace PdfiumNet.Text;

/// <summary>
/// Provides text extraction and search operations on a PDF page.
/// </summary>
public sealed class PdfTextPage : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    internal PdfTextPage(PdfPage page)
    {
        _handle = PdfiumNative.FPDFText_LoadPage(page.Handle);
        if (_handle == IntPtr.Zero)
            throw new PdfiumException("Failed to load text page.");
    }

    internal IntPtr Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _handle;
        }
    }

    /// <summary>
    /// Gets the total number of characters on the page.
    /// </summary>
    public int CharacterCount => PdfiumNative.FPDFText_CountChars(Handle);

    /// <summary>
    /// Extracts all text from the page.
    /// </summary>
    public string GetText() => GetText(0, CharacterCount);

    /// <summary>
    /// Extracts text starting from the specified index.
    /// </summary>
    public string GetText(int startIndex, int count)
    {
        if (count <= 0) return string.Empty;

        // FPDFText_GetText writes UTF-16LE including null terminator
        var bufferSize = (count + 1) * 2;
        var buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            var charsWritten = PdfiumNative.FPDFText_GetText(Handle, startIndex, count, buffer);
            if (charsWritten <= 0) return string.Empty;
            // charsWritten includes null terminator
            return Marshal.PtrToStringUni(buffer, charsWritten - 1) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Gets the Unicode character at the specified index.
    /// </summary>
    public char GetCharacter(int index)
    {
        var unicode = PdfiumNative.FPDFText_GetUnicode(Handle, index);
        return (char)unicode;
    }

    /// <summary>
    /// Gets the bounding box of a character.
    /// </summary>
    public PdfRectangle GetCharacterBox(int index)
    {
        if (!PdfiumNative.FPDFText_GetCharBox(Handle, index,
            out var left, out var right, out var bottom, out var top))
            return default;
        return new PdfRectangle((float)left, (float)bottom, (float)right, (float)top);
    }

    /// <summary>
    /// Gets the origin point of a character.
    /// </summary>
    public PdfPoint GetCharacterOrigin(int index)
    {
        if (!PdfiumNative.FPDFText_GetCharOrigin(Handle, index, out var x, out var y))
            return default;
        return new PdfPoint((float)x, (float)y);
    }

    /// <summary>
    /// Gets the font size of a character in points.
    /// </summary>
    public double GetFontSize(int index) => PdfiumNative.FPDFText_GetFontSize(Handle, index);

    /// <summary>
    /// Gets the font name of a character.
    /// </summary>
    public string? GetFontName(int index)
    {
        // First call to get required buffer size
        var size = PdfiumNative.FPDFText_GetFontInfo(Handle, index, IntPtr.Zero, 0, out _);
        if (size == 0) return null;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            PdfiumNative.FPDFText_GetFontInfo(Handle, index, buffer, size, out _);
            return Marshal.PtrToStringUTF8(buffer);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Gets detailed information for all characters on the page.
    /// </summary>
    public IReadOnlyList<CharacterInfo> GetAllCharacterInfo()
    {
        var count = CharacterCount;
        var result = new List<CharacterInfo>(count);

        for (var i = 0; i < count; i++)
        {
            var ch = GetCharacter(i);
            // Skip control characters
            if (char.IsControl(ch) && ch != '\n' && ch != '\r') continue;

            result.Add(new CharacterInfo
            {
                Index = i,
                Character = ch,
                BoundingBox = GetCharacterBox(i),
                Origin = GetCharacterOrigin(i),
                FontSize = GetFontSize(i),
                FontName = GetFontName(i)
            });
        }

        return result;
    }

    /// <summary>
    /// Gets the bounding rectangles for a range of characters.
    /// </summary>
    public IReadOnlyList<PdfRectangle> GetTextRectangles(int startIndex, int count)
    {
        var rectCount = PdfiumNative.FPDFText_CountRects(Handle, startIndex, count);
        if (rectCount <= 0)
            return Array.Empty<PdfRectangle>();

        var rects = new List<PdfRectangle>(rectCount);
        for (var i = 0; i < rectCount; i++)
        {
            if (PdfiumNative.FPDFText_GetRect(Handle, i, out var left, out var top, out var right, out var bottom))
                rects.Add(new PdfRectangle((float)left, (float)bottom, (float)right, (float)top));
        }
        return rects;
    }

    /// <summary>
    /// Searches for text on the page and returns results with bounding rectangles.
    /// </summary>
    public IReadOnlyList<TextSearchResult> SearchWithBounds(string text, bool caseSensitive = false, bool wholeWord = false)
    {
        uint flags = 0;
        if (caseSensitive) flags |= 0x0001;
        if (wholeWord) flags |= 0x0002;

        var results = new List<TextSearchResult>();
        var searchHandle = PdfiumNative.FPDFText_FindStart(Handle, text, flags, 0);
        if (searchHandle == IntPtr.Zero)
            return results;

        try
        {
            while (PdfiumNative.FPDFText_FindNext(searchHandle))
            {
                var index = PdfiumNative.FPDFText_GetSchResultIndex(searchHandle);
                var count = PdfiumNative.FPDFText_GetSchCount(searchHandle);
                var rects = GetTextRectangles(index, count);
                results.Add(new TextSearchResult { StartIndex = index, Length = count, Rectangles = rects });
            }
        }
        finally
        {
            PdfiumNative.FPDFText_FindClose(searchHandle);
        }

        return results;
    }

    /// <summary>
    /// Searches for text on the page.
    /// </summary>
    public IReadOnlyList<TextSearchResult> Search(string text, bool caseSensitive = false, bool wholeWord = false)
    {
        uint flags = 0;
        if (caseSensitive) flags |= 0x0001;
        if (wholeWord) flags |= 0x0002;

        var results = new List<TextSearchResult>();
        var searchHandle = PdfiumNative.FPDFText_FindStart(Handle, text, flags, 0);
        if (searchHandle == IntPtr.Zero)
            return results;

        try
        {
            while (PdfiumNative.FPDFText_FindNext(searchHandle))
            {
                var index = PdfiumNative.FPDFText_GetSchResultIndex(searchHandle);
                var count = PdfiumNative.FPDFText_GetSchCount(searchHandle);
                results.Add(new TextSearchResult { StartIndex = index, Length = count });
            }
        }
        finally
        {
            PdfiumNative.FPDFText_FindClose(searchHandle);
        }

        return results;
    }

    /// <summary>
    /// Extracts text within the specified rectangular region.
    /// </summary>
    public string GetTextInRegion(PdfRectangle region)
    {
        // First call to get character count
        var charCount = PdfiumNative.FPDFText_GetBoundedText(Handle,
            region.Left, region.Top, region.Right, region.Bottom,
            IntPtr.Zero, 0);
        if (charCount <= 0)
            return string.Empty;

        // Second call to get actual text (charCount includes null terminator)
        var bufferSize = charCount * 2;
        var buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            PdfiumNative.FPDFText_GetBoundedText(Handle,
                region.Left, region.Top, region.Right, region.Bottom,
                buffer, charCount);
            return Marshal.PtrToStringUni(buffer, charCount - 1) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Gets the character index at the specified position.
    /// Returns -1 if no character is found at the position, -3 if the position is outside the page.
    /// </summary>
    public int GetCharIndexAtPosition(float x, float y, float xTolerance = 10f, float yTolerance = 10f)
    {
        return PdfiumNative.FPDFText_GetCharIndexAtPos(Handle, x, y, xTolerance, yTolerance);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_handle != IntPtr.Zero)
        {
            PdfiumNative.FPDFText_ClosePage(_handle);
            _handle = IntPtr.Zero;
        }
    }
}
