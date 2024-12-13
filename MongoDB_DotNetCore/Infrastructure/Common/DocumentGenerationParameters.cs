//using Syncfusion.DocIO.DLS;
//using Syncfusion.Drawing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
////using VJ1Core.Infrastructure.Main;

////name1space VJCore.Infrastructure;

//public class TextParameters
//{
//    public string Text { get; set; } = null;
//    public StringAlignment? TextStringAlignment { get; set; } = null;
//    public Font Font { get; set; } = null;
//    public bool? Bold { get; set; } = null;
//    public bool? Italic { get; set; } = null;
//    public bool? Underline { get; set; } = null;
//    public bool? Strikeout { get; set; } = null;
//    public int? RowSpan { get; set; } = null;
//    public int? ColumnSpan { get; set; } = null;
//    public Color? TextColor { get; set; } = null;

//    public VerticalAlignment? TextVerticalAlignment
//    {
//        get
//        {
//            return TextStringAlignment switch
//            {
//                StringAlignment.TopLeft => (VerticalAlignment?)VerticalAlignment.Top,
//                StringAlignment.TopCenter => (VerticalAlignment?)VerticalAlignment.Top,
//                StringAlignment.TopRight => (VerticalAlignment?)VerticalAlignment.Top,
//                StringAlignment.MiddleLeft => (VerticalAlignment?)VerticalAlignment.Middle,
//                StringAlignment.MiddleCenter => (VerticalAlignment?)VerticalAlignment.Middle,
//                StringAlignment.MiddleRight => (VerticalAlignment?)VerticalAlignment.Middle,
//                StringAlignment.BottomLeft => (VerticalAlignment?)VerticalAlignment.Bottom,
//                StringAlignment.BottomCenter => (VerticalAlignment?)VerticalAlignment.Bottom,
//                StringAlignment.BottomRight => (VerticalAlignment?)VerticalAlignment.Bottom,
//                _ => null,
//            };
//        }
//    }

//    public HorizontalAlignment? TextHorizontalAlignment
//    {
//        get
//        {
//            return TextStringAlignment switch
//            {
//                StringAlignment.TopLeft => (HorizontalAlignment?)HorizontalAlignment.Left,
//                StringAlignment.MiddleLeft => (HorizontalAlignment?)HorizontalAlignment.Left,
//                StringAlignment.BottomLeft => (HorizontalAlignment?)HorizontalAlignment.Left,
//                StringAlignment.TopCenter => (HorizontalAlignment?)HorizontalAlignment.Center,
//                StringAlignment.MiddleCenter => (HorizontalAlignment?)HorizontalAlignment.Center,
//                StringAlignment.BottomCenter => (HorizontalAlignment?)HorizontalAlignment.Center,
//                StringAlignment.TopRight => (HorizontalAlignment?)HorizontalAlignment.Right,
//                StringAlignment.MiddleRight => (HorizontalAlignment?)HorizontalAlignment.Right,
//                StringAlignment.BottomRight => (HorizontalAlignment?)HorizontalAlignment.Right,
//                _ => null,
//            };
//        }
//    }
//}


//public class ImageParameters
//{
//    public string PlaceHolder { get; set; } = string.Empty;
//    public byte[] ImageBytes { get; set; } = Array.Empty<byte>();
//}


//public class CellBorderDefinition
//{
//    public BorderStyle BorderType { get; set; } = BorderStyle.None;
//    public Color Color { get; set; } = Color.Transparent;
//    public float LineWidth { get; set; } = 0.0f;
//    public bool Shadow { get; set; } = false;
//    public float Space { get; set; } = 0.0f;
//}

//public class CellBorderCollection
//{
//    public CellBorderDefinition LeftBorder { get; set; } = null;
//    public CellBorderDefinition TopBorder { get; set; } = null;
//    public CellBorderDefinition RightBorder { get; set; } = null;
//    public CellBorderDefinition BottomBorder { get; set; } = null;

