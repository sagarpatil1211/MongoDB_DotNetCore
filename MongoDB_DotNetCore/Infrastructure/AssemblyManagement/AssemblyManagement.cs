using System;
using System.Collections.Generic;
using System.Reflection;

//name1space VJCore.Infrastructure;

public class AssemblyManagement
{
    public static List<Assembly> GetAllAssemblies()
    {
        return new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
    }
}