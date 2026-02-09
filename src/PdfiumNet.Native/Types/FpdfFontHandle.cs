using System.Runtime.InteropServices;

namespace PdfiumNet.Native.Types;

/// <summary>
/// Handle for FPDF_FONT. Fonts are owned by the document and should not be freed manually.
/// </summary>
public sealed class FpdfFontHandle : SafeHandle
{
    public FpdfFontHandle() : base(IntPtr.Zero, false) { }
    public FpdfFontHandle(IntPtr handle) : base(handle, false) { }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        // Fonts are owned by the document; do not free.
        return true;
    }
}
