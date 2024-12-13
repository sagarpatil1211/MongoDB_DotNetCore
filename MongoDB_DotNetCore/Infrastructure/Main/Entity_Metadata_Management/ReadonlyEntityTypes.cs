using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ReadonlyEntityTypes
{
    private static readonly Dictionary<string, Dictionary<int, string>> m_enumStrings = new Dictionary<string, Dictionary<int, string>>();

    public static string GetEnumMemberString<T>(T value)
    {
        if (!m_enumStrings.ContainsKey(typeof(T).FullName)) return string.Empty;
        var dictStrings = m_enumStrings[typeof(T).FullName];

        int valueInt = Convert.ToInt32(value);

        if (!dictStrings.ContainsKey(valueInt)) return string.Empty;

        return dictStrings[valueInt];
    }

    public static List<string> GetEntityTypesInTransportData(TransportData td)
    {
        List<string> result = new List<string>();

        var keys = td.MainData.GetKeys();

        foreach (var tableName in keys)
        {
            string entityType = GetEntityTypeFromDatabaseTableName(tableName);
            if (entityType.Trim().Length > 0) result.Add(entityType);
        }

        return result;
    }

    public static string GetEntityTypePrimaryKeys(string entityType)
    {
        if (m_entityTypePrimaryKeys.ContainsKey(entityType)) return m_entityTypePrimaryKeys[entityType].Replace(" ", string.Empty);
        return string.Empty;
    }

    public static string GetEntityTypeFromDatabaseTableName(string tableName)
    {
        if (m_databaseTableToEntityTypeMapping.ContainsKey(tableName))
            return m_databaseTableToEntityTypeMapping[tableName];
        else return string.Empty;
    }

    public static string GetDatabaseTableNameFromEntityType(string entityType)
    {
        if (m_entityTypeToDatabaseTableMapping.ContainsKey(entityType))
            return m_entityTypeToDatabaseTableMapping[entityType];
        else return string.Empty;
    }

    public static Func<JObject, DataContainer, bool, bool, ReadonlyBaseObject> GetInstanceCreator(string entityType)
    {
        if (m_instanceCreators.ContainsKey(entityType)) return m_instanceCreators[entityType];
        return null;
    }

    public static Func<ReadonlyBaseObject> GetNewInstanceCreator(string entityType)
    {
        if (m_newInstanceCreators.ContainsKey(entityType)) return m_newInstanceCreators[entityType];
        return null;
    }

    public static List<ReadonlyBaseObject> FormulateDomainEntityListFromTransportData(string entityType,
                                                                              TransportData td,
                                                                              Func<JObject, bool> predicate,
                                                                              Func<JObject, string> sortFunction,
                                                                              bool allowEdit,
                                                                              bool allowInPlaceEditing)
    {
        if (m_domainEntityListFormulators.ContainsKey(entityType))
        {
            return m_domainEntityListFormulators[entityType](td, predicate, sortFunction, allowEdit, allowInPlaceEditing);
        }
        else
        {
            return new List<ReadonlyBaseObject>();
        }
    }

    public static void RegisterEntityTypePrimaryKeys(string entityType, string primaryKeys)
    {
        m_entityTypePrimaryKeys.Add(entityType, primaryKeys);
    }

    private static readonly Dictionary<string, string> m_entityTypeToDatabaseTableMapping = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> m_databaseTableToEntityTypeMapping = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> m_entityTypePrimaryKeys = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> m_entityTypeMergeKeys = new Dictionary<string, string>();
    private static readonly Dictionary<string, Func<TransportData, Func<JObject, bool>, Func<JObject, string>, bool, bool, List<ReadonlyBaseObject>>> m_domainEntityListFormulators = new();

    private static readonly Dictionary<string, string> m_inMemoryTablesRequired = new Dictionary<string, string>();

    private static readonly Dictionary<string, Func<JObject, DataContainer, bool, bool, ReadonlyBaseObject>> m_instanceCreators = new();
    private static readonly Dictionary<string, Func<ReadonlyBaseObject>> m_newInstanceCreators = new();

    public static void RegisterEntityTypeAspects(string entityType, string masterTableName, Func<TransportData, Func<JObject, bool>, 
                                                    Func<JObject, string>, bool, bool, List<ReadonlyBaseObject>> fn,
                                                    string singularName, string pluralName,
                                                    string primaryKeys,
                                                    Func<JObject, DataContainer, bool, bool, ReadonlyBaseObject> instanceCreator,
                                                    Func<ReadonlyBaseObject> newInstanceCreator,
                                                    Dictionary<string, string> inMemoryTablesRequired)
    {
        RegisterDatabaseTableMapping(entityType, masterTableName);
        RegisterDomainEntityListFormulator(entityType, fn);
        EntityNomenclature.RegisterEntityNomenclature(entityType, singularName, pluralName);
        RegisterInstanceCreator(entityType, instanceCreator);
        RegisterNewInstanceCreator(entityType, newInstanceCreator);
        RegisterEntityTypePrimaryKeys(entityType, primaryKeys);

        if (inMemoryTablesRequired != null)
        {
            foreach (var kvp in inMemoryTablesRequired)
            {
                m_inMemoryTablesRequired.Add(kvp.Key, kvp.Value);
            }
        }
    }

    public static void RegisterEntityTypeMergeKeys(string entityType, string mergeKeys)
    {
        m_entityTypeMergeKeys[entityType] = mergeKeys;
    }

    public static string GetEntityTypeMergeKeys(string entityType)
    {
        if (m_entityTypeMergeKeys.ContainsKey(entityType)) return m_entityTypeMergeKeys[entityType];
        return GetEntityTypePrimaryKeys(entityType);
    }

    public static Dictionary<string, string> GetInMemoryTablesRequired()
    {
        return m_inMemoryTablesRequired;
    }

    static void RegisterInstanceCreator(string entityType,
        Func<JObject, DataContainer, bool, bool, ReadonlyBaseObject> instanceCreator)
    {
        m_instanceCreators.Add(entityType, instanceCreator);
    }

    static void RegisterNewInstanceCreator(string entityType, Func<ReadonlyBaseObject> newInstanceCreator)
    {
        m_newInstanceCreators.Add(entityType, newInstanceCreator);
    }

    static void RegisterDatabaseTableMapping(string entityType, string tableName)
    {
        m_entityTypeToDatabaseTableMapping.Add(entityType, tableName);
        m_databaseTableToEntityTypeMapping.Add(tableName, entityType);
    }

    private static void RegisterDomainEntityListFormulator(string entityType, 
        Func<TransportData, Func<JObject, bool>, Func<JObject, string>, bool, bool, List<ReadonlyBaseObject>> fn)
    {
        m_domainEntityListFormulators.Add(entityType, fn);
    }

    public static List<string> GetAllRegisteredEntityTypes()
    {
        return m_entityTypePrimaryKeys.Keys.Select(k => k).ToList();
    }
}
