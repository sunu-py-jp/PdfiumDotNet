using System.Collections;
using PdfiumNet.Native;

namespace PdfiumNet.Annotations;

/// <summary>
/// Collection of annotations on a PDF page. Supports enumeration, adding, and removing.
/// </summary>
public sealed class PdfAnnotationCollection : IReadOnlyList<PdfAnnotation>
{
    private readonly PdfPage _page;

    internal PdfAnnotationCollection(PdfPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Gets the number of annotations on the page.
    /// </summary>
    public int Count => PdfiumNative.FPDFPage_GetAnnotCount(_page.Handle);

    /// <summary>
    /// Gets the annotation at the specified index. The caller must dispose the returned annotation.
    /// </summary>
    public PdfAnnotation this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var handle = PdfiumNative.FPDFPage_GetAnnot(_page.Handle, index);
            if (handle == IntPtr.Zero)
                throw new Exceptions.PdfiumException($"Failed to get annotation at index {index}.");
            return new PdfAnnotation(handle);
        }
    }

    /// <summary>
    /// Creates and adds a new annotation of the specified subtype.
    /// The caller must dispose the returned annotation.
    /// </summary>
    public PdfAnnotation Add(PdfAnnotationSubtype subtype)
    {
        var handle = PdfiumNative.FPDFPage_CreateAnnot(_page.Handle, (int)subtype);
        if (handle == IntPtr.Zero)
            throw new Exceptions.PdfiumException($"Failed to create annotation of type {subtype}.");
        return new PdfAnnotation(handle);
    }

    /// <summary>
    /// Removes the annotation at the specified index.
    /// </summary>
    public bool Remove(int index)
    {
        return PdfiumNative.FPDFPage_RemoveAnnot(_page.Handle, index);
    }

    public IEnumerator<PdfAnnotation> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            var handle = PdfiumNative.FPDFPage_GetAnnot(_page.Handle, i);
            if (handle != IntPtr.Zero)
                yield return new PdfAnnotation(handle);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
