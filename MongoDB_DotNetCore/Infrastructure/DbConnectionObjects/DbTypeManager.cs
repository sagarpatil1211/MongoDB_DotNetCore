using System.Data;

public class DbTypeManager
{
    public const DbType DbType_Varchar = DbType.AnsiStringFixedLength;
    public const DbType DbType_NVarchar = DbType.StringFixedLength;
    public const DbType DbType_Int = DbType.Int32;
    public const DbType DbType_Long = DbType.Int64;
    public const DbType DbType_Decimal = DbType.Decimal;
    public const DbType DbType_Bool = DbType.Boolean;
}
