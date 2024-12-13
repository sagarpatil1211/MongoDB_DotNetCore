using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class PersistedPropertyAttribute : Attribute 
{
    public PersistedPropertyAttribute() {}

    public PersistedPropertyAttribute(string dbFieldName)
    {
        DbFieldName = dbFieldName;
    }

    public string DbFieldName { get; } = string.Empty;
}
