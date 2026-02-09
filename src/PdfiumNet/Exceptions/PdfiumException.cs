namespace PdfiumNet.Exceptions;

/// <summary>
/// Base exception for PDFium operations.
/// </summary>
public class PdfiumException : Exception
{
    public PdfiumException(string message) : base(message) { }
    public PdfiumException(string message, Exception innerException) : base(message, innerException) { }

    internal static PdfiumException FromLastError()
    {
        var errorCode = Native.PdfiumNative.FPDF_GetLastError();
        var message = errorCode switch
        {
            1 => "Unknown error",
            2 => "File not found or could not be opened",
            3 => "File not in PDF format or corrupted",
            4 => "Password required or incorrect password",
            5 => "Unsupported security scheme",
            6 => "Page not found or content error",
            _ => $"PDFium error code: {errorCode}"
        };
        if (errorCode == 4)
            return new PdfPasswordRequiredException();
        return new PdfiumException(message);
    }
}
