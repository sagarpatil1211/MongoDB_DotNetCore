using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

//name1space VJCore.Infrastructure.DbConnectionObjects;

public class CommandFormulationCoordinator
{
    private CommandFormulationCoordinator(string dbObjectName, string columnNamesToSelect = "*")
        : this(new List<string>(), DbParameterContainer.CreateInstance(), dbObjectName, columnNamesToSelect)
    { }

    private CommandFormulationCoordinator(List<string> lstFilterStrings,
        DbParameterContainer parameters, string dbObjectName,
        string columnNamesToSelect = "*")
    {
        LstFilterStrings = lstFilterStrings;
        Parameters = parameters;
        DbObjectName = dbObjectName;
        ColumnNamesToSelect = columnNamesToSelect;
    }

    public List<string> LstFilterStrings { get; }
    public DbParameterContainer Parameters { get; }
    public string DbObjectName { get; }
    public string ColumnNamesToSelect { get; set; } = "*";

    public long LimitRowCount { get; private set; } = -1L;

    public static CommandFormulationCoordinator CreateInstance(string dbObjectName, string columnNamesToSelect = "*")
    {
        return new CommandFormulationCoordinator(dbObjectName, columnNamesToSelect);
    }

    public static CommandFormulationCoordinator CreateInstance(List<string> lstFilterStrings,
        DbParameterContainer parameters, string dbObjectName)
    {
        return new CommandFormulationCoordinator(lstFilterStrings, parameters, dbObjectName);
    }

    public CommandFormulationCoordinator AddFilterString(string filterString)
    {
        LstFilterStrings.Add(filterString);
        return this;
    }

    public CommandFormulationCoordinator SetLimitRowCount(long limit)
    {
        if (limit > 0) LimitRowCount = limit;
        return this;
    }

    public CommandFormulationCoordinator AddVarcharParameter(string parameterName, int size, string value)
    {
        Parameters.AddVarchar(parameterName, size, value);
        return this;
    }

    public CommandFormulationCoordinator AddVarcharParameter(string parameterName, string dbObjectName, string columnName, string value)
    {
        int size = DataAccessUtils.GetVarcharFieldSize(dbObjectName, columnName);
        return AddVarcharParameter(parameterName, size, value);
    }

    public CommandFormulationCoordinator AddVarcharParameter(string parameterName, string dbObjectName, string value)
    {
        string columnName = parameterName;
        if (columnName.StartsWith("@")) columnName = columnName[1..];

        return AddVarcharParameter(parameterName, dbObjectName, columnName, value);
    }

    public CommandFormulationCoordinator AddNVarcharParameter(string parameterName, int size, string value)
    {
        Parameters.AddNVarchar(parameterName, size, value);
        return this;
    }

    public CommandFormulationCoordinator AddLongParameter(string parameterName, long value)
    {
        Parameters.AddLong(parameterName, value);
        return this;
    }

    public CommandFormulationCoordinator AddIntegerParameter(string parameterName, int value)
    {
        Parameters.AddInteger(parameterName, value);
        return this;
    }

    public CommandFormulationCoordinator AddDecimalParameter(string parameterName, byte precision, byte scale, decimal value)
    {
        Parameters.AddDecimal(parameterName, precision, scale, value);
        return this;
    }

    public CommandFormulationCoordinator CoordinateVarcharValues(List<string> lstFilterStrings,
        DbParameterContainer parameters, string dbObjectName,
        string dbFieldName, int size, List<string> lstValues)
    {
        if (lstValues.Count > 0)
        {
            string parameterName = $"@{dbFieldName}";

            parameters.AddVarcharList(parameterName, size, lstValues);

            string parameterNamesString = Utils.CombineListIntoSingleString(parameters.GetIndexedVarcharListParameterNames(parameterName), ",");

            lstFilterStrings.Add($"{dbObjectName}.{dbFieldName} IN ({parameterNamesString})");
        }

        return this;
    }

    public CommandFormulationCoordinator CoordinateVarcharValues(string dbFieldName, int size, List<string> lstValues)
    {
        return CoordinateVarcharValues(LstFilterStrings, Parameters, DbObjectName, dbFieldName, size, lstValues);
    }

    public CommandFormulationCoordinator CoordinateVarcharValues(string dbFieldName, string dbObjectName, List<string> lstValues)
    {
        var pSize = DataAccessUtils.GetVarcharFieldSize(dbObjectName, dbFieldName);
        return CoordinateVarcharValues(dbFieldName, pSize, lstValues);
    }

    public CommandFormulationCoordinator CoordinateNVarcharValues(List<string> lstFilterStrings,
        DbParameterContainer parameters, string dbObjectName,
        string dbFieldName, int size, List<string> lstValues)
    {
        if (lstValues.Count > 0)
        {
            string parameterName = $"@{dbFieldName}";

            parameters.AddNVarcharList(parameterName, size, lstValues);

            string parameterNamesString = Utils.CombineListIntoSingleString(parameters.GetIndexedNVarcharListParameterNames(parameterName), ",");

            lstFilterStrings.Add($"{dbObjectName}.{dbFieldName} IN ({parameterNamesString})");
        }

        return this;
    }

