using System;
using System.Collections.Generic;
using System.Text;

//name1space VJCore.Infrastructure.DbConnectionObjects;

partial class DbParameterContainer
{
    public DbParameterContainer AddVarchar(string parameterName, string dbObjectName, string value)
    {
        string columnName = parameterName;
        if (columnName.StartsWith("@")) columnName = columnName[1..];

        int size = DataAccessUtils.GetVarcharFieldSize(dbObjectName, columnName);
        return AddVarchar(parameterName, size, value);
    }

    public DbParameterContainer AddVarchar(string parameterName, string dbObjectName, string columnName, string value)
    {
        int size = DataAccessUtils.GetVarcharFieldSize(dbObjectName, columnName);
        return AddVarchar(parameterName, size, value);
    }
}
