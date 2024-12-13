using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
//using VJ1Core.Infrastructure.DbConnectionObjects;

namespace VJCore.Infrastructure.Extensions;

public static class DBObjectExtensions
{
    public static DbParameter AddParameterWithValue(this DbCommand cmd, string parameterName, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddVarcharParameter(this DbCommand cmd, string parameterName, int size, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Varchar;
        p.Size = size;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddVarcharMaxParameter(this DbCommand cmd, string parameterName, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Varchar;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddNVarcharParameter(this DbCommand cmd, string parameterName, int size, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_NVarchar;
        p.Size = size;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddIntegerParameter(this DbCommand cmd, string parameterName, int value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Int;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddEnumMemberParameter<T>(this DbCommand cmd, string parameterName, T value) where T : Enum
    {
        return cmd.AddIntegerParameter(parameterName, Convert.ToInt32(value));
    }

    public static string AddEnumMemberListParameter<T>(this DbCommand cmd, string parameterName, List<T> values) where T : Enum
    {
        List<string> result = new List<string>();

        int index = 0;

        foreach (T paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterName}{index}";
            cmd.AddEnumMemberParameter(indexedParamName, paramValue);
            result.Add(indexedParamName);
        }

        return CombineListIntoSingleString(result, ",");
    }

    private static string CombineListIntoSingleString(List<string> lst, string combinator)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string str in lst)
        {
            if (sb.Length > 0) sb.Append(combinator);
            sb.Append(str);
        }

        return sb.ToString();
    }

    public static DbParameter AddLongParameter(this DbCommand cmd, string parameterName, long value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Long;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddDecimalParameter(this DbCommand cmd, string parameterName,
        byte precision, byte scale, decimal value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Decimal;
        p.Precision = precision;
        p.Scale = scale;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static DbParameter AddBooleanParameter(this DbCommand cmd, string parameterName, bool value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Bool;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static void AddParameterContainer(this DbCommand cmd, DbParameterContainer container)
    {
        container.AttachToCommand(cmd);
    }

    public static string GenerateParameterDeclarationsAndCommandStringForQueryWindow(this DbCommand cmd)
    {
        string result = string.Empty;

        foreach (DbParameter p in cmd.Parameters)
        {
            string paramType = string.Empty;
            string strParamValue = string.Empty; //Converted explicitly to string

            switch (p.DbType)
            {
                case DbTypeManager.DbType_Bool:
                    paramType = "int";
                    strParamValue = p.Value.ToString();
                    break;
                case DbTypeManager.DbType_Decimal:
                    paramType = "decimal";
                    strParamValue = p.Value.ToString();
                    break;
                case DbTypeManager.DbType_Int:
                    paramType = "int";
                    strParamValue = p.Value.ToString();
                    break;
                case DbTypeManager.DbType_Long:
                    paramType = "bigint";
                    strParamValue = p.Value.ToString();
                    break;
                case DbTypeManager.DbType_NVarchar:
                    paramType = $"nvarchar({p.Size})";
                    strParamValue = $"'{p.Value}'";
                    break;
                case DbTypeManager.DbType_Varchar:
                    paramType = $"varchar({p.Size})";
                    strParamValue = $"'{p.Value}'";
                    break;
            }

            result += $"DECLARE {p.ParameterName} {paramType} = {strParamValue};\n";
        }

        result += $"{cmd.CommandText}\nGO";

        return result;
    }

    private static int GetInt32(object input)
    {
        if (input == null || input == DBNull.Value || !int.TryParse(input.ToString(), out _)) return 0;
        return Convert.ToInt32(input);
    }

    private static long GetInt64(object input)
    {
        if (input == null || input == DBNull.Value || !long.TryParse(input.ToString(), out _)) return 0L;
        return Convert.ToInt64(input);
    }

    private static decimal GetDecimal(object input, int decimalPlaces)
    {
        if (input == null || input == DBNull.Value || !decimal.TryParse(input.ToString(), out _)) return 0M;
        return Math.Round(Convert.ToDecimal(input), decimalPlaces);
    }

    private static double GetDouble(object input, int decimalPlaces)
    {
        if (input == null || input == DBNull.Value || !decimal.TryParse(input.ToString(), out _)) return 0.0;
        return Math.Round(Convert.ToDouble(input), decimalPlaces);
    }

    private static string GetString(object input)
    {
        if (input == null || input == DBNull.Value) return string.Empty;
        return input.ToString();
    }

    private static bool GetBoolean(object input)
    {
        if (input == DBNull.Value) return false;
        return Convert.ToBoolean(input);
    }

    public static int GetIntegerValue(this DbCommand cmd)
    {
        return GetInt32(cmd.ExecuteScalar2());
    }

    public static long GetLongValue(this DbCommand cmd)
    {
        return GetInt64(cmd.ExecuteScalar2());
    }

    public static decimal GetDecimalValue(this DbCommand cmd, int decimalPlaces)
    {
        return GetDecimal(cmd.ExecuteScalar2(), decimalPlaces);
    }

    public static double GetDoubleValue(this DbCommand cmd, int decimalPlaces)
    {
        return GetDouble(cmd.ExecuteScalar2(), decimalPlaces);
    }

    public static string GetStringValue(this DbCommand cmd)
    {
        return GetString(cmd.ExecuteScalar2());
    }

    public static bool GetBooleanValue(this DbCommand cmd)
    {
        return GetBoolean(cmd.ExecuteScalar2());
    }
}
