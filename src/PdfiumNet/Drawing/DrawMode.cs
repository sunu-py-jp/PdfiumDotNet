namespace PdfiumNet.Drawing;

/// <summary>
/// Specifies how a path should be rendered.
/// </summary>
[Flags]
public enum DrawMode
{
    /// <summary>No fill, no stroke.</summary>
    None = 0,

    /// <summary>Stroke the path outline.</summary>
    Stroke = 1,

    /// <summary>Fill the path using the non-zero winding rule.</summary>
    Fill = 2,

    /// <summary>Fill the path using the even-odd rule.</summary>
    FillEvenOdd = 4,

    /// <summary>Fill and stroke the path.</summary>
    FillAndStroke = Fill | Stroke,
}
