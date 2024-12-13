using VJCore.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

public partial class DbParameterContainer
{
    public static DbParameterContainer CreateInstance()
    {
        return new DbParameterContainer();
    }

    private readonly List<Tuple<DbParameterInfo, string>> m_varcharParameters = new List<Tuple<DbParameterInfo, string>>();
    // first string for parameter name, second string for parameter value, int for size

    private readonly List<Tuple<DbParameterInfo, string>> m_varcharMaxParameters = new List<Tuple<DbParameterInfo, string>>();
    // first string for parameter name, second string for parameter value

    private readonly List<Tuple<DbParameterInfo, string>> m_nvarcharParameters = new List<Tuple<DbParameterInfo, string>>();
    // first string for parameter name, second string for parameter value, int for size

    private readonly List<Tuple<DbParameterInfo, int>> m_integerParameters = new List<Tuple<DbParameterInfo, int>>();
    // first string for parameter name, int for value

    private readonly List<Tuple<DbParameterInfo, long>> m_longParameters = new List<Tuple<DbParameterInfo, long>>();
    // first string for parameter name, long for value

    private readonly List<Tuple<DbParameterInfo, decimal>> m_decimalParameters = new List<Tuple<DbParameterInfo, decimal>>();
    // first string for parameter name, decimal for value, first byte for precision, second byte for scale

    private readonly List<Tuple<DbParameterInfo, bool>> m_booleanParameters = new List<Tuple<DbParameterInfo, bool>>();
    // first string for parameter name, bool for value


    private readonly List<Tuple<DbParameterInfo, List<string>>> m_varcharListParameters = new List<Tuple<DbParameterInfo, List<string>>>();
    // first string for parameter name, second string list for parameter values, int for size

    private readonly List<Tuple<DbParameterInfo, List<string>>> m_nvarcharListParameters = new List<Tuple<DbParameterInfo, List<string>>>();
    // first string for parameter name, second string list for parameter values, int for size

    private readonly List<Tuple<DbParameterInfo, List<int>>> m_integerListParameters = new List<Tuple<DbParameterInfo, List<int>>>();
    // first string for parameter name, List<int> for values

    private readonly List<Tuple<DbParameterInfo, List<long>>> m_longListParameters = new List<Tuple<DbParameterInfo, List<long>>>();
    // first string for parameter name, List<long> for values

    private readonly List<Tuple<DbParameterInfo, List<decimal>>> m_decimalListParameters = new List<Tuple<DbParameterInfo, List<decimal>>>();
    // first string for parameter name, List<decimal> for values, first byte for precision, second byte for scale


    public List<Tuple<DbParameterInfo, string>> GetVarcharParameters() 
    { 
        return m_varcharParameters; 
    }

    public List<Tuple<DbParameterInfo, string>> GetVarcharMaxParameters()
    {
        return m_varcharMaxParameters;
    }

    public List<Tuple<DbParameterInfo, string>> GetNVarcharParameters()
    {
        return m_nvarcharParameters;
    }

    public List<Tuple<DbParameterInfo, int>> GetIntegerParameters()
    {
        return m_integerParameters;
    }

    public List<Tuple<DbParameterInfo, long>> GetLongParameters()
    {
        return m_longParameters;
    }

    public List<Tuple<DbParameterInfo, decimal>> GetDecimalParameters()
    {
        return m_decimalParameters;
    }

    public List<Tuple<DbParameterInfo, bool>> GetBooleanParameters()
    {
        return m_booleanParameters;
    }

    public List<Tuple<DbParameterInfo, List<string>>> GetAllVarcharListParameters()
    {
        return m_varcharListParameters;
    }

    public List<Tuple<DbParameterInfo, List<string>>> GetVarcharListParameters(string parameterName)
    {
        return m_varcharListParameters.FindAll(p => p.Item1.Name == parameterName);
    }

    public List<string> GetIndexedVarcharListParameterNames(string parameterName)
    {
        List<string> result = new List<string>();

        foreach (var t in GetVarcharListParameters(parameterName))
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (string paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                result.Add(indexedParamName);
            }
        }