//    public void ApplyToCell(WTableCell cell)
//    {
//        if (LeftBorder != null) TransferBorderProperties(LeftBorder, cell.CellFormat.Borders.Left);
//        if (TopBorder != null) TransferBorderProperties(TopBorder, cell.CellFormat.Borders.Top);
//        if (RightBorder != null) TransferBorderProperties(RightBorder, cell.CellFormat.Borders.Right);
//        if (BottomBorder != null) TransferBorderProperties(BottomBorder, cell.CellFormat.Borders.Bottom);
//    }

//    private static void TransferBorderProperties(CellBorderDefinition sourceBorder, Border destinationBorder)
//    {
//        destinationBorder.BorderType = sourceBorder.BorderType;
//        destinationBorder.Color = sourceBorder.Color;
//        destinationBorder.LineWidth = sourceBorder.LineWidth;
//        destinationBorder.Shadow = sourceBorder.Shadow;
//        destinationBorder.Space = sourceBorder.Space;
//    }
//}

//public class DocumentGenerationTableParameters
//{
//    public string TableMarker { get; set; } = string.Empty;
//    private System.Collections.Generic.SortedDictionary<int, Dictionary<int, TextParameters>> HeaderStrings { get; } = new System.Collections.Generic.SortedDictionary<int, Dictionary<int, TextParameters>>();
//    private System.Collections.Generic.SortedDictionary<int, Dictionary<int, TextParameters>> CellValues { get; } = new System.Collections.Generic.SortedDictionary<int, Dictionary<int, TextParameters>>();
//    private System.Collections.Generic.SortedDictionary<int, Dictionary<int, CellBorderCollection>> CellBorderCollections { get; } = new System.Collections.Generic.SortedDictionary<int, Dictionary<int, CellBorderCollection>>();
//    private Dictionary<int, int> DesiredRowHeights { get; } = new Dictionary<int, int>();
//    private List<int> RowIndicesToRemove { get; } = new List<int>();

//    public void IncludeRowIndexInTableHeader(int rowIndex)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) HeaderStrings.Add(rowIndex, new Dictionary<int, TextParameters>());
//    }

//    public void RemoveRow(int rowIndex)
//    {
//        if (!RowIndicesToRemove.Contains(rowIndex)) RowIndicesToRemove.Add(rowIndex);
//        RowIndicesToRemove.Sort(new Comparison<int>((a, b) => b - a));
//    }

//    public void SetHeaderString(int rowIndex, int columnIndex, string text, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) HeaderStrings.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = HeaderStrings[rowIndex];

//        dict[columnIndex] = new TextParameters { Text = text, Bold = bold, Italic = italic,
//        Underline = underline, Strikeout = strikeout};
//    }

//    public void SetHeaderString(int rowIndex, int columnIndex, string text, StringAlignment stringAlignment, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) HeaderStrings.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = HeaderStrings[rowIndex];

//        dict[columnIndex] = new TextParameters { Text = text, TextStringAlignment = stringAlignment,
//            Bold = bold,
//            Italic = italic,
//            Underline = underline,
//            Strikeout = strikeout
//        };
//    }

//    public void SetHeaderString(int rowIndex, int columnIndex, string text, StringAlignment stringAlignment, Font font, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) HeaderStrings.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = HeaderStrings[rowIndex];

//        dict[columnIndex] = new TextParameters { Text = text, TextStringAlignment = stringAlignment, Font = font,
//            Bold = bold,
//            Italic = italic,
//            Underline = underline,
//            Strikeout = strikeout
//        };
//    }

//    public void SetHeaderString(int rowIndex, int columnIndex, string text, StringAlignment stringAlignment, Font font, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null, Color? textColor = null)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) HeaderStrings.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = HeaderStrings[rowIndex];

//        dict[columnIndex] = new TextParameters
//        {
//            Text = text,
//            TextStringAlignment = stringAlignment,
//            Font = font,
//            Bold = bold,
//            Italic = italic,
//            Underline = underline,
//            Strikeout = strikeout,
//            TextColor = textColor
//        };
//    }

//    public List<int> GetRowIndicesToRemove()
//    {
//        return RowIndicesToRemove;
//    }

