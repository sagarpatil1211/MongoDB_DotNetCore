using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Infrastructure.Main;
//using VJ1Core.Infrastructure.Main.Entity_Metadata_Management;

public abstract class ReadonlyBaseObject
{
    protected ReadonlyBaseObject(JObject data = null, bool allowEdit = false, DataContainer parentDataContainer = null,
        bool allowInPlaceEdit = false)
    {
        BaseData = (data == null ? new JObject() : (allowEdit ? (allowInPlaceEdit ? data : new JObject(data)) : data));
        this.allowEdit = allowEdit;
        this.ParentDataContainer = parentDataContainer;
    }

    protected bool allowEdit = false;

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
            BaseData.Add(key, JToken.FromObject(value ?? new byte[] { }));
        }
        else
        {
            BaseData.Property(key).Value = JToken.FromObject(value ?? new byte[] { });
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

    // NOTE : THERE IS NO DIFFERENCE BETWEEN THE SIMPLE PROPERTY METHODS ABOVE AND THE COMPLEX PROPERTY METHODS BELOW

    protected List<T> GetComplexListPropertyValue<T>(string key)
    {
        var ary = GetJArrayPropertyValue(key);
        return ary.ToObject<List<T>>();
    }

    protected void SetComplexListPropertyValue<T>(string key, List<T> value, bool overrideAllowEdit = false)
    {
        var aryToSave = JArray.FromObject(value);
        SetJArrayPropertyValue(key, aryToSave, overrideAllowEdit);
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

    public virtual void MergeIntoTransportData(TransportData td)
    {
        MergeIntoDataContainer(td.MainData);
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
    }

    public string FormulateWhereClauseForSelectionFromDBWithPrimaryKeys(DbCommand cmd, string tableName)
    {
        var primaryKeyValueMap = GetPrimaryKeyValueMap();
        var conditionsAndParams = DomainUtils.FormulateQueryConditionStringAndParametersFromValueMap(cmd, tableName, primaryKeyValueMap);

        string strCondition = conditionsAndParams.Item1;
        return strCondition;
    }
}
