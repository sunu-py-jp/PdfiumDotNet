using System.Runtime.InteropServices;
using PdfiumNet.Native;
using PdfiumNet.Native.Types;

namespace PdfiumNet.Forms;

/// <summary>
/// Provides read-only access to form field information in a PDF document.
/// Must be disposed after use to release the form fill environment.
/// </summary>
public sealed class PdfFormInfo : IDisposable
{
    private readonly PdfDocument _document;
    private IntPtr _formHandle;
    private IntPtr _formInfoPtr; // Heap-allocated struct — must stay alive while form handle is in use
    private bool _disposed;

    internal PdfFormInfo(PdfDocument document)
    {
        _document = document;

        // Allocate the struct on the heap so it stays valid for the lifetime of the form handle
        var formInfo = FpdfFormFillInfo.CreateMinimal();
        _formInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf<FpdfFormFillInfo>());
        Marshal.StructureToPtr(formInfo, _formInfoPtr, false);

        _formHandle = PdfiumNative.FPDFDOC_InitFormFillEnvironment(document.Handle, _formInfoPtr);
    }

    /// <summary>
    /// Gets the form type of the document.
    /// </summary>
    public PdfFormType FormType => (PdfFormType)PdfiumNative.FPDF_GetFormType(_document.Handle);

    /// <summary>
    /// Gets whether the document has forms.
    /// </summary>
    public bool HasForms => FormType != PdfFormType.None;

    /// <summary>
    /// Enumerates all form fields across all pages.
    /// </summary>
    public IReadOnlyList<PdfFormField> GetFields()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var fields = new List<PdfFormField>();

        if (_formHandle == IntPtr.Zero || !HasForms)
            return fields;

        var pageCount = _document.PageCount;
        for (var pageIndex = 0; pageIndex < pageCount; pageIndex++)
        {
            var pageHandle = PdfiumNative.FPDF_LoadPage(_document.Handle, pageIndex);
            if (pageHandle == IntPtr.Zero) continue;

            try
            {
                var annotCount = PdfiumNative.FPDFPage_GetAnnotCount(pageHandle);
                for (var i = 0; i < annotCount; i++)
                {
                    var annotHandle = PdfiumNative.FPDFPage_GetAnnot(pageHandle, i);
                    if (annotHandle == IntPtr.Zero) continue;

                    try
                    {
                        var subtype = PdfiumNative.FPDFAnnot_GetSubtype(annotHandle);
                        if (subtype != 20) // FPDF_ANNOT_WIDGET = 20
                            continue;

                        var fieldType = (PdfFormFieldType)PdfiumNative.FPDFAnnot_GetFormFieldType(_formHandle, annotHandle);
                        var name = GetFormFieldString(annotHandle, true);
                        var value = GetFormFieldString(annotHandle, false);
                        var flags = PdfiumNative.FPDFAnnot_GetFormFieldFlags(_formHandle, annotHandle);

                        fields.Add(new PdfFormField(name, value, fieldType, flags, pageIndex));
                    }
                    finally
                    {
                        PdfiumNative.FPDFPage_CloseAnnot(annotHandle);
                    }
                }
            }
            finally
            {
                PdfiumNative.FPDF_ClosePage(pageHandle);
            }
        }

        return fields;
    }

    private string GetFormFieldString(IntPtr annotHandle, bool isName)
    {
        uint length;
        if (isName)
            length = PdfiumNative.FPDFAnnot_GetFormFieldName(_formHandle, annotHandle, IntPtr.Zero, 0);
        else
            length = PdfiumNative.FPDFAnnot_GetFormFieldValue(_formHandle, annotHandle, IntPtr.Zero, 0);

        if (length <= 2)
            return string.Empty;

        var buffer = Marshal.AllocHGlobal((int)length);
        try
        {
            if (isName)
                PdfiumNative.FPDFAnnot_GetFormFieldName(_formHandle, annotHandle, buffer, length);
            else
                PdfiumNative.FPDFAnnot_GetFormFieldValue(_formHandle, annotHandle, buffer, length);

            return Marshal.PtrToStringUni(buffer) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_formHandle != IntPtr.Zero)
        {
            PdfiumNative.FPDFDOC_ExitFormFillEnvironment(_formHandle);
            _formHandle = IntPtr.Zero;
        }
        if (_formInfoPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_formInfoPtr);
            _formInfoPtr = IntPtr.Zero;
        }
    }
}
