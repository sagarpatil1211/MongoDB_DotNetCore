//name1space VJCore.Infrastructure;

public class LinePrintingParameters
{
    public int X1 = 0;
    public int Y1 = 0;

    public int X2 = 0;
    public int Y2 = 0;

    public int LineThickness = 1;

    public LinePrintingParameters(int x1, int y1, int x2, int y2, int lineThickness)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
        LineThickness = lineThickness;
    }

    public LinePrintingParameters CreateCopy()
    {
        return new LinePrintingParameters(X1, Y1, X2, Y2, LineThickness);
    }
}
