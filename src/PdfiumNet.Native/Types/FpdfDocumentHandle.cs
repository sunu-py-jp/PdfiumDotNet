using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

public sealed class FpdfDocumentHandle : SafeHandle
{
    public FpdfDocumentHandle() : base(IntPtr.Zero, true) { }
    public FpdfDocumentHandle(IntPtr handle) : base(handle, true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        PdfiumNative.FPDF_CloseDocument(handle);
        return true;
    }
}
