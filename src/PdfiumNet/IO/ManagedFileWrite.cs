using System.Runtime.InteropServices;
using PdfiumNet.Native.Types;

namespace PdfiumNet.IO;

/// <summary>
/// Bridges a managed Stream with the PDFium FPDF_FILEWRITE callback.
/// Must remain alive (not GC'd) for the duration of the save operation.
/// </summary>
internal sealed class ManagedFileWrite : IDisposable
{
    private readonly Stream _stream;
    private GCHandle _callbackHandle;
    private FpdfFileWrite _fileWrite;

    // Must match FPDF_FILEWRITE's WriteBlock signature:
    // int WriteBlock(FPDF_FILEWRITE* pThis, const void* data, unsigned long size)
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WriteBlockDelegate(IntPtr pThis, IntPtr data, uint size);

    public ManagedFileWrite(Stream stream)
    {
        _stream = stream;
        WriteBlockDelegate callback = WriteBlock;
        _callbackHandle = GCHandle.Alloc(callback);

        _fileWrite = new FpdfFileWrite
        {
            Version = 1,
            WriteBlock = Marshal.GetFunctionPointerForDelegate(callback)
        };
    }

    public ref FpdfFileWrite NativeStruct => ref _fileWrite;

    private int WriteBlock(IntPtr pThis, IntPtr data, uint size)
    {
        try
        {
            var buffer = new byte[size];
            Marshal.Copy(data, buffer, 0, (int)size);
            _stream.Write(buffer, 0, (int)size);
            return 1; // success
        }
        catch
        {
            return 0; // failure
        }
    }

    public void Dispose()
    {
        if (_callbackHandle.IsAllocated)
            _callbackHandle.Free();
    }
}
