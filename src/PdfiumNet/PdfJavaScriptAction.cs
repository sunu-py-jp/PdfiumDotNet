using PdfiumNet.Native;

namespace PdfiumNet;

/// <summary>
/// Represents a JavaScript action embedded in a PDF document.
/// Useful for security auditing to detect potentially malicious scripts.
/// </summary>
public sealed class PdfJavaScriptAction
{
    /// <summary>
    /// Gets the name of the JavaScript action.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the JavaScript source code.
    /// </summary>
    public string Script { get; }

    internal PdfJavaScriptAction(string name, string script)
    {
        Name = name;
        Script = script;
    }
}
