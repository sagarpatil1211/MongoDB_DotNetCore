using System;

//name1space VJCore.Infrastructure;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class EnumMemberStringAttribute : Attribute
{
    public EnumMemberStringAttribute(string memberString)
    {
        this.MemberString = memberString;
    }

    public string MemberString { get; private set; } = string.Empty;
}
