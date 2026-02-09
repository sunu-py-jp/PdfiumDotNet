using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

public sealed class FpdfPageHandle : SafeHandle
{
    public FpdfPageHandle() : base(IntPtr.Zero, true) { }
    public FpdfPageHandle(IntPtr handle) : base(handle, true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        PdfiumNative.FPDF_ClosePage(handle);
        return true;
    }
}
