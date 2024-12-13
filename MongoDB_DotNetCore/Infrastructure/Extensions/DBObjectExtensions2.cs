using System.Collections.Generic;
using System.Data.Common;
//using VJ1Core.Infrastructure.DbConnectionObjects;

namespace VJCore.Infrastructure.Extensions;

public static class DBObjectExtensions2
{
    public static DbParameter AddVarcharParameter(this DbCommand cmd, string parameterName,
        string dbObjectName, string columnName, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Varchar;
        p.Size = DataAccessUtils.GetVarcharFieldSize(dbObjectName, columnName);
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static string AddVarcharListParameter(this DbCommand cmd, string parameterName,
        string dbObjectName, string columnName, List<string> values)
    {
        List<string> result = new List<string>();

        int index = 0;

        foreach (string paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterName}{index}";
            cmd.AddVarcharParameter(indexedParamName, dbObjectName, columnName, paramValue);
            result.Add(indexedParamName);
        }

        return Utils.CombineListIntoSingleString(result, ",");
    }

    public static DbParameter AddVarcharParameter(this DbCommand cmd, string parameterName,
        string dbObjectName, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Varchar;

        string columnName = parameterName;
        if (columnName.StartsWith("@")) columnName = columnName[1..];

        p.Size = DataAccessUtils.GetVarcharFieldSize(dbObjectName, columnName);
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static string AddVarcharListParameter(this DbCommand cmd, string parameterName,
        string dbObjectName, List<string> values)
    {
        List<string> result = new List<string>();

        int index = 0;

        foreach (string paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterName}{index}";
            cmd.AddVarcharParameter(indexedParamName, dbObjectName, paramValue);
            result.Add(indexedParamName);
        }

        return Utils.CombineListIntoSingleString(result, ",");
    }

    public static string AddLongListParameter(this DbCommand cmd, string parameterName, List<long> values)
    {
        List<string> result = new List<string>();

        int index = 0;

        foreach (long paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterName}{index}";
            cmd.AddLongParameter(indexedParamName, paramValue);
            result.Add(indexedParamName);
        }

        return Utils.CombineListIntoSingleString(result, ",");
    }

    public static DbParameter AddDecimalParameter(this DbCommand cmd, string parameterName,
       string dbObjectName, decimal value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;

        var contTemplate = DataAccessUtils.GetTemplateDbParameterContainer(cmd, dbObjectName);
        DbParameterInfo pInfo = contTemplate.GetDbParameterInfo(parameterName);

        p.DbType = pInfo.DbType;
        p.Precision = pInfo.Precision;
        p.Scale = pInfo.Scale;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }
}
