using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Newtonsoft.Json.Linq;

public class InMemoryData : DataSet
{
    private InMemoryData() {}

    private static InMemoryData m_instance = null;

    static InMemoryData()
    {
        m_instance = new InMemoryData();
    }

    public static InMemoryData GetInstance()
    {
        return m_instance;
    }

    public List<string> UpdatedTableNames = new List<string>();

    public List<string> GetTableNames()
    {
        var result = new List<string>();

        foreach (DataTable dtb in Tables)
        {
            result.Add(dtb.TableName);
        }

        return result;
    }

    public DataCollection GetCollection(string tableName)
    {
        var coll = new DataCollection();

        if (!Tables.Contains(tableName)) return coll;

        DataAccessUtils.TransferDataTableDataToDataCollection(Tables[tableName], ref coll);

        return coll;
    }

    private static DataContainer m_dataContainer = new DataContainer();

    public static DataContainer DataContainer => m_dataContainer;
}
