//using SSDBCA;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Infrastructure;
//using VJ1Core.Infrastructure.DatabaseMigration.Instructions;
//using VJ1Core.Infrastructure.DatabaseMigration.SQLiteConnectivity;
//using VJ1Core.Infrastructure.Main.Entity_Metadata_Management;

public partial class ReadonlyEntityTypes
{
    private static bool entityTypeRegistrationsDone = false;

    private static readonly ConcurrentDictionary<string, List<DenormalizedPropertyAttribute>> m_denormalizedPropertyAttributes
        = new ConcurrentDictionary<string, List<DenormalizedPropertyAttribute>>();
    // Key = EntityType, Value = List<DenormalizedPropertyAttribute>

    private static readonly ConcurrentDictionary<string, List<DenormalizedEnumPropertyNameAttribute>> m_denormalizedEnumPropertyNameAttributes
        = new ConcurrentDictionary<string, List<DenormalizedEnumPropertyNameAttribute>>();
    // Key = EntityType, Value = List<DenormalizedEnumPropertyNameAttribute>

    public static List<DenormalizedPropertyAttribute> GetDenormalizedPropertyAttributes(string entityType)
    {
        m_denormalizedPropertyAttributes.TryGetValue(entityType, out var result);
        return result;
    }

    public static List<DenormalizedEnumPropertyNameAttribute> GetDenormalizedEnumPropertyNameAttributes(string entityType)
    {
        m_denormalizedEnumPropertyNameAttributes.TryGetValue(entityType, out var result);
        return result;
    }

    public static void PerformRegistrations()
    {
        if (entityTypeRegistrationsDone) return;

        var dbMigrationDAL = new SQLiteDataAccessService(string.Empty, (msg, title) => { });

        var assemblies = AssemblyManagement.GetAllAssemblies();

        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                if (t.IsSubclassOf(typeof(ReadonlyBaseObject)) && !t.IsAbstract)
                {
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (m.Name == "RegisterEntityTypeAspects") m.Invoke(null, null);
                    }

                    string entityType = string.Empty;

                    foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (p.Name == "EntityType") entityType = p.GetValue(null).ToString();
                    }

                    if (entityType.Trim().Length == 0) throw new DomainException($"EntityType field not found on type {t.FullName}");

                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        var dnpa = p.GetCustomAttribute<DenormalizedPropertyAttribute>();
                        if (dnpa != null)
                        {
                            m_denormalizedPropertyAttributes.AddOrUpdate(entityType,
                                new List<DenormalizedPropertyAttribute> { dnpa },
                                (eType, lst) =>
                                {
                                    lst.Add(dnpa);
                                    return lst;
                                });
                        }
                    }

                    foreach (PropertyInfo p in t.GetProperties())
                    {
                        var dnepa = p.GetCustomAttribute<DenormalizedEnumPropertyNameAttribute>();
                        if (dnepa != null)
                        {
                            m_denormalizedEnumPropertyNameAttributes.AddOrUpdate(entityType,
                                new List<DenormalizedEnumPropertyNameAttribute> { dnepa },
                                (eType, lst) =>
                                {
                                    lst.Add(dnepa);
                                    return lst;
                                });
                        }
                    }
                }

                if (t.GetCustomAttribute<NonPersistedFunctionalEntityAttribute>() != null)
                {
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (m.Name == "RegisterEntityTypeAspects") m.Invoke(null, null);
                    }
                }

                if (t.GetCustomAttribute<RealTimeDataEntityAttribute>() != null)
                {
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        if (m.Name == "RegisterEntityTypeAspects") m.Invoke(null, null);
                    }
                }
            }
        }

        entityTypeRegistrationsDone = true;
    }

    public static Dictionary<string, Tuple<PropertyTypes, string>> GetPropertyTypeMap(string tName)
    {
        if (m_cachedPropertyTypeMaps.TryGetValue(tName, out Dictionary<string, Tuple<PropertyTypes, string>> value))
        {
            return value;
        }

        return GetPropertyTypeMap(Type.GetType(tName));
    }

    public static Dictionary<string, Tuple<PropertyTypes, string>> GetPropertyTypeMap(Type t)
        // Key = Property Name
        // Tuple Item 1 = Property Type
        // Tuple Item 2 = Specific Type in case of Collections and Complex Properties
    {
        if (m_cachedPropertyTypeMaps.TryGetValue(t.FullName, out Dictionary<string, Tuple<PropertyTypes, string>> value))
        {
            return value;
        }

        Dictionary<string, Tuple<PropertyTypes, string>> result
            = new Dictionary<string, Tuple<PropertyTypes, string>>();

        PropertyInfo[] props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (PropertyInfo p in props)
        {
            string typeFullName = p.PropertyType.FullName;

            if (typeFullName == typeof(string).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.String, string.Empty);
            }
            else if (typeFullName == typeof(decimal).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Decimal, string.Empty);
            }
            else if (typeFullName == typeof(int).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Integer, string.Empty);
            }
            else if (typeFullName == typeof(long).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Long, string.Empty);
            }
            else if (typeFullName == typeof(bool).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Boolean, string.Empty);
            }
            else if (typeFullName == typeof(byte[]).FullName)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.ByteArray, string.Empty);
            }
            else if (p.PropertyType.GetInterface(typeof(IEnumerable).FullName) != null)
            {
                if (p.PropertyType.GenericTypeArguments.Length > 0)
                {
                    foreach (Type tInternal in p.PropertyType.GenericTypeArguments)
                    {
                        result[p.Name] = Tuple.Create(PropertyTypes.Collection, tInternal.FullName);
                        break;
                    }
                }
                else
                {
                    result[p.Name] = Tuple.Create(PropertyTypes.Collection, string.Empty);
                }
            }
            else if (p.PropertyType.IsClass)
            {
                result[p.Name] = Tuple.Create(PropertyTypes.Complex, p.PropertyType.FullName);
            }
        }

        if (!m_cachedPropertyTypeMaps.ContainsKey(t.FullName))
        {
            m_cachedPropertyTypeMaps[t.FullName] = result;
        }

        return result;
    }

    private static readonly Dictionary<string, Dictionary<string, Tuple<PropertyTypes, string>>> m_cachedPropertyTypeMaps
        = new Dictionary<string, Dictionary<string, Tuple<PropertyTypes, string>>>();
}
