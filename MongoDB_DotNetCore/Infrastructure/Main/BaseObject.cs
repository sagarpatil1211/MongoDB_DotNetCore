using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VJCore.Infrastructure.Extensions;

public abstract class BaseObject
{
    protected BaseObject(JObject data = null, bool allowEdit = false, DataContainer parentDataContainer = null,
        bool allowInPlaceEdit = false)
    {
        BaseData = (data == null ? new JObject() : (allowEdit ? (allowInPlaceEdit ? data : new JObject(data)) : data));
        this.allowEdit = allowEdit;
        this.ParentDataContainer = parentDataContainer;
    }

    protected bool allowEdit = false;

    public bool DenormalizationProcessDone
    {
        get => GetBooleanPropertyValue("DenormalizationProcessDone");
        set => SetBooleanPropertyValue("DenormalizationProcessDone", value);
    }

    public JObject BaseData { get; set; }

    public DataContainer ParentDataContainer { get; set; } = null;

    public bool IsNewlyCreated() => BaseData.ContainsKey("IsNewlyCreated") && GetBooleanPropertyValue("IsNewlyCreated") == true;
    
    public void SetAsNewlyCreated()
    {
        SetBooleanPropertyValue("IsNewlyCreated", true, true);
    }

    public void UnsetAsNewlyCreated()
    {
        SetBooleanPropertyValue("IsNewlyCreated", false, true);
    }

