using PdfiumNet.Drawing;
using PdfiumNet.Native;
using PdfiumNet.Native.Types;

namespace PdfiumNet.Annotations;

/// <summary>
/// Represents a single annotation on a PDF page.
/// Must be disposed after use to release the native annotation handle.
/// </summary>
public sealed class PdfAnnotation : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    internal PdfAnnotation(IntPtr handle)
    {
        _handle = handle;
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
    /// Gets the subtype of this annotation.
    /// </summary>
    public PdfAnnotationSubtype Subtype => (PdfAnnotationSubtype)PdfiumNative.FPDFAnnot_GetSubtype(Handle);

    /// <summary>
    /// Gets the annotation rectangle.
    /// </summary>
    public FsRectF Rect
    {
        get
        {
            PdfiumNative.FPDFAnnot_GetRect(Handle, out var rect);
            return rect;
        }
    }

    /// <summary>
    /// Sets the annotation rectangle.
    /// </summary>
    public bool SetRect(float left, float bottom, float right, float top)
    {
        var rect = new FsRectF(left, bottom, right, top);
        return PdfiumNative.FPDFAnnot_SetRect(Handle, ref rect);
    }

    /// <summary>
    /// Gets the annotation flags.
    /// </summary>
    public int Flags => PdfiumNative.FPDFAnnot_GetFlags(Handle);

    /// <summary>
    /// Sets the annotation flags.
    /// </summary>
    public bool SetFlags(int flags) => PdfiumNative.FPDFAnnot_SetFlags(Handle, flags);

    /// <summary>
    /// Sets the annotation color. Type: 0 = color, 1 = interior color.
    /// </summary>
    public bool SetColor(PdfColor color, int colorType = 0)
    {
        return PdfiumNative.FPDFAnnot_SetColor(Handle, colorType, color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Gets the annotation color. Type: 0 = color, 1 = interior color.
    /// </summary>
    public PdfColor? GetColor(int colorType = 0)
    {
        if (!PdfiumNative.FPDFAnnot_GetColor(Handle, colorType, out var r, out var g, out var b, out var a))
            return null;
        return new PdfColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Sets a string value for the annotation (e.g., "Contents", "Author").
    /// </summary>
    public bool SetStringValue(string key, string value)
    {
        return PdfiumNative.FPDFAnnot_SetStringValue(Handle, key, value);
    }

    /// <summary>
    /// Gets a string value from the annotation.
    /// </summary>
    public string GetStringValue(string key)
    {
        return NativeStringHelper.ReadUtf16((buf, len) =>
            PdfiumNative.FPDFAnnot_GetStringValue(Handle, key, buf, len));
    }

    /// <summary>
    /// Appends attachment points for text markup annotations (highlight, underline, etc.).
    /// </summary>
    public bool AppendAttachmentPoints(FsQuadPointsF quadPoints)
    {
        return PdfiumNative.FPDFAnnot_AppendAttachmentPoints(Handle, ref quadPoints);
    }

    /// <summary>
    /// Sets the border style.
    /// </summary>
    public bool SetBorder(float horizontalRadius, float verticalRadius, float borderWidth)
    {
        return PdfiumNative.FPDFAnnot_SetBorder(Handle, horizontalRadius, verticalRadius, borderWidth);
    }

    /// <summary>
    /// Sets the URI for a link annotation.
    /// </summary>
    public bool SetUri(string uri)
    {
        return PdfiumNative.FPDFAnnot_SetURI(Handle, uri);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_handle != IntPtr.Zero)
        {
            PdfiumNative.FPDFPage_CloseAnnot(_handle);
            _handle = IntPtr.Zero;
        }
    }
}