        return result;
    }

    public List<Tuple<DbParameterInfo, List<string>>> GetAllNVarcharListParameters()
    {
        return m_nvarcharListParameters;
    }

    public List<Tuple<DbParameterInfo, List<string>>> GetNVarcharListParameters(string parameterName)
    {
        return m_nvarcharListParameters.FindAll(p => p.Item1.Name == parameterName);
    }

    public List<string> GetIndexedNVarcharListParameterNames(string parameterName)
    {
        List<string> result = new List<string>();

        foreach (var t in GetNVarcharListParameters(parameterName))
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (string paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                result.Add(indexedParamName);
            }
        }

        return result;
    }

    public List<Tuple<DbParameterInfo, List<int>>> GetAllIntegerListParameters()
    {
        return m_integerListParameters;
    }

    public List<Tuple<DbParameterInfo, List<int>>> GetIntegerListParameters(string parameterName)
    {
        return m_integerListParameters.FindAll(p => p.Item1.Name == parameterName);
    }

    public List<string> GetIndexedIntegerListParameterNames(string parameterName)
    {
        List<string> result = new List<string>();

        foreach (var t in GetIntegerListParameters(parameterName))
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (int paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                result.Add(indexedParamName);
            }
        }

        return result;
    }

    public List<Tuple<DbParameterInfo, List<long>>> GetAllLongListParameters()
    {
        return m_longListParameters;
    }

    public List<Tuple<DbParameterInfo, List<long>>> GetLongListParameters(string parameterName)
    {
        return m_longListParameters.FindAll(p => p.Item1.Name == parameterName);
    }

    public List<string> GetIndexedLongListParameterNames(string parameterName)
    {
        List<string> result = new List<string>();

        foreach (var t in GetLongListParameters(parameterName))
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (long paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                result.Add(indexedParamName);
            }
        }

        return result;
    }

    public List<Tuple<DbParameterInfo, List<decimal>>> GetAllDecimalListParameters()
    {
        return m_decimalListParameters;
    }

    public List<Tuple<DbParameterInfo, List<decimal>>> GetDecimalListParameters(string parameterName)
    {
        return m_decimalListParameters.FindAll(p => p.Item1.Name == parameterName);
    }

    public List<string> GetIndexedDecimalListParameterNames(string parameterName)
    {
        List<string> result = new List<string>();

        foreach (var t in GetDecimalListParameters(parameterName))
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (decimal paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                result.Add(indexedParamName);
            }
        }

        return result;
    }

    public DbParameterContainer AddVarchar(string parameterName, int size, string value)
    {
        m_varcharParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Varchar, size), value));
        return this;
    }

    public DbParameterContainer AddVarcharMax(string parameterName, string value)
    {
        m_varcharMaxParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Varchar), value));
        return this;
    }

    public DbParameterContainer AddNVarchar(string parameterName, int size, string value)
    {
        m_nvarcharParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_NVarchar, size), value));
        return this;
    }

    public DbParameterContainer AddInteger(string parameterName, int value)
    {
        m_integerParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Int), value));
        return this;
    }

    public DbParameterContainer AddLong(string parameterName, long value)
    {
        m_longParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Long), value));
        return this;
    }

    public DbParameterContainer AddDecimal(string parameterName, byte precision, byte scale, decimal value)
    {
        m_decimalParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Decimal, precision, scale), value));
        return this;
    }

    public DbParameterContainer AddBoolean(string parameterName, bool value)
    {
        m_booleanParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Bool), value));
        return this;
    }

    public DbParameterContainer AddVarcharList(string parameterName, int size, List<string> value)
    {
        m_varcharListParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Varchar, size), value));
        return this;
    }

    public DbParameterContainer AddNVarcharList(string parameterName, int size, List<string> value)
    {
        m_nvarcharListParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_NVarchar, size), value));
        return this;
    }

    public DbParameterContainer AddIntegerList(string parameterName, List<int> value)
    {
        m_integerListParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Int), value));
        return this;
    }

    public DbParameterContainer AddLongList(string parameterName, List<long> value)
    {
        m_longListParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Long), value));
        return this;
    }

    public DbParameterContainer AddDecimalList(string parameterName, byte precision, byte scale, List<decimal> value)
    {
        m_decimalListParameters.Add(Tuple.Create(new DbParameterInfo(parameterName, DbTypeManager.DbType_Decimal, precision, scale), value));
        return this;
    }

    public void AttachToCommand(DbCommand cmd)
    {
        foreach(var t in GetVarcharParameters())
        {
            cmd.AddVarcharParameter(t.Item1.Name, t.Item1.Size, t.Item2);
        }

        foreach (var t in GetVarcharMaxParameters())
        {
            cmd.AddVarcharMaxParameter(t.Item1.Name, t.Item2);
        }

        foreach (var t in GetNVarcharParameters())
        {
            cmd.AddNVarcharParameter(t.Item1.Name, t.Item1.Size, t.Item2);
        }

        foreach (var t in GetIntegerParameters())
        {
            cmd.AddIntegerParameter(t.Item1.Name, t.Item2);
        }

        foreach (var t in GetLongParameters())
        {
            cmd.AddLongParameter(t.Item1.Name, t.Item2);
        }

        foreach (var t in GetDecimalParameters())
        {
            cmd.AddDecimalParameter(t.Item1.Name, t.Item1.Precision, t.Item1.Scale, t.Item2);
        }

        foreach (var t in GetBooleanParameters())
        {
            cmd.AddBooleanParameter(t.Item1.Name, t.Item2);
        }

        foreach (var t in GetAllVarcharListParameters())
        {
            string paramName = t.Item1.Name;
            int paramSize = t.Item1.Size;

            int index = 0;

            foreach (string paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                cmd.AddVarcharParameter(indexedParamName, paramSize, paramValue);
            }
        }

        foreach (var t in GetAllNVarcharListParameters())
        {
            string paramName = t.Item1.Name;
            int paramSize = t.Item1.Size;

            int index = 0;

            foreach (string paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                cmd.AddNVarcharParameter(indexedParamName, paramSize, paramValue);
            }
        }

        foreach (var t in GetAllIntegerListParameters())
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (int paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                cmd.AddIntegerParameter(indexedParamName, paramValue);
            }
        }

        foreach (var t in GetAllLongListParameters())
        {
            string paramName = t.Item1.Name;

            int index = 0;

            foreach (long paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                cmd.AddLongParameter(indexedParamName, paramValue);
            }
        }

        foreach (var t in GetAllDecimalListParameters())
        {
            string paramName = t.Item1.Name;
            byte precision = t.Item1.Precision;
            byte scale = t.Item1.Scale;

            int index = 0;

            foreach (decimal paramValue in t.Item2)
            {
                index++;
                string indexedParamName = $"{paramName}{index}";
                cmd.AddDecimalParameter(indexedParamName, precision, scale, paramValue);
            }
        }
    }

    public void FormulateParameterValuesFromTemplateAndDictionary(TemplateDbParameterContainer contTemp,
            Dictionary<string, object> valueMap)
    {
        foreach (var kvp in valueMap)
        {
            string paramName = kvp.Key;

            if (!paramName.StartsWith("@")) paramName = $"@{paramName}";

            object paramValue = kvp.Value;

            var pInfo = contTemp.GetDbParameterInfo(paramName);
            if (pInfo != null)
            {
                switch (pInfo.DbType)
                {
                    case DbTypeManager.DbType_Varchar:
                        AddVarchar(pInfo.Name, pInfo.Size, Convert.ToString(paramValue));
                        break;
                    case DbTypeManager.DbType_NVarchar:
                        AddNVarchar(pInfo.Name, pInfo.Size, Convert.ToString(paramValue));
                        break;
                    case DbTypeManager.DbType_Int:
                        AddInteger(pInfo.Name, Convert.ToInt32(paramValue));
                        break;
                    case DbTypeManager.DbType_Long:
                        AddLong(pInfo.Name, Convert.ToInt64(paramValue));
                        break;
                    case DbTypeManager.DbType_Decimal:
                        AddDecimal(pInfo.Name, pInfo.Precision, pInfo.Scale, Convert.ToDecimal(paramValue));
                        break;
                    case DbTypeManager.DbType_Bool:
                        AddBoolean(pInfo.Name, Convert.ToBoolean(paramValue));
                        break;
                }
            }
        }
    }

    public List<string> GetAllParameterNames()
    {
        List<string> result = new List<string>();

        result.AddRange(GetVarcharParameters().Select(p => p.Item1.Name));
        result.AddRange(GetVarcharMaxParameters().Select(p => p.Item1.Name));
        result.AddRange(GetNVarcharParameters().Select(p => p.Item1.Name));
        result.AddRange(GetIntegerParameters().Select(p => p.Item1.Name));
        result.AddRange(GetLongParameters().Select(p => p.Item1.Name));
        result.AddRange(GetDecimalParameters().Select(p => p.Item1.Name));
        result.AddRange(GetBooleanParameters().Select(p => p.Item1.Name));

        foreach (var pList in GetAllVarcharListParameters())
        {
            result.AddRange(GetIndexedVarcharListParameterNames(pList.Item1.Name));
        }

        foreach (var pList in GetAllNVarcharListParameters())
        {
            result.AddRange(GetIndexedNVarcharListParameterNames(pList.Item1.Name));
        }

        foreach (var pList in GetAllLongListParameters())
        {
            result.AddRange(GetIndexedLongListParameterNames(pList.Item1.Name));
        }

        foreach (var pList in GetAllIntegerListParameters())
        {
            result.AddRange(GetIndexedIntegerListParameterNames(pList.Item1.Name));
        }

        foreach (var pList in GetAllDecimalListParameters())
        {
            result.AddRange(GetIndexedDecimalListParameterNames(pList.Item1.Name));
        }

        return result;
    }

    public List<string> GetAllParameterNamesConvertedToColumnNames()
    {
        List<string> result = GetAllParameterNames()
            .Select(pName => pName.StartsWith("@") ? pName[1..] : pName)
            .ToList();

        return result;
    }

    public void AbsorbOtherContainer(DbParameterContainer other)
    {
        other.GetVarcharParameters().ForEach(p => m_varcharParameters.Add(p));
        other.GetVarcharMaxParameters().ForEach(p => m_varcharMaxParameters.Add(p));
        other.GetNVarcharParameters().ForEach(p => m_nvarcharParameters.Add(p));
        other.GetLongParameters().ForEach(p => m_longParameters.Add(p));
        other.GetIntegerParameters().ForEach(p => m_integerParameters.Add(p));
        other.GetDecimalParameters().ForEach(p => m_decimalParameters.Add(p));
        other.GetBooleanParameters().ForEach(p => m_booleanParameters.Add(p));

        other.GetAllVarcharListParameters().ForEach(p => m_varcharListParameters.Add(p));
        other.GetAllNVarcharListParameters().ForEach(p => m_nvarcharListParameters.Add(p));
        other.GetAllLongListParameters().ForEach(p => m_longListParameters.Add(p));
        other.GetAllIntegerListParameters().ForEach(p => m_integerListParameters.Add(p));
        other.GetAllDecimalListParameters().ForEach(p => m_decimalListParameters.Add(p));
    }
}