    public bool GetBooleanPropertyValue(string key) => BaseData.ContainsKey(key) && Utils.GetBoolean(BaseData[key].ToObject(typeof(object)));
    public void SetBooleanPropertyValue(string key, bool value, bool overrideAllowEdit = false)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value);
        }
    }

    //public T GetEnumPropertyValue<T>(string key) where T : Enum => BaseData.ContainsKey(key) ? (T)Enum.Parse(typeof(T), Utils.GetString(BaseData[key])) : default(T);

    public int GetIntegerPropertyValue(string key) => BaseData.ContainsKey(key) ? Utils.GetInt32(BaseData[key].ToObject(typeof(object))) : 0;
    public void SetIntegerPropertyValue(string key, int value)
    {
        if (!allowEdit) ThrowNonEditableExceptionIfNecessary();
        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value);
        }
    }

    public long GetLongPropertyValue(string key) => BaseData.ContainsKey(key) ? Utils.GetInt64(BaseData[key].ToObject(typeof(object))) : 0L;
    public void SetLongPropertyValue(string key, long value, bool overrideAllowEdit = false)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value);
        }
    }

    public long[] GetLongArrayPropertyValue(string keys)
    {
        var lstResult = new List<long>();

        foreach (var key in keys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            lstResult.Add(GetLongPropertyValue(key));
        }

        return lstResult.ToArray();
    }

    public string GetStringPropertyValue(string key) => BaseData.ContainsKey(key) ? Utils.GetString(BaseData[key].ToObject(typeof(object))) : string.Empty;
    public void SetStringPropertyValue(string key, string value, bool overrideAllowEdit = false)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value ?? string.Empty));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value ?? string.Empty);
        }
    }

    public decimal GetDecimalPropertyValue(string key, int decimalPlaces) => BaseData.ContainsKey(key) ? Utils.GetDecimal(BaseData[key].ToObject(typeof(object)), decimalPlaces) : 0M;
    public void SetDecimalPropertyValue(string key, decimal value, bool overrideAllowEdit = false)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value);
        }
    }

    public object GetObjectPropertyValue(string key) => BaseData.ContainsKey(key) ? BaseData[key].ToObject(typeof(object)) : null;
    public void SetObjectPropertyValue(string key, string value)
    {
        if (!allowEdit) ThrowNonEditableExceptionIfNecessary();
        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value ?? string.Empty));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value ?? string.Empty);
        }
    }

    public T GetObjectPropertyValue<T>(string key) => BaseData.ContainsKey(key) ? (T)(BaseData[key].ToObject(typeof(object))) : default;
    public void SetObjectPropertyValue<T>(string key, T value, bool overrideAllowEdit)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value);
        }
    }

    public byte[] GetByteArrayPropertyValue(string key) => BaseData.ContainsKey(key) ? (byte[])BaseData[key].ToObject(typeof(object)) : new byte[] { 0x00 };
    public void SetByteArrayPropertyValue(string key, byte[] value, bool overrideAllowEdit)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value ?? Array.Empty<byte>()));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value ?? Array.Empty<byte>());
        }
    }

    protected JArray GetJArrayPropertyValue(string key) => BaseData.ContainsKey(key) ? (JArray)BaseData[key].ToObject(typeof(object)) : new JArray();
    protected void SetJArrayPropertyValue(string key, JArray value, bool overrideAllowEdit = false)
    {
        if (!allowEdit)
        {
            if (!overrideAllowEdit) ThrowNonEditableExceptionIfNecessary();
        }

        if (!BaseData.ContainsKey(key))
        {
            BaseData.Add(key, JToken.FromObject(value ?? new JArray()));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value ?? new JArray());
        }
    }

    protected List<T> GetSimpleListPropertyValue<T>(string key)
    {
        var ary = GetJArrayPropertyValue(key);
        return ary.ToObject<List<T>>();
    }

    protected void SetSimpleListPropertyValue<T>(string key, List<T> value, bool overrideAllowEdit = false)
    {
        var aryToSave = JArray.FromObject(value);
        SetJArrayPropertyValue(key, aryToSave, overrideAllowEdit);
    }

    protected List<T> GetComplexListPropertyValue<T>(string key) where T : BaseObject, IInstantiable<T>
    {
        string keyInternal = $"{key}";
        var ary = GetJArrayPropertyValue(keyInternal);
        return ary.ToObject<List<JObject>>().Select(e => T.CreateInstance(e, ParentDataContainer, allowEdit)).ToList();
    }

    protected void SetComplexListPropertyValue<T>(string key, List<T> value, bool overrideAllowEdit = false) where T : BaseObject, IInstantiable<T>
    {
        string keyInternal = $"{key}";
        var aryToSave = JArray.FromObject(value.Select(e => e.BaseData).ToList());
        SetJArrayPropertyValue(keyInternal, aryToSave, overrideAllowEdit);
    }

    protected List<T> GetReadonlyComplexListPropertyValue<T>(string key) where T : ReadonlyBaseObject, IInstantiable<T>
    {
        string keyInternal = $"{key}_ReadonlyListData";
        var ary = GetJArrayPropertyValue(keyInternal);
        return ary.ToObject<List<JObject>>().Select(e => T.CreateInstance(e, ParentDataContainer, allowEdit)).ToList();
    }

    protected void SetReadonlyComplexListPropertyValue<T>(string key, List<T> value, bool overrideAllowEdit = false) where T : ReadonlyBaseObject, IInstantiable<T>
    {
        string keyInternal = $"{key}_ReadonlyListData";
        var aryToSave = JArray.FromObject(value.Select(e => e.BaseData).ToList());
        SetJArrayPropertyValue(keyInternal, aryToSave, overrideAllowEdit);
    }

    public DateTime GetDateTimePropertyValue(string key)
    {
        var stringValue = GetStringPropertyValue(key);
        return DTU.FromString(stringValue);
    }

    public void SetDateTimePropertyValue(string key, DateTime value)
    {
        SetStringPropertyValue(key, DTU.ConvertToString(value));
    }

    protected void ThrowNonEditableExceptionIfNecessary()
    {
        if (!allowEdit) throw new DomainException("Please use the editable version!");
    }

    protected virtual List<BaseObject> DependentEntityGenerator(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        return new List<BaseObject>();
    }

    public void GenerateAndMergeDependentEntities(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        var deps = DependentEntityGenerator(cmd, td, vrec);
        foreach (BaseObject dep in deps)
        {
            dep.MergeIntoTransportData(td);
            dep.GenerateAndMergeDependentEntities(cmd, td, vrec);
        }
    }

    public virtual void PerformPreSaveValidationProcess(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec) { }

    public virtual void SetComputedPropertyValues(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec)
    {

    }

    protected virtual void SaveValidator(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec) { }

    public virtual void PerformPreSaveProcess(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec) { }

    public void CheckSaveValidity(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        if (!allowEdit)
        {
            vrec.AddToList(string.Empty, "This object is not editable and cannot be saved.");
            return;
        }

        SaveValidator(cmd, td, vrec);
        CheckForUniqueConstraintViolations(cmd, td, vrec);
        EnsureReferentialIntegrityDuringSave(cmd, td, vrec);
    }

    public virtual void InvokePreCommitProcess(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec)
    {

    }

    public virtual void PerformDenormalizationProcesses(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec)
    {

    }

    public virtual void DeleteDependentEntities(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec)
    {

    }

    public virtual void InvokePostSaveProcess(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec) { }

    private Dictionary<string, int> GetUsageMapForReferentialIntegrityCheck(DbCommand cmd)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();

        string entityType = this.GetType().FullName;

        List<EntityRelationship> lstEntityRelationships
            = EntityRelationshipManager.GetChildRelationshipsForEntityType(entityType);

        foreach (EntityRelationship entityRelationship in lstEntityRelationships)
        {
            cmd.Parameters.Clear();

            string childEntityTableName = entityRelationship.ChildEntityTableName;
            string parentEntityForeignKeysInChildTable = entityRelationship.ParentEntityForeignKeyColumnNames;

            //string parentEntityId = GetStringPropertyValue(parentEntityForeignKeysInChildTable);
            string parentEntityId = GetPrimaryKeyValuesString();
            cmd.CommandText = $"select count(*) as result from {childEntityTableName}" +
                $" where {parentEntityForeignKeysInChildTable} = @parententityid";

            if (!parentEntityId.Contains(','))
            {
                cmd.AddLongParameter("@parententityid", long.Parse(parentEntityId));
            }
            else
            {
                cmd.AddVarcharParameter("@parententityid", 512, parentEntityId);
            }

            var count = Utils.GetInt32(cmd.ExecuteScalar2());

            if (count > 0)
            {
                result.Add(entityRelationship.ChildEntityType, count);
            }
        }

        return result;
    }

    public TransactionResult CheckDeletionValidity(DbCommand cmd)
    {
        TransactionResult result = new TransactionResult();

        var dis = SessionController.DIS;

        if (dis == null)
        {
            result.Successful = true;
            return result;
        }

        var dMap = GetUsageMapForReferentialIntegrityCheck(cmd);

        if (dMap.Count == 0)
        {
            result.Successful = true;
        }
        else
        {
            string message = FormulateReferentialIntegrityViolationExceptionMessageDuringDeletion(dMap);
            result.Successful = false;
            result.Message = message;
        }

        return result;
    }

    static Dictionary<string, Object> FormulateForeignKeyValueMap(string foreignKeys, params long[] foreignKeyValues)
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();

        var lstKeys = foreignKeys.Split(',', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lstKeys.Length; i++)
        {
            if (foreignKeyValues.Length > i)
            {
                string fieldName = lstKeys[i];
                if (fieldName.Trim().Length > 0)
                {
                    Object fieldValue = ((JToken)foreignKeyValues[i]).ToObject<Object>();
                    result.Add(fieldName, fieldValue);
                }
            }
        }

        return result;
    }

    public bool SkipUniqueContraintCheck
    {
        get => GetBooleanPropertyValue("OverrideUniqueContraintCheck");
        set => SetBooleanPropertyValue("OverrideUniqueContraintCheck", value);
    }

    protected void CheckForUniqueConstraintViolations(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        if (!SkipUniqueContraintCheck)
        {
            string entityType = this.GetType().FullName;

            List<string> uniqueConstraintGroupNames = EntityConstraintsManager.GetUniqueConstraintGroupNamesForEntityType(entityType);
            List<string> primaryKeys = new List<string>(EntityTypes.GetEntityTypePrimaryKeys(entityType).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            StringBuilder sbMyPrimaryKeyValues = new StringBuilder();

            var myPrimaryKeyValues = GetPrimaryKeyValueMap();
            foreach (var pk in primaryKeys)
            {
                if (sbMyPrimaryKeyValues.Length > 0) sbMyPrimaryKeyValues.Append(',');
                sbMyPrimaryKeyValues.Append(myPrimaryKeyValues[pk]);
            }

            var strMyPrimaryKeyValues = sbMyPrimaryKeyValues.ToString();

            bool violationFound = false;

            foreach (var uniqueConstraintGroupName in uniqueConstraintGroupNames)
            {
                var uniquePropNames = EntityConstraintsManager.GetUniquePropertyNamesForEntityType(entityType, uniqueConstraintGroupName);

                var lstUniquePropValues = new List<object>();
                foreach (var propName in uniquePropNames)
                {
                    lstUniquePropValues.Add(GetObjectPropertyValue(propName));
                }

                var aryUniquePropValues = lstUniquePropValues.ToArray();

                var strUniquePropNames = Utils.CombineListIntoSingleString(uniquePropNames, ',');

                var masterTableName = EntityTypes.GetDatabaseTableNameFromEntityType(entityType);

                var coll = td.MainData.GetCollection(masterTableName);

                if (coll != null)
                {
                    var data_other = ClassService.GetMatchingData(coll.Entries, strUniquePropNames, aryUniquePropValues);

                    if (data_other != null)
                    {
                        StringBuilder sbOtherPrimaryKeyValues = new StringBuilder();

                        var other_primaryKeyValues = GetPrimaryKeyValueMapFromDataObject(entityType, data_other);

                        foreach (var pk in primaryKeys)
                        {
                            if (sbOtherPrimaryKeyValues.Length > 0) sbOtherPrimaryKeyValues.Append(',');
                            sbOtherPrimaryKeyValues.Append(other_primaryKeyValues[pk]);
                        }

                        var strOtherPrimaryKeyValues = sbOtherPrimaryKeyValues.ToString();

                        if (strOtherPrimaryKeyValues != strMyPrimaryKeyValues)
                        {
                            violationFound = true;

                            var violationMessage = EntityConstraintsManager.GetUniqueConstraintViolationMessage(entityType, uniqueConstraintGroupName);

                            if (violationMessage.Trim().Length > 0)
                            {
                                vrec.AddToList(string.Empty, violationMessage);
                            }
                        }
                    }
                }

                if (!violationFound)
                {
                    var uniquePropertyValueMap = GetPropertyValueMapFromDataObject(BaseData, strUniquePropNames);

                    var conditionsAndParams = DomainUtils.FormulateQueryConditionStringAndParametersFromValueMap(cmd, masterTableName, uniquePropertyValueMap);

                    string strCondition = conditionsAndParams.Item1;
                    var paramsMap = conditionsAndParams.Item2;

                    string tableName = EntityTypes.GetDatabaseTableNameFromEntityType(entityType);

                    var collFromDB = new DataCollection();

                    string strQuery = $"select * from {tableName}";
                    if (strCondition.Trim().Length > 0) strQuery += $" where {strCondition}";

                    SessionController.DAU.FillByQuery(cmd, collFromDB, strQuery, paramsMap);

                    foreach (JObject data_other in collFromDB.Entries)
                    {
                        StringBuilder sbOtherPrimaryKeyValues = new StringBuilder();

                        var other_primaryKeyValues = GetPrimaryKeyValueMapFromDataObject(entityType, data_other);

                        foreach (var pk in primaryKeys)
                        {
                            if (sbOtherPrimaryKeyValues.Length > 0) sbOtherPrimaryKeyValues.Append(',');
                            sbOtherPrimaryKeyValues.Append(other_primaryKeyValues[pk]);
                        }

                        var strOtherPrimaryKeyValues = sbOtherPrimaryKeyValues.ToString();

                        if (strOtherPrimaryKeyValues != strMyPrimaryKeyValues)
                        {
                            violationFound = true;

                            var violationMessage = EntityConstraintsManager.GetUniqueConstraintViolationMessage(entityType, uniqueConstraintGroupName);

                            if (violationMessage.Trim().Length > 0)
                            {
                                vrec.AddToList(string.Empty, violationMessage);
                            }
                        }
                    }
                }
            }
        }
    }

    protected virtual void EnsureReferentialIntegrityDuringSave(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        string entityType = this.GetType().FullName;

        var DAU = SessionController.DAU;

        List<EntityRelationship> lstEntityRelationships = EntityRelationshipManager.GetParentRelationshipsForEntityType(entityType);

        foreach (EntityRelationship entityRelationship in lstEntityRelationships)
        {
            if (!entityRelationship.NullParentAllowed)
            {
                string parentEntityType = entityRelationship.ParentEntityType;

                string parentEntityForeignKeys = entityRelationship.ParentEntityForeignKeyColumnNames;
                long[] parentEntityForeignKeyValues = GetLongArrayPropertyValue(parentEntityForeignKeys);

                var strParentEntityForeignKeyValues = Utils.FormulateRefsString(parentEntityForeignKeyValues);

                string parentEntityTableName = string.Empty;

                if (parentEntityType == string.Empty)   // Special case when any one type of parent entity
                                                        // out of multiple possibilities is 
                                                        // referrable from this entity, the system should look
                                                        // into the EntityRegister Table
                {
                    var strQuery = "select * from entityregister where primarykeyvalues = @PrimaryKeyValues";

                    var collEntityRegister = new DataCollection();

                    DbParameterContainer parameters = new DbParameterContainer();
                    parameters.AddVarchar("@PrimaryKeyValues", 512, strParentEntityForeignKeyValues);

                    DAU.FillByQuery(cmd, collEntityRegister, strQuery, parameters);

                    if (collEntityRegister.Entries.Count == 0)
                    {
                        vrec.AddToList(string.Empty, $"The entity referred to does not exist.");
                        return;
                    }
                    else
                    {
                        var dataEntityType = collEntityRegister.Entries[0];
                        parentEntityTableName = Utils.GetString(dataEntityType["TableName"]);
                    }
                }
                else
                {
                    parentEntityTableName = EntityTypes.GetDatabaseTableNameFromEntityType(parentEntityType);
                }

                string parentEntitySingularTypeName = EntityNomenclature.GetSingularName(parentEntityType);

                string parentEntityPrimaryKeys = EntityTypes.GetEntityTypePrimaryKeys(parentEntityType);

                if (parentEntityForeignKeyValues.Length == 0)
                {
                    vrec.AddToList(parentEntityForeignKeys, $"{parentEntitySingularTypeName} cannot be null.");
                }
                else
                {
                    bool parentEntityExists = DomainUtils.DomainEntityDictionaryExistsByPrimaryKeyValues(parentEntityTableName, parentEntityPrimaryKeys, parentEntityForeignKeyValues);

                    if (!parentEntityExists)
                    {
                        parentEntityExists = DomainUtils.DomainEntityDictionaryExistsByPrimaryKeyValues(td, parentEntityTableName, parentEntityPrimaryKeys, parentEntityForeignKeyValues);
                    }

                    if (!parentEntityExists)
                    {
                        var foreignKeyValueMap = FormulateForeignKeyValueMap(parentEntityPrimaryKeys, parentEntityForeignKeyValues);
                        var conditionsAndParams = DomainUtils.FormulateQueryConditionStringAndParametersFromValueMap(cmd, parentEntityTableName, foreignKeyValueMap);

                        var strConditions = conditionsAndParams.Item1;
                        var paramsMap = conditionsAndParams.Item2;

                        string strQuery = $"select count(*) from {parentEntityTableName}";
                        if (strConditions.Trim().Length > 0) strQuery += $" where {strConditions}";

                        cmd.Parameters.Clear();

                        cmd.CommandText = strQuery;
                        paramsMap.AttachToCommand(cmd);

                        var count = Utils.GetInt32(cmd.ExecuteScalar2());
                        parentEntityExists = (count > 0);
                    }

                    if (!parentEntityExists)
                    {
                        vrec.AddToList(parentEntityForeignKeys, $"The {parentEntitySingularTypeName} specified does not exist.");
                    }
                }
            }
        }
    }

    private string FormulateReferentialIntegrityViolationExceptionMessageDuringDeletion(Dictionary<string, int> dMap)
    {
        List<string> lstMessages = new List<string>();

        string entityType = this.GetType().FullName;

        string parentSingularName = EntityNomenclature.GetSingularName(entityType);

        foreach (string childEntityType in dMap.Keys)
        {
            string childSingularName = EntityNomenclature.GetSingularName(childEntityType);
            string childPluralName = EntityNomenclature.GetPluralName(childEntityType);

            string childEntityTypeName = childPluralName;

            int dependentCount = dMap[childEntityType];
            string referenceIndicator = "refer";

            if (dependentCount == 1)
            {
                childEntityTypeName = childSingularName;
                referenceIndicator = "refers";
            }

            lstMessages.Add($"You cannot delete {parentSingularName} because {dependentCount} {childEntityTypeName} {referenceIndicator} to it.");
        }

        string message = Utils.CombineListIntoSingleString(lstMessages, '\n');
        return message;
    }

    public Dictionary<string, object> GetPrimaryKeyValueMap()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        var entityType = this.GetType().FullName;
        string strPrimaryKeys = EntityTypes.GetEntityTypePrimaryKeys(entityType);

        foreach (string key in strPrimaryKeys.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            string pkName = key.Trim();

            if (pkName.Length > 0)
            {
                result.Add(pkName, GetObjectPropertyValue(pkName));
            }
        }

        return result;
    }

    public string GetPrimaryKeyValuesString()
    {
        string entityType = this.GetType().FullName;

        long[] primaryKeyValues = GetLongArrayPropertyValue(EntityTypes.GetEntityTypePrimaryKeys(entityType));

        return Utils.FormulateRefsString(primaryKeyValues);
    }

    public long[] FormulatePrimaryKeyValueArray()
    {
        string entityType = this.GetType().FullName;
        return GetLongArrayPropertyValue(EntityTypes.GetEntityTypePrimaryKeys(entityType));
    }

    public long[] FormulateMergeKeyValueArray()
    {
        string entityType = this.GetType().FullName;
        return GetLongArrayPropertyValue(EntityTypes.GetEntityTypeMergeKeys(entityType));
    }

    private static Dictionary<string, Object> GetPrimaryKeyValueMapFromDataObject(string entityType, JObject data)
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();

        string strPrimaryKeys = EntityTypes.GetEntityTypePrimaryKeys(entityType);

        foreach (string key in strPrimaryKeys.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            string pkName = key.Trim();

            if (pkName.Length > 0)
            {
                result.Add(pkName, GetObjectPropertyValueFromDataObject(data, pkName));
            }
        }

        return result;
    }

    private static object GetObjectPropertyValueFromDataObject(JObject data, string key) => data.ContainsKey(key) ? data[key].ToObject(typeof(object)) : null;

    private static Dictionary<string, Object> GetPropertyValueMapFromDataObject(JObject data, string propertyNames)
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();

        foreach (string key in propertyNames.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            string pkName = key.Trim();

            if (pkName.Length > 0)
            {
                result.Add(pkName, GetObjectPropertyValueFromDataObject(data, pkName));
            }
        }

        return result;
    }

    public virtual void DeleteChildEntitiesFromDB(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        string entityType = this.GetType().FullName;

        List<EntityRelationship> lstParentChildEntityRelationships
            = EntityRelationshipManager.GetChildRelationshipsForEntityType(entityType)
                .Where(e => e.RelationshipType == EntityRelationshipTypes.Parent_Child)
                .ToList();

        foreach (var rel in lstParentChildEntityRelationships)
        {
            string childEntityType = rel.ChildEntityType;
            string childEntityMasterTableName = EntityTypes.GetDatabaseTableNameFromEntityType(childEntityType);
        }
    }

    public virtual void MergeIntoTransportData(TransportData td)
    {
        MergeIntoDataContainer(td.MainData);

        //string entityType = this.GetType().FullName;

        //List<EntityRelationship> lstParentChildEntityRelationships
        //    = EntityRelationshipManager.GetChildRelationshipsForEntityType(entityType)
        //        .Where(e => e.RelationshipType == EntityRelationshipTypes.Parent_Child)
        //        .ToList();

        //foreach (var rel in lstParentChildEntityRelationships)
        //{
        //    JArray childBaseDataCollection = BaseData[rel.ChildEntityCollectionPropertyNameInParent].ToObject<JArray>();
        //    foreach (JObject joChild in childBaseDataCollection)
        //    {
        //        var instanceCreator = EntityTypes.GetInstanceCreator(rel.ChildEntityType);
        //        if (instanceCreator != null)
        //        {
        //            BaseObject childObject = instanceCreator(joChild, td.MainData, true, false);
        //            childObject.MergeIntoTransportData(td);
        //        }
        //    }
        //}
    }

    public void MergeIntoDataContainer(DataContainer cont)
    {
        string entityType = this.GetType().FullName;
        var masterTableName = EntityTypes.GetDatabaseTableNameFromEntityType(entityType);

        var mergeKeys = EntityTypes.GetEntityTypeMergeKeys(entityType);

        var coll = cont.GetOrCreateCollection(masterTableName);
        var existingData = DomainUtils.GetDomainEntityDictionaryByPrimaryKeyValues(cont, masterTableName, mergeKeys, 
            FormulateMergeKeyValueArray());
        if (existingData == null)
        {
            coll.Entries.Add(BaseData);
        }
        else
        {
            ClassService.TransferData(BaseData, existingData);
        }

        //var primaryKeys = EntityTypes.GetEntityTypePrimaryKeys(entityType);

        //var coll = cont.GetOrCreateCollection(masterTableName);
        //var existingData = DomainUtils.GetDomainEntityDictionaryByPrimaryKeyValues(cont, masterTableName, primaryKeys, FormulatePrimaryKeyValueArray());
        //if (existingData == null)
        //{
        //    coll.Entries.Add(BaseData);
        //}
        //else
        //{
        //    ClassService.TransferData(BaseData, existingData);
        //}
    }

    public string FormulateWhereClauseForSelectionFromDBWithPrimaryKeys(DbCommand cmd, string tableName)
    {
        var primaryKeyValueMap = GetPrimaryKeyValueMap();
        var conditionsAndParams = DomainUtils.FormulateQueryConditionStringAndParametersFromValueMap(cmd, tableName, primaryKeyValueMap);

        string strCondition = conditionsAndParams.Item1;
        return strCondition;
    }
}
