using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
public class PersistedEnumAttribute : Attribute
{
    public PersistedEnumAttribute() { }

    public PersistedEnumAttribute(string dbTableName)
    {
        this.DBTableName = dbTableName;
    }

    public string DBTableName { get; private set; } = string.Empty;
}
