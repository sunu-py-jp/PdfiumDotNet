using System.Runtime.InteropServices;
using PdfiumNet.Native;

namespace PdfiumNet.Signatures;

/// <summary>
/// Represents a digital signature in a PDF document (read-only).
/// </summary>
public sealed class PdfSignature
{
    private readonly IntPtr _handle;

    internal PdfSignature(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Gets the reason for the signature, if available.
    /// </summary>
    public string Reason => NativeStringHelper.ReadUtf16((buf, len) =>
        PdfiumNative.FPDFSignatureObj_GetReason(_handle, buf, len));

    /// <summary>
    /// Gets the signing time string, if available.
    /// </summary>
    public string? Time
    {
        get
        {
            var size = PdfiumNative.FPDFSignatureObj_GetTime(_handle, IntPtr.Zero, 0);
            if (size == 0) return null;

            var buffer = Marshal.AllocHGlobal((int)size);
            try
            {
                PdfiumNative.FPDFSignatureObj_GetTime(_handle, buffer, size);
                return Marshal.PtrToStringUTF8(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    /// <summary>
    /// Gets the sub-filter of the signature (e.g., "adbe.pkcs7.detached").
    /// </summary>
    public string? SubFilter
    {
        get
        {
            var size = PdfiumNative.FPDFSignatureObj_GetSubFilter(_handle, IntPtr.Zero, 0);
            if (size == 0) return null;

            var buffer = Marshal.AllocHGlobal((int)size);
            try
            {
                PdfiumNative.FPDFSignatureObj_GetSubFilter(_handle, buffer, size);
                return Marshal.PtrToStringUTF8(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    /// <summary>
    /// Gets the raw signature contents (the PKCS#7 or CMS data).
    /// </summary>
    public byte[]? GetContents()
    {
        var size = PdfiumNative.FPDFSignatureObj_GetContents(_handle, IntPtr.Zero, 0);
        if (size == 0) return null;

        var buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            PdfiumNative.FPDFSignatureObj_GetContents(_handle, buffer, size);
            var result = new byte[size];
            Marshal.Copy(buffer, result, 0, (int)size);
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Gets the DocMDP permission level. 0 means no restriction.
    /// </summary>
    public uint DocMdpPermission => PdfiumNative.FPDFSignatureObj_GetDocMDPPermission(_handle);
}
