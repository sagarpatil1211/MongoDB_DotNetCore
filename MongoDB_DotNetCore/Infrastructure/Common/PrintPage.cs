//using Syncfusion.Drawing;
//using Syncfusion.Pdf.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
////using VJ1Core.Infrastructure.Main;

////name1space VJCore.Infrastructure;

//public class PrintPage
//{
//    private List<StringPrintingParameters> alStringPrintingParameters = null;
//    private List<LinePrintingParameters> alLinePrintingParameters = null;
//    private List<ImagePrintingParameters> alImagePrintingParameters = null;

//    public string PageNo = string.Empty;
//    public int TopMargin = 0;
//    public int BottomMargin = 0;
//    public int LeftMargin = 0;
//    public int RightMargin = 0;
//    public PrintingOrientations PrintingOrientation = global::PrintingOrientations.Portrait;

//    public int XOffset = 0;
//    public int YOffset = 0;

//    private int m_customPageWidth = 750;
//    private int m_customPageHeight = 1100;

//    public bool CustomPageSize = false;

//    public int PageWidth
//    {
//        get => m_customPageWidth;
//    }

//    public int PageHeight
//    {
//        get => m_customPageHeight;
//    }

//    public PrintPage(string pageNo = "", int topMargin = 0, int bottomMargin = 0, int leftMargin = 0,
//        int rightMargin = 0, PrintingOrientations printingOrientation = global::PrintingOrientations.None)
//    {
//        alStringPrintingParameters = new List<StringPrintingParameters>();
//        alLinePrintingParameters = new List<LinePrintingParameters>();
//        alImagePrintingParameters = new List<ImagePrintingParameters>();

//        PageNo = pageNo;
//        TopMargin = topMargin;
//        BottomMargin = bottomMargin;
//        LeftMargin = leftMargin;
//        RightMargin = rightMargin;
//        PrintingOrientation = printingOrientation;
//    }

//    public void AddString(string _printString, PdfFont _printFont, int _x, int _y, PdfStringFormat _printStringFormat,
//        int _maxWidth = 0, int _maxHeight = 0)
//    {
//        if (_printFont == null) throw new ArgumentException("Font not specified.");
//        alStringPrintingParameters.Add(new StringPrintingParameters(_printString, _printFont, _x + XOffset, _y + YOffset, _printStringFormat, _maxWidth, _maxHeight));
//    }

//    public void AddLine(int x1, int y1, int x2, int y2, int lineThickness)
//    {
//        alLinePrintingParameters.Add(new LinePrintingParameters(x1 + XOffset, y1 + YOffset, x2 + XOffset, y2 + YOffset, lineThickness));
//    }

//    public void AddImage(PdfImage img, int x, int y, int width, int height, bool considerOffsets = true)
//    {
//        if (considerOffsets)
//        {
//            alImagePrintingParameters.Add(new ImagePrintingParameters(img, x + XOffset, y + YOffset, width, height));
//        } else {
//            alImagePrintingParameters.Add(new ImagePrintingParameters(img, x, y, width, height));
//        }
//    }

//    private static void DrawString(PdfGraphics g, StringPrintingParameters spp)
//    {
//        if (spp.MaxHeight == 0 && spp.MaxWidth == 0)
//        {
//            g.DrawString(spp.PrintString, spp.PrintFont, PdfBrushes.Black, spp.X, spp.Y, spp.PrintStringFormat);
//        }
//        else
//        {
//            var r = new RectangleF(spp.X, spp.Y, spp.MaxWidth, spp.MaxHeight);
//            g.DrawString(spp.PrintString, spp.PrintFont, PdfBrushes.Black, r, spp.PrintStringFormat);
//        }
//    }

//    private static void DrawLine(PdfGraphics g, LinePrintingParameters lpp)
//    {
//        g.DrawLine(new PdfPen(Color.Black, lpp.LineThickness), lpp.X1, lpp.Y1, lpp.X2, lpp.Y2);
//    }

