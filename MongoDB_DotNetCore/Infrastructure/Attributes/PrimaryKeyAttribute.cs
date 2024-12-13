using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class PrimaryKeyAttribute : Attribute 
{
    public PrimaryKeyAttribute() { }
}
