using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

public sealed class FpdfTextPageHandle : SafeHandle
{
    public FpdfTextPageHandle() : base(IntPtr.Zero, true) { }
    public FpdfTextPageHandle(IntPtr handle) : base(handle, true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        PdfiumNative.FPDFText_ClosePage(handle);
        return true;
    }
}
