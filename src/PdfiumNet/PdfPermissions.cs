namespace PdfiumNet;

/// <summary>
/// Represents PDF document permissions as defined in ISO 32000-1 Table 22.
/// </summary>
[Flags]
public enum PdfPermissions : uint
{
    None = 0,

    /// <summary>Bit 3: Print the document.</summary>
    Print = 1 << 2,

    /// <summary>Bit 4: Modify the contents of the document.</summary>
    ModifyContents = 1 << 3,

    /// <summary>Bit 5: Copy or otherwise extract text and graphics.</summary>
    ExtractText = 1 << 4,

    /// <summary>Bit 6: Add or modify text annotations, fill in interactive form fields.</summary>
    ModifyAnnotations = 1 << 5,

    /// <summary>Bit 9: Fill in existing interactive form fields.</summary>
    FillForms = 1 << 8,

    /// <summary>Bit 10: Extract text and graphics for accessibility.</summary>
    ExtractForAccessibility = 1 << 9,

    /// <summary>Bit 11: Assemble the document (insert, rotate, or delete pages and create bookmarks or thumbnail images).</summary>
    AssembleDocument = 1 << 10,

    /// <summary>Bit 12: Print the document to a representation from which a faithful digital copy could be generated.</summary>
    PrintHighQuality = 1 << 11,
}
