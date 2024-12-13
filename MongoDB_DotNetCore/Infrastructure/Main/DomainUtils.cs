using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
//using VJ1Core.Infrastructure.Main;
//using VJ1Core.Infrastructure.Main.Entity_Metadata_Management;

public class DomainUtils
{
    public static Tuple<string, DbParameterContainer> FormulateQueryConditionStringAndParametersFromValueMap(DbCommand cmd, string dbObjectName, Dictionary<string, object> map)
    {
        TemplateDbParameterContainer contTemplate = DataAccessUtils.GetTemplateDbParameterContainer(cmd, dbObjectName);

        string strCondition = string.Empty;
        //Dictionary<string, Object> parameters = new Dictionary<string, Object>();

        foreach (var kvp in map)
        {
            if (strCondition.Length > 0) strCondition += " and ";
            string fName = kvp.Key;
            if (fName.StartsWith("@")) fName = fName.Substring(1);
            string parameterName = $"@{fName}";
            strCondition += $"{fName} = {parameterName}";
        }

        DbParameterContainer cont = new DbParameterContainer();
        cont.FormulateParameterValuesFromTemplateAndDictionary(contTemplate, map);

        return Tuple.Create(strCondition, cont);
    }

    private static Func<JObject, bool> FormulateSelectorFunctionFromValueMap(Dictionary<string, Object> map)
    {
        Func<JObject, bool> result = (JObject jo) =>
        {
            foreach (var kvp in map)
            {
                string fName = kvp.Key;
                //if (!(jo[fName].Value<Object>().Equals(kvp.Value)))
                if (!(jo[fName].ToObject(typeof(object)).Equals(kvp.Value)))
                {
                    return false;
                }
            }

            return true;
        };

        return result;
    }

    private static JObject GetDomainEntityDictionaryByPrimaryKeyValues(string masterTableName, string primaryKeys,
                                                                        params long[] primaryKeyValues)
    {
        var map_primaryKeyValues = FormulatePrimaryKeyValueMap(primaryKeys, primaryKeyValues);
        var selectorFunction = FormulateSelectorFunctionFromValueMap(map_primaryKeyValues);

        return GetDomainEntityDictionaryByFilter(masterTableName, selectorFunction);
    }

    private static JObject GetDomainEntityDictionaryByPrimaryKeyValues(TransportData ds, string masterTableName, string primaryKeys,
                                                                        params long[] primaryKeyValues)
    {
        return GetDomainEntityDictionaryByPrimaryKeyValues(ds.MainData, masterTableName, primaryKeys, primaryKeyValues);
    }

    public static JObject GetDomainEntityDictionaryByPrimaryKeyValues(DataContainer cont, string masterTableName, string primaryKeys,
                                                                        params long[] primaryKeyValues)
    {
        var map_primaryKeyValues = FormulatePrimaryKeyValueMap(primaryKeys, primaryKeyValues);
        var selectorFunction = FormulateSelectorFunctionFromValueMap(map_primaryKeyValues);

        return GetDomainEntityDictionaryByFilter(cont, masterTableName, selectorFunction);
    }


    private static JObject GetDomainEntityDictionaryByFilter(string masterTableName, Func<JObject, bool> selectorFunction)
    {
        foreach (JObject obj in InMemoryData.GetInstance().GetCollection(masterTableName).Entries)
        {
            if (selectorFunction(obj)) return obj;
        }

        return null;
    }

    private static JObject GetDomainEntityDictionaryByFilter(TransportData td, string masterTableName, Func<JObject, bool> selectorFunction)
    {
        return GetDomainEntityDictionaryByFilter(td.MainData, masterTableName, selectorFunction);
    }

    public static JObject GetDomainEntityDictionaryByFilter(DataContainer cont, string masterTableName, Func<JObject, bool> selectorFunction)
    {
        foreach (JObject obj in cont.GetCollection(masterTableName).Entries)
        {
            if (selectorFunction(obj)) return obj;
        }

        return null;
    }

    public static bool DomainEntityDictionaryExistsByPrimaryKeyValues(string masterTableName, string primaryKeys,
                                                                     params long[] primaryKeyValues)
    {
        var r = GetDomainEntityDictionaryByPrimaryKeyValues(masterTableName, primaryKeys, primaryKeyValues);
        if (r == null) return false;
        return true;
    }

    public static bool DomainEntityDictionaryExistsByPrimaryKeyValues(TransportData td, string masterTableName, string primaryKeys,
                                                                        params long[] primaryKeyValues)
    {
        var r = GetDomainEntityDictionaryByPrimaryKeyValues(td, masterTableName, primaryKeys, primaryKeyValues);
        if (r == null) return false;
        return true;
    }

