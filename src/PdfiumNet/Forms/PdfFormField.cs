namespace PdfiumNet.Forms;

/// <summary>
/// Represents a read-only form field extracted from a PDF document.
/// </summary>
public readonly record struct PdfFormField(
    string Name,
    string Value,
    PdfFormFieldType FieldType,
    int Flags,
    int PageIndex);
