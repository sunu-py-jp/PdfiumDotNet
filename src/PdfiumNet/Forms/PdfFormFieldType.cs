namespace PdfiumNet.Forms;

/// <summary>
/// PDF form field types as returned by FPDFAnnot_GetFormFieldType.
/// </summary>
public enum PdfFormFieldType
{
    Unknown = -1,
    PushButton = 1,
    CheckBox = 2,
    RadioButton = 3,
    ComboBox = 4,
    ListBox = 5,
    TextField = 6,
    Signature = 7,
}