    public static bool DomainEntityDictionaryExistsByFilter(string masterTableName, Func<JObject, bool> selectorFunction)
    {
        var r = GetDomainEntityDictionaryByFilter(masterTableName, selectorFunction);
        if (r == null) return false;
        return true;
    }

    public static bool DomainEntityDictionaryExistsByFilterString(TransportData td, string masterTableName,
                                                                    Func<JObject, bool> selectorFunction)
    {
        var r = GetDomainEntityDictionaryByFilter(td, masterTableName, selectorFunction);
        if (r == null) return false;
        return true;
    }

    private static Dictionary<string, Object> FormulatePrimaryKeyValueMap(string primaryKeys, params long[] primaryKeyValues)
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();

        var lstPrimaryKeys = primaryKeys.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (lstPrimaryKeys.Length != primaryKeyValues.Length)
        {
            throw new DomainException("Key Count and Value count do not match.");
        }

        for (int i = 0; i < lstPrimaryKeys.Length; i++)
        {
            string pkName = lstPrimaryKeys[i].Trim();

            if (pkName.Length > 0)
            {
                Object pkValue = primaryKeyValues[i];
                result.Add(pkName, pkValue);
            }
        }

        return result;
    }

    public static void FormulateFilledDataContainerWithDataObjects(DbCommand cmd, DataContainer dc, string entityType, 
        params long[] primaryKeyValues)
    {
        DataAccessUtils DAU = SessionController.DAU;

        var dbTableName = EntityTypes.GetDatabaseTableNameFromEntityType(entityType);

        var primaryKeyValueMap = FormulatePrimaryKeyValueMap(EntityTypes.GetEntityTypePrimaryKeys(entityType), primaryKeyValues);
        var conditionsAndParams = FormulateQueryConditionStringAndParametersFromValueMap(cmd, dbTableName, primaryKeyValueMap);

        string strCondition = conditionsAndParams.Item1;
        var paramsMap = conditionsAndParams.Item2;

        string strSelectQuery = $"select * from {dbTableName}";
        if (strCondition.Trim().Length > 0) strSelectQuery += $" where {strCondition}";

        //DAU.FillByQuery(DAU.CreateCommand(), dc, dbTableName, strSelectQuery, paramsMap);

        DAU.FillByQuery(cmd, dc, dbTableName, strSelectQuery, paramsMap);
    }

    public static DataContainer FormulateFilledDataContainerWithDataObjects(DbCommand cmd, string entityType, 
        params long[] primaryKeyValues)
    {
        var dc = new DataContainer();
        FormulateFilledDataContainerWithDataObjects(cmd, dc, entityType, primaryKeyValues);

        return dc;
    }

    public static List<JObject> GetChildEntitiesDataList(DataContainer cont, string parentEntityType, JObject parentEntityData, string childEntityType)
    {
        if (cont == null) return new List<JObject>();

        var childEntityTableName = EntityTypes.GetDatabaseTableNameFromEntityType(childEntityType);
        if (!cont.CollectionExists(childEntityTableName)) return new List<JObject>();

        var childEntityForeignKeyColumns = EntityRelationshipManager.GetForeignKeysInChildEntityTypeForParentEntityType(childEntityType, parentEntityType);
        var lstChildEntityForeignKeyPropertyNames = new List<string>(childEntityForeignKeyColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

        var parentEntityPrimaryKeyColumns = EntityTypes.GetEntityTypePrimaryKeys(parentEntityType);
        var lstParentEntityPrimaryKeyPropertyNames = new List<string>(parentEntityPrimaryKeyColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

        var keyCount = lstChildEntityForeignKeyPropertyNames.Count;

        List<long> parentEntityPrimaryKeyValues = new List<long>();

        for (int i = 0; i < lstParentEntityPrimaryKeyPropertyNames.Count; i++)
        {
            var key = lstParentEntityPrimaryKeyPropertyNames[i];
            var value = parentEntityData.ContainsKey(key) ? Utils.GetInt64(parentEntityData[key].ToObject(typeof(object))) : 0L;

            parentEntityPrimaryKeyValues.Add(value);
        }

        var result = new List<JObject>();

        foreach(JObject data in cont.GetCollection(childEntityTableName).Entries)
        {
            var matched = true;

            for (int i = 0; i < lstChildEntityForeignKeyPropertyNames.Count; i++)
            {
                var key = lstChildEntityForeignKeyPropertyNames[i];
                var value = data.ContainsKey(key) ? Utils.GetInt64(data[key].ToObject(typeof(object))) : 0L;

                if (value != parentEntityPrimaryKeyValues[i])
                {
                    matched = false;
                    break;
                }
            }

            if (matched) result.Add(data);
        }

        return result;
    }
}

