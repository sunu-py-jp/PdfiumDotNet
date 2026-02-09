using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

public sealed class FpdfSearchHandle : SafeHandle
{
    public FpdfSearchHandle() : base(IntPtr.Zero, true) { }
    public FpdfSearchHandle(IntPtr handle) : base(handle, true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        PdfiumNative.FPDFText_FindClose(handle);
        return true;
    }
}
