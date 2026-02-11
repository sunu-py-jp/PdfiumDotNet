using System.Runtime.InteropServices;
using PdfiumNet.Native;

namespace PdfiumNet.Attachments;

/// <summary>
/// Represents a file attachment embedded in a PDF document.
/// </summary>
public sealed class PdfAttachment
{
    private readonly IntPtr _handle;
    private readonly PdfDocument _document;

    internal PdfAttachment(PdfDocument document, IntPtr handle)
    {
        _document = document;
        _handle = handle;
    }

    /// <summary>
    /// Gets the name of the attachment.
    /// </summary>
    public string Name => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDFAttachment_GetName(_handle, buf, len));

    /// <summary>
    /// Gets a string value for the given key (e.g., "CreationDate", "CheckSum").
    /// </summary>
    public string GetStringValue(string key)
    {
        return NativeStringHelper.ReadUtf16((buf, len) =>
            PdfiumNative.FPDFAttachment_GetStringValue(_handle, key, buf, len));
    }

    /// <summary>
    /// Gets whether the attachment has a given key.
    /// </summary>
    public bool HasKey(string key) => PdfiumNative.FPDFAttachment_HasKey(_handle, key);

    /// <summary>
    /// Sets the file contents of this attachment.
    /// </summary>
    public bool SetFile(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            fixed (byte* ptr = data)
            {
                return PdfiumNative.FPDFAttachment_SetFile(_handle, _document.Handle,
                    (IntPtr)ptr, (uint)data.Length);
            }
        }
    }

    /// <summary>
    /// Gets the file contents of this attachment.
    /// </summary>
    public byte[]? GetFile()
    {
        if (!PdfiumNative.FPDFAttachment_GetFile(_handle, IntPtr.Zero, 0, out var requiredLen))
            return null;

        if (requiredLen == 0)
            return Array.Empty<byte>();

        var buffer = Marshal.AllocHGlobal((int)requiredLen);
        try
        {
            if (!PdfiumNative.FPDFAttachment_GetFile(_handle, buffer, requiredLen, out _))
                return null;

            var result = new byte[requiredLen];
            Marshal.Copy(buffer, result, 0, (int)requiredLen);
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