    public CommandFormulationCoordinator CoordinateNVarcharValues(string dbFieldName, int size, List<string> lstValues)
    {
        return CoordinateNVarcharValues(LstFilterStrings, Parameters, DbObjectName, dbFieldName, size, lstValues);
    }

    public CommandFormulationCoordinator CoordinateNVarcharValues(string dbFieldName, string dbObjectName, List<string> lstValues)
    {
        var pSize = DataAccessUtils.GetVarcharFieldSize(dbObjectName, dbFieldName);
        return CoordinateNVarcharValues(dbFieldName, pSize, lstValues);
    }

    public CommandFormulationCoordinator CoordinateLongValues(List<string> lstFilterStrings,
        DbParameterContainer parameters, string dbObjectName,
        string dbFieldName, List<long> lstValues)
    {
        if (lstValues.Count > 0)
        {
            string parameterName = $"@{dbFieldName}";

            parameters.AddLongList(parameterName, lstValues);

            string parameterNamesString = Utils.CombineListIntoSingleString(parameters.GetIndexedLongListParameterNames(parameterName), ",");

            lstFilterStrings.Add($"{dbObjectName}.{dbFieldName} IN ({parameterNamesString})");
        }

        return this;
    }

    public CommandFormulationCoordinator CoordinateLongValues(string dbFieldName, List<long> lstValues)
    {
        return CoordinateLongValues(LstFilterStrings, Parameters, DbObjectName, dbFieldName, lstValues);
    }

    public CommandFormulationCoordinator CoordinateIntegerValues(List<string> lstFilterStrings,
        DbParameterContainer parameters, string dbObjectName,
        string dbFieldName, List<int> lstValues)
    {
        if (lstValues.Count > 0)
        {
            string parameterName = $"@{dbFieldName}";

            parameters.AddIntegerList(parameterName, lstValues);

            string parameterNamesString = Utils.CombineListIntoSingleString(parameters.GetIndexedIntegerListParameterNames(parameterName), ",");

            lstFilterStrings.Add($"{dbObjectName}.{dbFieldName} IN ({parameterNamesString})");
        }

        return this;
    }

    public CommandFormulationCoordinator CoordinateIntegerValues(string dbFieldName, List<int> lstValues)
    {
        return CoordinateIntegerValues(LstFilterStrings, Parameters, DbObjectName, dbFieldName, lstValues);
    }

    public CommandFormulationCoordinator CoordinateEnumValues<T>(string dbFieldName, List<T> lstValues)
        where T : Enum
    {
        return CoordinateIntegerValues(dbFieldName, lstValues.Select(v => Convert.ToInt32(v)).ToList());
    }

    private static List<string> FormulateSearchStringParts(string searchString)
    {
        List<string> result = searchString.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
        result.RemoveAll(s => s.Trim().Length == 0);

        return result;
    }

    public CommandFormulationCoordinator CoordinateStringSearchValue(List<string> lstFilterStrings,
       string dbObjectName, string dbFieldName, string searchString)
    {
        if (searchString.Trim().Length > 0)
        {
            var stringParts = FormulateSearchStringParts(searchString);

            string fStringInternal = string.Empty;

            int i = 0;

            foreach (var namePart in stringParts)
            {
                i++;

                if (fStringInternal.Trim().Length > 0) fStringInternal += $" AND ";
                fStringInternal += $"{dbObjectName}.{dbFieldName} LIKE '%' + '{namePart}' + '%'";
            }

            lstFilterStrings.Add($"({fStringInternal})");
        }

        return this;
    }

    public CommandFormulationCoordinator CoordinateStringSearchValue(string dbFieldName,
        string searchString)
    {
        return CoordinateStringSearchValue(LstFilterStrings, DbObjectName, dbFieldName, searchString);
    }

    public CommandFormulationCoordinator CoordinateStringSearchValue(string dbFieldName,
        string dbObjectName, string searchString)
    {
        int size = DataAccessUtils.GetVarcharFieldSize(dbObjectName, dbFieldName);
        return CoordinateStringSearchValue(LstFilterStrings, DbObjectName, dbFieldName, searchString);
    }

    public string FormulateFilterString()
    {
        return Utils.CombineListIntoSingleString(LstFilterStrings, " AND ");
    }

    public void AttachParametersToCommand(DbCommand cmd)
    {
        cmd.Parameters.Clear();
        Parameters.AttachToCommand(cmd);
    }

    public CommandFormulationCoordinator FormulateCommandStringAndParameters(DbCommand cmd, long rowLimit = -1,
        string commandStringSuffix = "")
    {
        string strSelect = string.Empty;

        if (rowLimit > 0)
        {
            strSelect = $"SELECT TOP {rowLimit} {DbObjectName}.{ColumnNamesToSelect} FROM {DbObjectName}";
        }
        else
        {
            strSelect = $"SELECT {DbObjectName}.{ColumnNamesToSelect} FROM {DbObjectName}";
        }

        string fString = FormulateFilterString();

        if (fString.Length > 0)
        {
            strSelect += $" WHERE {fString}";
        }

        if (commandStringSuffix.Trim().Length > 0)
        {
            strSelect += $" {commandStringSuffix}";
        }

        cmd.CommandText = strSelect;

        //cmd.Parameters.Clear();
        //Parameters.AttachToCommand(cmd);

        AttachParametersToCommand(cmd);

        return this;
    }
}