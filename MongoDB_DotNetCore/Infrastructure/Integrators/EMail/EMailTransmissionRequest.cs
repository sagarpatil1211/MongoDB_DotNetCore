using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class AttachmentFileStream
{
    public string FileName { get; set; } = string.Empty;
    public Stream FileStream { get; set; } = null;
    public ContentType ContentType { get; set; } = null;
}

public class EMailTransmissionRequest
{
    public Dictionary<string, string> Receivers { get; set; } = new Dictionary<string, string>();
    // Key = Friendly Name of Receiver
    // Value = EMail ID of receiver

    public string Subject { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
    public string HTMLBody { get; set; } = string.Empty;
    public List<string> AttachmentFilePaths { get; set; } = new List<string>();
    public List<AttachmentFileStream> AttachmentFileStreams { get; set; } = new List<AttachmentFileStream>();
    public bool IsBlank()
    {
        return TextBody.Trim().Length == 0 && HTMLBody.Trim().Length == 0;
    }

    public static readonly EMailTransmissionRequest Blank = null;

    static EMailTransmissionRequest()
    {
        Blank = new EMailTransmissionRequest();
    }
}
