namespace PdfiumNet.Annotations;

/// <summary>
/// PDF annotation subtypes as defined by the PDF specification.
/// Values match the FPDF_ANNOT_* constants in PDFium.
/// </summary>
public enum PdfAnnotationSubtype
{
    Unknown = 0,
    Text = 1,
    Link = 2,
    FreeText = 3,
    Line = 4,
    Square = 5,
    Circle = 6,
    Polygon = 7,
    Polyline = 8,
    Highlight = 9,
    Underline = 10,
    Squiggly = 11,
    Strikeout = 12,
    Stamp = 13,
    Caret = 14,
    Ink = 15,
    Popup = 16,
    FileAttachment = 17,
    Sound = 18,
    Movie = 19,
    Widget = 20,
    Screen = 21,
    PrinterMark = 22,
    TrapNet = 23,
    Watermark = 24,
    ThreeD = 25,
    RichMedia = 26,
    XfaWidget = 27,
    Redact = 28,
}
