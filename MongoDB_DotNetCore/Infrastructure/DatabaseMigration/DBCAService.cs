using Microsoft.VisualBasic;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class DBCAService
{
    public static string FormatQuery(string q)
    {
        string qTemp = q;
        qTemp = qTemp.Replace(ControlChars.NewLine, Strings.Space(1));
        while (qTemp.Contains(Strings.Space(2), System.StringComparison.CurrentCulture))
        {
            qTemp = qTemp.Replace(Strings.Space(2), Strings.Space(1));
        }

        return qTemp;
    }

    public static string FormatTableCreationQuery(string q)
    {
        string qTemp = FormatQuery(q);
        return qTemp;
    }

    public static string FormatViewCreationQuery(string q)
    {
        string qTemp = FormatQuery(q);
        return qTemp;
    }
}