//    private static void DrawImage(PdfGraphics g, ImagePrintingParameters ipp)
//    {
//        g.DrawImage(ipp.Img, ipp.X, ipp.Y, ipp.Width, ipp.Height);
//    }

//    public void DrawPage(PdfGraphics g)
//    {
//        foreach (ImagePrintingParameters ipp in alImagePrintingParameters)
//        {
//            DrawImage(g, ipp);
//        }

//        foreach (LinePrintingParameters lpp in alLinePrintingParameters)
//        {
//            DrawLine(g, lpp);
//        }

//        foreach (StringPrintingParameters spp in alStringPrintingParameters)
//        {
//            DrawString(g, spp);
//        }
//    }

//    public List<StringPrintingParameters> GetAllStringPrintingParameters()
//    {
//        return alStringPrintingParameters;
//    }

//    public void CopyInSamePage(int xOffset, int yOffset)
//    {
//        var alSPPTemp = new List<StringPrintingParameters>();
//        var alLPPTemp = new List<LinePrintingParameters>();
//        var alIPPTemp = new List<ImagePrintingParameters>();

//        foreach (StringPrintingParameters spp in alStringPrintingParameters)
//        {
//            alSPPTemp.Add(new StringPrintingParameters(spp.PrintString, spp.PrintFont, spp.X + xOffset, spp.Y + yOffset, spp.PrintStringFormat));
//        }

//        foreach (LinePrintingParameters lpp in alLinePrintingParameters)
//        {
//            alLPPTemp.Add(new LinePrintingParameters(lpp.X1 + xOffset, lpp.Y1 + yOffset, lpp.X2 + xOffset, lpp.Y2 + yOffset, lpp.LineThickness));
//        }

//        foreach (ImagePrintingParameters ipp in alImagePrintingParameters)
//        {
//            alIPPTemp.Add(new ImagePrintingParameters(ipp.Img, ipp.X, ipp.Y, ipp.Width, ipp.Height));
//        }

//        alStringPrintingParameters.AddRange(alSPPTemp);
//        alLinePrintingParameters.AddRange(alLPPTemp);
//        alImagePrintingParameters.AddRange(alIPPTemp);
//    }

//    public void ShiftInSamePage(int xOffset, int yOffset)
//    {
//        foreach (StringPrintingParameters spp in alStringPrintingParameters)
//        {
//            spp.X += xOffset;
//            spp.Y += yOffset;
//        }

//        foreach (LinePrintingParameters lpp in alLinePrintingParameters)
//        {
//            lpp.X1 += xOffset;
//            lpp.Y1 += yOffset;
//            lpp.X2 += xOffset;
//            lpp.Y2 += yOffset;
//        }

//        foreach (ImagePrintingParameters ipp in alImagePrintingParameters)
//        {
//            ipp.X += xOffset;
//            ipp.Y += yOffset;
//        }
//    }

//    public void ReplaceString(string originalString, string newString)
//    {
//        var spp = alStringPrintingParameters.Find(sppCheck => sppCheck.PrintString == originalString);
//        if (spp != null) spp.PrintString = newString;
//    }

//    public PrintPage CreateCopy()
//    {
//        var result = new PrintPage
//        {
//            alStringPrintingParameters = alStringPrintingParameters.Select(spp => spp.CreateCopy()).ToList(),
//            alLinePrintingParameters = alLinePrintingParameters.Select(lpp => lpp.CreateCopy()).ToList(),
//            alImagePrintingParameters = alImagePrintingParameters.Select(ipp => ipp.CreateCopy()).ToList(),
//            PageNo = this.PageNo,
//            TopMargin = this.TopMargin,
//            BottomMargin = this.BottomMargin,
//            LeftMargin = this.LeftMargin,
//            RightMargin = this.RightMargin,
//            PrintingOrientation = this.PrintingOrientation,

//            CustomPageSize = this.CustomPageSize,

//            m_customPageHeight = this.m_customPageHeight,
//            m_customPageWidth = this.m_customPageWidth
//        };

//        return result;
//    }
//}
