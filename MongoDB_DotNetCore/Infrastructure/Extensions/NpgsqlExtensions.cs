using Npgsql;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
//using VJ1Core.Infrastructure.DbConnectionObjects;

namespace VJCore.Infrastructure.Extensions;

public static class NpgsqlExtensions
{
    public static NpgsqlParameter AddParameterWithValue(this NpgsqlCommand cmd, string parameterName, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static NpgsqlParameter AddVarcharParameter(this NpgsqlCommand cmd, string parameterName, int size, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Varchar;
        p.Size = size;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static NpgsqlParameter AddVarcharMaxParameter(this NpgsqlCommand cmd, string parameterName, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Varchar;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static NpgsqlParameter AddNVarcharParameter(this NpgsqlCommand cmd, string parameterName, int size, string value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_NVarchar;
        p.Size = size;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static NpgsqlParameter AddIntegerParameter(this NpgsqlCommand cmd, string parameterName, int value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Int;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static NpgsqlParameter AddLongParameter(this NpgsqlCommand cmd, string parameterName, long value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Long;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static NpgsqlParameter AddDecimalParameter(this NpgsqlCommand cmd, string parameterName,
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

    public static NpgsqlParameter AddBooleanParameter(this NpgsqlCommand cmd, string parameterName, bool value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = DbTypeManager.DbType_Bool;
        p.Value = value;
        cmd.Parameters.Add(p);

        return p;
    }

    public static List<NpgsqlParameter> AddLongListParameter(this NpgsqlCommand cmd, string parameterBaseName, List<long> values)
    {
        List<NpgsqlParameter> result = new List<NpgsqlParameter>();

        int index = 0;

        foreach (long paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterBaseName}{index}";
            result.Add(cmd.AddLongParameter(indexedParamName, paramValue));
        }

        return result;
    }

    public static List<NpgsqlParameter> AddIntegerListParameter(this NpgsqlCommand cmd, string parameterBaseName, List<int> values)
    {
        List<NpgsqlParameter> result = new List<NpgsqlParameter>();

        int index = 0;

        foreach (int paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterBaseName}{index}";
            result.Add(cmd.AddIntegerParameter(indexedParamName, paramValue));
        }

        return result;
    }

    public static List<NpgsqlParameter> AddDecimalListParameter(this NpgsqlCommand cmd, string parameterBaseName,
        byte precision, byte scale, List<decimal> values)
    {
        List<NpgsqlParameter> result = new List<NpgsqlParameter>();

        int index = 0;

        foreach (decimal paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterBaseName}{index}";
            result.Add(cmd.AddDecimalParameter(indexedParamName, precision, scale, paramValue));
        }

        return result;
    }

    public static List<NpgsqlParameter> AddVarcharListParameter(this NpgsqlCommand cmd, string parameterBaseName,
        int paramSize, List<string> values)
    {
        List<NpgsqlParameter> result = new List<NpgsqlParameter>();

        int index = 0;

        foreach (string paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterBaseName}{index}";
            result.Add(cmd.AddVarcharParameter(indexedParamName, paramSize, paramValue));
        }

        return result;
    }

    public static List<NpgsqlParameter> AddNVarcharListParameter(this NpgsqlCommand cmd, string parameterBaseName,
        int paramSize, List<string> values)
    {
        List<NpgsqlParameter> result = new List<NpgsqlParameter>();

        int index = 0;

        foreach (string paramValue in values)
        {
            index++;
            string indexedParamName = $"{parameterBaseName}{index}";
            result.Add(cmd.AddNVarcharParameter(indexedParamName, paramSize, paramValue));
        }

        return result;
    }

    public static void AddParameterContainer(this NpgsqlCommand cmd, DbParameterContainer container)
    {
        container.AttachToCommand(cmd);
    }

    public static string GenerateParameterDeclarationsAndCommandStringForQueryWindow(this NpgsqlCommand cmd)
    {
        string result = string.Empty;

        foreach (NpgsqlParameter p in cmd.Parameters.Cast<NpgsqlParameter>())
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
}
