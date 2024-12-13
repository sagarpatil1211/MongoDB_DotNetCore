using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class PersistedEntityAttribute : Attribute 
{
    public PersistedEntityAttribute(string dbTableName)
    {
        DbTableName = dbTableName;
    }

    public string DbTableName { get; } = string.Empty;
}