//    public List<int> GetHeaderColumnIndices(int rowIndex)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) return new List<int>();
//        var dict = HeaderStrings[rowIndex];

//        return dict.Keys.ToList();
//    }

//    public TextParameters GetHeaderTextParameters(int rowIndex, int columnIndex)
//    {
//        if (!HeaderStrings.ContainsKey(rowIndex)) return new TextParameters();
//        var dict = HeaderStrings[rowIndex];

//        if (!dict.ContainsKey(columnIndex)) return new TextParameters();
//        return dict[columnIndex];
//    }

//    public void SetCellString(int rowIndex, int columnIndex, string text, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null, int? rowSpan = null, int? columnSpan = null,
//        Color? textColor = null)
//    {
//        if (!CellValues.ContainsKey(rowIndex)) CellValues.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = CellValues[rowIndex];
//        dict[columnIndex] = new TextParameters
//        {
//            Text = text,
//            Bold = bold,
//            Italic = italic,
//            Underline = underline,
//            Strikeout = strikeout,
//            RowSpan = rowSpan,
//            ColumnSpan = columnSpan,
//            TextColor = textColor
//        };
//    }

//    public void SetCellString(int rowIndex, int columnIndex, string text, StringAlignment stringAlignment, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null, int? rowSpan = null, int? columnSpan = null,
//        Color? textColor = null)
//    {
//        if (!CellValues.ContainsKey(rowIndex)) CellValues.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = CellValues[rowIndex];
//        dict[columnIndex] = new TextParameters { Text = text, TextStringAlignment = stringAlignment,
//            Bold = bold,
//            Italic = italic,
//            Underline = underline,
//            Strikeout = strikeout,
//            RowSpan = rowSpan,
//            ColumnSpan = columnSpan,
//            TextColor = textColor
//        };
//    }

//    public void SetCellString(int rowIndex, int columnIndex, string text, StringAlignment stringAlignment, Font font, bool? bold = null, bool? italic = null,
//        bool? underline = null, bool? strikeout = null, int? rowSpan = null, int? columnSpan = null,
//        Color? textColor = null)
//    {
//        if (!CellValues.ContainsKey(rowIndex)) CellValues.Add(rowIndex, new Dictionary<int, TextParameters>());
//        var dict = CellValues[rowIndex];
//        dict[columnIndex] = new TextParameters { Text = text, TextStringAlignment = stringAlignment, Font = font,
//            Bold = bold,
//            Italic = italic,
//            Underline = underline,
//            Strikeout = strikeout,
//            RowSpan = rowSpan,
//            ColumnSpan = columnSpan,
//            TextColor = textColor
//        };
//    }

//    public void SetCellBorders(int rowIndex, int columnIndex, CellBorderCollection borders)
//    {
//        if (!CellBorderCollections.ContainsKey(rowIndex)) CellBorderCollections.Add(rowIndex, new Dictionary<int, CellBorderCollection>());

//        var dict = CellBorderCollections[rowIndex];
//        dict[columnIndex] = borders;
//    }

//    public CellBorderCollection GetCellBorders(int rowIndex, int columnIndex)
//    {
//        if (CellBorderCollections.ContainsKey(rowIndex))
//        {
//            var dict = CellBorderCollections[rowIndex];
//            if (dict.ContainsKey(columnIndex))
//            {
//                return dict[columnIndex];
//            }
//        }

//        return null;
//    }

//    public List<int> GetHeaderRowIndices()
//    {
//        var result = HeaderStrings.Keys.ToList();
//        return result;
//    }

//    public List<int> GetRowIndices()
//    {
//        var result = CellValues.Keys.ToList();
//        return result;
//    }

//    public List<int> GetColumnIndices(int rowIndex)
//    {
//        if (!CellValues.ContainsKey(rowIndex)) return new List<int>();
//        var dict = CellValues[rowIndex];
//        return dict.Keys.ToList();
//    }

//    public TextParameters GetCellTextParameters(int rowIndex, int columnIndex)
//    {
//        if (!CellValues.ContainsKey(rowIndex)) return new TextParameters();
//        var dict = CellValues[rowIndex];
//        if (!dict.ContainsKey(columnIndex)) return new TextParameters();
//        return dict[columnIndex];
//    }

