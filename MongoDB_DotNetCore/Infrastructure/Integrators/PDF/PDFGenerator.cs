//using Syncfusion.Pdf;
//using Syncfusion.Pdf.Graphics;
//using System.IO;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Linq;
////using VJ1Core.Infrastructure;

//public class PDFGenerator
//{
//    public static FileStreamResult GeneratePdfFileStreamResult(List<PrintPage> lstPages, string fileName,
//        int pageHeight = 0, int pageWidth = 0, PdfPageOrientation orientation = PdfPageOrientation.Portrait,
//        int shiftX = 0, int shiftY = 0)
//    {
//        var document = GeneratePdfDocument(lstPages, fileName, pageHeight, pageWidth, orientation, shiftX, shiftY);
//        return ConvertPDFDocumentToFileStreamResult(document, fileName);
//    }

//    public static PdfDocument GeneratePdfDocument(List<PrintPage> lstPages, string fileName, 
//        int pageHeight = 0, int pageWidth = 0, PdfPageOrientation orientation = PdfPageOrientation.Portrait,
//        int shiftX = 0, int shiftY = 0)
//    {
//        PdfDocument document = new PdfDocument();

//        if (lstPages.Count > 0)
//        {
//            var ppFirst = lstPages.First();

//            if (pageHeight == 0) pageHeight = ppFirst.PageHeight;
//            if (pageWidth == 0) pageWidth = ppFirst.PageWidth;

//            document.PageSettings = new PdfPageSettings { Height = pageHeight, Width = pageWidth, Orientation = orientation };
//        }

//        foreach (PrintPage pp in lstPages)
//        {
//            PdfPage page = document.Pages.Add();
//            PdfGraphics graphics = page.Graphics;

//            pp.ShiftInSamePage(shiftX, shiftY);

//            pp.DrawPage(graphics);
//        }

//        return document;
//    }

//    public static FileStreamResult ConvertPDFDocumentToFileStreamResult(PdfDocument doc, string fileName = "")
//    {
//        MemoryStream stream = new MemoryStream();

//        doc.Save(stream);

//        doc.Close();

//        stream.Position = 0;

//        FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf")
//        {
//            FileDownloadName = fileName
//        };

//        return fileStreamResult;
//    }

//    public static FileStreamResult CombinePDFDocumentsIntoFileStreamResult(List<PdfDocument> documents, string fileName = "")
//    {
//        var docResult = CombinePDFDocumentsIntoSingleDocument(documents, fileName);
//        return ConvertPDFDocumentToFileStreamResult(docResult, fileName);
//    }

//    public static PdfDocument CombinePDFDocumentsIntoSingleDocument(List<PdfDocument> documents, string fileName = "")
//    {
//        PdfDocument docResult = new PdfDocument();

//        foreach (var doc in documents)
//        {
//            docResult.PageSettings = doc.PageSettings;
//            break;
//        }

//        int pageIndex = -1;

//        foreach (var doc in documents)
//        {
//            foreach (PdfPage page in doc.Pages)
//            {
//                pageIndex++;

//                docResult.Pages.Insert(pageIndex, page);
//            }
//        }

//        return docResult;
//    }
//}
