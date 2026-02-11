using System.Runtime.InteropServices;
using PdfiumNet.Native;
using PdfiumNet.Native.Types;

namespace PdfiumNet.Forms;

/// <summary>
/// Provides access to form field information in a PDF document.
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

    /// <summary>
    /// Sets the value of a text field by field name.
    /// </summary>
    /// <param name="fieldName">The name of the form field.</param>
    /// <param name="value">The new value to set.</param>
    /// <returns>True if the value was set successfully.</returns>
    public bool SetFieldValue(string fieldName, string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_formHandle == IntPtr.Zero)
            return false;

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
                        if (subtype != 20) continue; // FPDF_ANNOT_WIDGET = 20

                        var name = GetFormFieldString(annotHandle, true);
                        if (!string.Equals(name, fieldName, StringComparison.Ordinal))
                            continue;

                        return PdfiumNative.FORM_SetFieldText(_formHandle, pageHandle, annotHandle, value);
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

        return false;
    }

    private string GetFormFieldString(IntPtr annotHandle, bool isName)
    {
        return NativeStringHelper.ReadUtf16((buf, len) =>
            isName
                ? PdfiumNative.FPDFAnnot_GetFormFieldName(_formHandle, annotHandle, buf, len)
                : PdfiumNative.FPDFAnnot_GetFormFieldValue(_formHandle, annotHandle, buf, len));
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
