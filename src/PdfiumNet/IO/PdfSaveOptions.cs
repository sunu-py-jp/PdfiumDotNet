namespace PdfiumNet.IO;

/// <summary>
/// Options for saving PDF documents.
/// </summary>
public sealed class PdfSaveOptions
{
    /// <summary>
    /// Default save options.
    /// </summary>
    public static readonly PdfSaveOptions Default = new();

    /// <summary>
    /// Save flags. Default is no incremental save.
    /// </summary>
    public PdfSaveFlags Flags { get; init; } = PdfSaveFlags.NoIncremental;

    /// <summary>
    /// PDF file version (e.g. 17 for PDF 1.7). 0 means use the document's existing version.
    /// </summary>
    public int FileVersion { get; init; }
}

public enum PdfSaveFlags : uint
{
    Incremental = 1,
    NoIncremental = 2,
    RemoveSecurity = 3,
}
