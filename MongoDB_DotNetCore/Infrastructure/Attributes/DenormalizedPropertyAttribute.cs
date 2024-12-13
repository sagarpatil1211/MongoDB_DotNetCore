using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class DenormalizedPropertyAttribute : Attribute 
{
    public DenormalizedPropertyAttribute(Type targetEntityType, string targetEntityPropertyName,
        string viaTargetEntityForeignKeyPropertyName)
    {
        TargetEntityType = targetEntityType;
        TargetEntityPropertyName = targetEntityPropertyName;
        ViaTargetEntityForeignKeyPropertyName = viaTargetEntityForeignKeyPropertyName;
    }

    public Type TargetEntityType { get; } = null;
    public string TargetEntityPropertyName { get; } = string.Empty;
    public string ViaTargetEntityForeignKeyPropertyName { get; } = string.Empty;
}
