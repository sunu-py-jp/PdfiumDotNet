using PdfiumNet.Native;

namespace PdfiumNet.StructTree;

/// <summary>
/// Represents the structure tree of a PDF page, providing access to tagged structure elements.
/// Useful for accessibility validation.
/// </summary>
public sealed class PdfStructTree : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    internal PdfStructTree(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Gets the structure tree for a page. Returns null if no structure tree exists.
    /// </summary>
    public static PdfStructTree? GetForPage(PdfPage page)
    {
        var handle = PdfiumNative.FPDF_StructTree_GetForPage(page.Handle);
        return handle == IntPtr.Zero ? null : new PdfStructTree(handle);
    }

    /// <summary>
    /// Gets the number of top-level children in the structure tree.
    /// </summary>
    public int ChildCount
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return PdfiumNative.FPDF_StructTree_CountChildren(_handle);
        }
    }

    /// <summary>
    /// Gets a top-level child element at the specified index.
    /// </summary>
    public PdfStructElement? GetChild(int index)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (index < 0 || index >= ChildCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        var handle = PdfiumNative.FPDF_StructTree_GetChildAtIndex(_handle, index);
        return handle == IntPtr.Zero ? null : new PdfStructElement(handle);
    }

    /// <summary>
    /// Gets all top-level children.
    /// </summary>
    public IReadOnlyList<PdfStructElement> GetChildren()
    {
        var count = ChildCount;
        var children = new List<PdfStructElement>(count);
        for (var i = 0; i < count; i++)
        {
            var child = GetChild(i);
            if (child != null)
                children.Add(child);
        }
        return children;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_handle != IntPtr.Zero)
        {
            PdfiumNative.FPDF_StructTree_Close(_handle);
            _handle = IntPtr.Zero;
        }
    }
}
