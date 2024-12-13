using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ClassService
{
    public static void InitializeDataRow(DataRow bRow)
    {
        bRow.BeginEdit();

        foreach (DataColumn dc in bRow.Table.Columns)
        {
            if (bRow[dc.ColumnName] is DBNull)
            {
                if (dc.DataType.FullName == typeof(string).FullName)
                {
                    bRow[dc.ColumnName] = string.Empty;
                }
                else
                {
                    bRow[dc.ColumnName] = 0;
                }
            }
        }

        bRow.EndEdit();
    }

    public static void TransferDataBetweenRows(DataRow drFrom, DataRow drTo)
    {
        drTo.BeginEdit();

        foreach (DataColumn dc in drTo.Table.Columns)
        {
            if (!dc.ReadOnly)
            {
                if (drFrom.Table.Columns.Contains(dc.ColumnName)) drTo[dc.ColumnName] = drFrom[dc.ColumnName];
            }
        }

        drTo.EndEdit();
    }

    public static void AddRowToTable(DataTable table, DataRow dr)
    {
        DataRow nRow = table.NewRow();
        InitializeDataRow(nRow);
        TransferDataBetweenRows(dr, nRow);
        table.Rows.Add(nRow);
    }

    public static JObject InitializeDataObject(DataTable dtbStructure)
    {
        var result = new JObject();

        foreach(DataColumn dc in dtbStructure.Columns)
        {
            if (dc.DataType.FullName == typeof(String).FullName)
            {
                result.Add(dc.ColumnName, string.Empty);
            }
            else
            {
                result.Add(dc.ColumnName, 0);
            }
        }

        return result;
        
        //foreach(var prop in obj.Properties())
        //{
        //    var propType = obj[prop.Name].GetType().FullName;

        //    if (propType == typeof(string).FullName) obj[prop.Name] = string.Empty;
        //    else if (propType == typeof(int).FullName) obj[prop.Name] = 0;
        //    else if (propType == typeof(short).FullName) obj[prop.Name] = 0;
        //    else if (propType == typeof(long).FullName) obj[prop.Name] = 0;
        //    else if (propType == typeof(uint).FullName) obj[prop.Name] = 0;
        //    else if (propType == typeof(ushort).FullName) obj[prop.Name] = 0;
        //    else if (propType == typeof(ulong).FullName) obj[prop.Name] = 0;
        //    else if (propType == typeof(byte[]).FullName) obj[prop.Name] = new byte[] { };
        //}
    }

    public static void SanitizeDataObject(DataTable dtbStructure, JObject obj)
    {
        if (dtbStructure == null) return;

        foreach (var prop in obj.Properties())
        {
            var propType = obj[prop.Name].GetType().FullName;

            if (obj[prop.Name].Type == JTokenType.Null)
            {
                var dc = dtbStructure.Columns[prop.Name];
                if (dc != null)
                {
                    if (dc.DataType.FullName == typeof(String).FullName)
                    {
                        obj[prop.Name] = string.Empty;
                    }
                    else
                    {
                        obj[prop.Name] = 0;
                    }
                }
            }
        }
    }

    public static bool CheckMatch(JObject sourceData, JObject targetData, params string[] keyNamesToMatch)
    {
        foreach (string keyName in keyNamesToMatch)
        {
            foreach (string strKeyName in keyName.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var kn = strKeyName.Trim();
                if (!(sourceData[kn].Equals(targetData[kn]))) return false;
            }
        }

        return true;
    }

    public static JObject GetMatchingData(IList<JObject> list, string keys, params object[] values)
    {
        var keyArray = keys.Split(',');

        if (keyArray.Length != values.Length) return null;

        foreach (var data in list)
        {
            bool matchFound = true;

            for (int i = 0; i < keyArray.Length; i++)
            {
                if (!(data[keyArray[i]].ToObject(typeof(object)).Equals(values[i])))
                {
                    matchFound = false;
                    break;
                }
            }

            if (matchFound) return data;
        }

        return null;
    }

    public static void TransferData(JObject fromObject, JObject toObject)
    {
        if (fromObject.GetHashCode() == toObject.GetHashCode()) return;

        //var toProps = toObject.Properties().Select(p => p.Name).ToList();
        //toProps.ForEach(pName => toObject.Remove(pName));

        foreach (var prop in fromObject.Properties())
        {
            //if (toObject.ContainsKey(prop.Name))
            //{
                toObject[prop.Name] = fromObject[prop.Name];
            //}
            //else
            //{
            //    toObject.Add(prop.Name, prop.Value);
            //}
        }
    }

    public static DataRow GetFirstDataRowByFilterString(DataTable dtb, string filterString)
    {
        foreach (DataRow dr in dtb.Select(filterString))
        {
            return dr;
        }

        return null;
    }

    public static void RemoveDataRowsByFilterString(DataTable dtb, string filterString)
    {
        bool allRemoved = false;

        while (!allRemoved)
        {
            allRemoved = true;

            foreach (DataRow dr in dtb.Select(filterString))
            {
                dtb.Rows.Remove(dr);
                allRemoved = false;
            }
        }
    }
}
