using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.SQLiteConnectivity;

public class DataRowPersistenceColumnMapper : IDBCAPersistenceColumnMapper
{
    private readonly DataRow dr = null;

    public DataRowPersistenceColumnMapper(DataRow dr)
    {
        this.dr = dr;
    }

    public T GetPropertyValueFromColumnName<T>(string columnName)
    {
        return (T)dr[columnName];
    }

    public void SetPropertyValueAtColumnName<T>(T value, string columnName, bool ignoreIfColumnNotFound = true)
    {
        if (dr.Table.Columns.Contains(columnName) == false && ignoreIfColumnNotFound == false)
            throw new Exception("Column '" + columnName + "' not found.");
        if (dr.Table.Columns.Contains(columnName) == true)
        {
            dr.BeginEdit();
            dr[columnName] = value;
            dr.EndEdit();
        }
    }

    public Dictionary<string, object> GetColumnNameAndValueCollection()
    {
        var values = new Dictionary<string, object>();
        foreach (DataColumn dc in dr.Table.Columns)
        {
            values.Add(dc.ColumnName, dr[dc.ColumnName]);
        }

        return values;
    }

    public void GenerateInsertCommand(SQLiteCommand cmd, string tableName)
    {
        var sb = new StringBuilder("INSERT INTO " + tableName + " (");
        var dtb = dr.Table;
        var loopTo = dr.Table.Columns.Count - 1;
        int i;

        for (i = 0; i <= loopTo; i++)
        {
            if (i == 0)
            {
                sb.Append(dtb.Columns[i].ColumnName);
            }
            else
            {
                sb.Append("," + dtb.Columns[i].ColumnName);
            }
        }

        sb.Append(") VALUES (");
        var loopTo1 = dr.Table.Columns.Count - 1;
        for (i = 0; i <= loopTo1; i++)
        {
            if (i == 0)
            {
                sb.Append("@" + dtb.Columns[i].ColumnName);
            }
            else
            {
                sb.Append(",@" + dtb.Columns[i].ColumnName);
            }
        }

        _ = sb.Append(')');
        cmd.CommandText = sb.ToString();
        cmd.Parameters.Clear();
        var loopTo2 = dr.Table.Columns.Count - 1;
        for (i = 0; i <= loopTo2; i++)
            cmd.Parameters.AddWithValue("@" + dtb.Columns[i].ColumnName, dr[i]);
    }
}
//}