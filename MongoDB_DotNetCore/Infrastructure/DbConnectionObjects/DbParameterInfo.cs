using VJCore.Infrastructure.Extensions;
using System;
using System.Data;
using System.Data.Common;

//name1space VJCore.Infrastructure.DbConnectionObjects;
public class DbParameterInfo
{
    public DbParameterInfo(string name, DbType dbType, int size = 0, byte precision = 0, byte scale = 0)
    {
        Name = name;
        DbType = dbType;
        Size = size;
        Precision = precision;
        Scale = scale;
    }

    public string Name { get; }
    public DbType DbType { get; }
    public int Size { get; }
    public byte Precision { get; }
    public byte Scale { get; }

    public void AttachToCommand(DbCommand cmd, object value)
    {
        switch (DbType)
        {
            case DbTypeManager.DbType_Varchar:
                cmd.AddVarcharParameter(Name, Size, Convert.ToString(value));
                break;

            case DbTypeManager.DbType_NVarchar:
                cmd.AddNVarcharParameter(Name, Size, Convert.ToString(value));
                break;

            case DbTypeManager.DbType_Long:
                cmd.AddLongParameter(Name, Convert.ToInt64(value));
                break;

            case DbTypeManager.DbType_Int:
                cmd.AddIntegerParameter(Name, Convert.ToInt32(value));
                break;

            case DbTypeManager.DbType_Decimal:
                cmd.AddDecimalParameter(Name, Precision, Scale, Convert.ToDecimal(value));
                break;

            case DbTypeManager.DbType_Bool:
                cmd.AddBooleanParameter(Name, Convert.ToBoolean(value));
                break;
        }
    }
}
