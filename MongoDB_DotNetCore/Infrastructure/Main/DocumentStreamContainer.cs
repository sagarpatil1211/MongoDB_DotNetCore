using Microsoft.AspNetCore.Mvc;
using System.IO;

public class DocumentStreamContainer
{
    public DocumentStreamContainer(Stream documentStream, 
        string downloadedDocumentFileNameWithExtension)
    {
        DocumentStream = documentStream;
        DownloadedDocumentFileNameWithExtension = downloadedDocumentFileNameWithExtension;
    }

    public static DocumentStreamContainer Empty()
    {
        return new DocumentStreamContainer(Stream.Null, string.Empty);
    }

    public Stream DocumentStream { get; } = Stream.Null;
    public string DownloadedDocumentFileNameWithExtension { get; } = "Document.pdf";
    public string MimeType => MimeMapping.MimeUtility.GetMimeMapping(DownloadedDocumentFileNameWithExtension);
}
