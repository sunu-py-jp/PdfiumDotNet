namespace PdfiumNet.Exceptions;

/// <summary>
/// Thrown when a PDF document requires a password to open.
/// </summary>
public class PdfPasswordRequiredException : PdfiumException
{
    public PdfPasswordRequiredException()
        : base("The PDF document requires a password to open.") { }
}
