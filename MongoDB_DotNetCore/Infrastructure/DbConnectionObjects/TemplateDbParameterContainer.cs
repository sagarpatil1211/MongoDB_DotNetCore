using System.Collections.Generic;

//name1space VJCore.Infrastructure.DbConnectionObjects;

public class TemplateDbParameterContainer
{
    public TemplateDbParameterContainer(string objectName)
    {
        ObjectName = objectName;
    }

    public string ObjectName { get; }

    private readonly List<DbParameterInfo> m_parameters = new List<DbParameterInfo>();

    public List<DbParameterInfo> GetAllParameters()
    {
        return m_parameters;
    }

    public List<DbParameterInfo> GetVarcharParameters()
    { 
        return m_parameters.FindAll(p => p.DbType == DbTypeManager.DbType_Varchar); 
    }

    public DbParameterInfo GetVarcharParameter(string parameterName)
    {
        return GetVarcharParameters().Find(p => p.Name == parameterName);
    }

    public List<DbParameterInfo> GetNVarcharParameters()
    {
        return m_parameters.FindAll(p => p.DbType == DbTypeManager.DbType_NVarchar);
    }

    public List<DbParameterInfo> GetIntegerParameters()
    {
        return m_parameters.FindAll(p => p.DbType == DbTypeManager.DbType_Int);
    }

    public List<DbParameterInfo> GetLongParameters()
    {
        return m_parameters.FindAll(p => p.DbType == DbTypeManager.DbType_Long);
    }

    public List<DbParameterInfo> GetDecimalParameters()
    {
        return m_parameters.FindAll(p => p.DbType == DbTypeManager.DbType_Decimal);
    }

    public List<DbParameterInfo> GetBooleanParameters()
    {
        return m_parameters.FindAll(p => p.DbType == DbTypeManager.DbType_Bool);
    }

    public void AddVarchar(string parameterName, int size)
    {
        m_parameters.Add(new DbParameterInfo(parameterName, DbTypeManager.DbType_Varchar, size));
    }

    public void AddNVarchar(string parameterName, int size)
    {
        m_parameters.Add(new DbParameterInfo(parameterName, DbTypeManager.DbType_NVarchar, size));
    }

    public void AddInteger(string parameterName)
    {
        m_parameters.Add(new DbParameterInfo(parameterName, DbTypeManager.DbType_Int));
    }

    public void AddLong(string parameterName)
    {
        m_parameters.Add(new DbParameterInfo(parameterName, DbTypeManager.DbType_Long));
    }

    public void AddDecimal(string parameterName, byte precision, byte scale)
    {
        m_parameters.Add(new DbParameterInfo(parameterName, DbTypeManager.DbType_Decimal, 0, precision, scale));
    }

    public void AddBoolean(string parameterName)
    {
        m_parameters.Add(new DbParameterInfo(parameterName, DbTypeManager.DbType_Bool));
    }

    public DbParameterInfo GetDbParameterInfo(string parameterName)
    {
        return m_parameters.Find(p => p.Name == parameterName);
    }
}