//    public void SetDesiredRowHeight(int rowIndex, int height)
//    {
//        DesiredRowHeights[rowIndex] = height;
//    }

//    public int GetDesiredRowHeight(int rowIndex)
//    {
//        if (!DesiredRowHeights.ContainsKey(rowIndex)) return -1;
//        return DesiredRowHeights[rowIndex];
//    }
//}

//public class DocumentGenerationParameters
//{
//    public string BackgroundImageFilePath { get; private set; } = string.Empty;
//    public byte[] BackgroundImageByteArray { get; private set; } = null;

//    private Dictionary<string, TextParameters> OpenFieldValues { get; } = new();

//    private readonly Dictionary<string, DocumentGenerationTableParameters> TableParameters = new();

//    private readonly Dictionary<string, ImageParameters> ImageParameters = new();

//    private readonly List<string> TableMarkersMarkedForRemoval = new();

//    public DocumentGenerationParameters() {}

//    public string TemplateWordFileName { get; set; } = string.Empty;

//    public void SetBackgroundImageFilePath(string path)
//    {
//        BackgroundImageFilePath = path;
//    }

//    public void SetBackgroundImageByteArray(byte[] bArray)
//    {
//        BackgroundImageByteArray = bArray;
//    }

//    public void SetImage(string placeHolder, byte[] imageBytes)
//    {
//        ImageParameters iParams = new()
//        {
//            PlaceHolder = placeHolder, ImageBytes = imageBytes
//        };

//        ImageParameters[placeHolder] = iParams;
//    }

//    public void MarkTableForRemoval(string tableMarker)
//    {
//        if (!TableMarkersMarkedForRemoval.Contains(tableMarker))
//        {
//            TableMarkersMarkedForRemoval.Add(tableMarker);
//        }
//    }

//    public void SetOpenFieldValue(string marker, string value)
//    {
//        OpenFieldValues[marker] = new TextParameters { Text = value };
//    }

//    public void SetOpenFieldValue(string marker, string value, StringAlignment stringAlignment)
//    {
//        OpenFieldValues[marker] = new TextParameters { Text = value, TextStringAlignment= stringAlignment };
//    }

//    public void SetOpenFieldValue(string marker, string value, StringAlignment stringAlignment, Font font)
//    {
//        OpenFieldValues[marker] = new TextParameters { Text = value, TextStringAlignment = stringAlignment, Font = font };
//    }

//    public void SetOpenFieldValue(string marker, string value, StringAlignment stringAlignment, Font font, Color textColor)
//    {
//        OpenFieldValues[marker] = new TextParameters { Text = value, TextStringAlignment = stringAlignment, 
//            Font = font, TextColor = textColor };
//    }

//    public List<string> GetOpenFieldMarkers()
//    {
//        return OpenFieldValues.Keys.ToList();
//    }

//    public TextParameters GetOpenFieldValue(string marker)
//    {
//        if (OpenFieldValues.ContainsKey(marker)) return OpenFieldValues[marker];
//        return null;
//    }

//    public void SetTableParameters(string tableMarker, DocumentGenerationTableParameters parameters)
//    {
//        TableParameters[tableMarker] = parameters;
//    }

//    public List<string> GetTableMarkers()
//    {
//        return TableParameters.Keys.ToList();
//    }

//    public List<string> GetTableMarkersMarkedForRemoval()
//    {
//        return TableMarkersMarkedForRemoval;
//    }

//    public DocumentGenerationTableParameters GetTableParameters(string tableMarker)
//    {
//        if (TableParameters.ContainsKey(tableMarker)) return TableParameters[tableMarker];
//        return null;
//    }

//    public List<string> GetImagePlaceholders()
//    {
//        return ImageParameters.Keys.ToList();
//    }

//    public ImageParameters GetImageParameter(string placeHolder)
//    {
//        if (ImageParameters.ContainsKey(placeHolder)) return ImageParameters[placeHolder];
//        return null;
//    }
//}
