namespace PdfiumNet;

/// <summary>
/// Flags for controlling PDF page rendering behavior.
/// </summary>
[Flags]
public enum RenderFlags
{
    /// <summary>No special rendering flags.</summary>
    None = 0,

    /// <summary>Render annotations.</summary>
    Annotations = 0x01,

    /// <summary>Use LCD-optimized text rendering.</summary>
    LcdText = 0x02,

    /// <summary>Do not use native text output.</summary>
    NoNativeText = 0x04,

    /// <summary>Render in grayscale.</summary>
    Grayscale = 0x08,

    /// <summary>Render for printing.</summary>
    Printing = 0x800,
}
