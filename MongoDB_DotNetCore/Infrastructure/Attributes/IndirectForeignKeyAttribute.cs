using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class IndirectForeignKeyAttribute : Attribute 
{
    public IndirectForeignKeyAttribute(Type primaryKeyEntityType, bool nullPrimaryKeyAllowed = false)
    {
        PrimaryKeyEntityType = primaryKeyEntityType;
        NullPrimaryKeyAllowed = nullPrimaryKeyAllowed;
    }

    public IndirectForeignKeyAttribute(Type primaryKeyEntityType, string primaryKeyEntityPropertyName,
        bool nullPrimaryKeyAllowed = false)
    {
        PrimaryKeyEntityType = primaryKeyEntityType;
        PrimaryKeyEntityPropertyName = primaryKeyEntityPropertyName;
        NullPrimaryKeyAllowed = nullPrimaryKeyAllowed;
    }

    public Type PrimaryKeyEntityType { get; } = null;
    public string PrimaryKeyEntityPropertyName { get; } = string.Empty;
    public bool NullPrimaryKeyAllowed { get; } = false;
}
