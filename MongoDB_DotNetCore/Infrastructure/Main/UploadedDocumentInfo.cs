using System;
using System.Collections.Generic;
using System.Text;

public class UploadedDocumentInfo
{
    public UploadedDocumentInfo(int displayOrder, long documentTypeRef, string documentTypeName, string tag)
    {
        DisplayOrder = displayOrder;
        DocumentTypeRef = documentTypeRef;
        DocumentTypeName = documentTypeName;
        Tag = tag;
    }

    public int DisplayOrder { get; } = 0;
    public long DocumentTypeRef { get; } = 0L;
    public string DocumentTypeName { get; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
}
