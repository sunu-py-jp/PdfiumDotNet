using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

public sealed class FpdfBitmapHandle : SafeHandle
{
    public FpdfBitmapHandle() : base(IntPtr.Zero, true) { }
    public FpdfBitmapHandle(IntPtr handle) : base(handle, true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        PdfiumNative.FPDFBitmap_Destroy(handle);
        return true;
    }
}
