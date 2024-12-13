using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class DenormalizedEnumPropertyNameAttribute : Attribute
{
    public DenormalizedEnumPropertyNameAttribute(Type targetEnumType, string viaTargetEntityForeignKeyPropertyName)
    {
        TargetEnumType = targetEnumType;
        ViaTargetEntityForeignKeyPropertyName = viaTargetEntityForeignKeyPropertyName;
    }

    public Type TargetEnumType { get; } = null;
    public string ViaTargetEntityForeignKeyPropertyName { get; }
}