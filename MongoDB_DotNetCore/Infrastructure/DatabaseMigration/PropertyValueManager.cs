using Newtonsoft.Json.Linq;
using System.Collections.Generic;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class PropertyValueManager
{
    private readonly JObject values;

    public PropertyValueManager()
    {
        values = new JObject();
    }

    public PropertyValueManager(PropertyValueManager pvm) : this()
    {
        foreach (var prop in pvm.GetProperties())
            SetPropertyValue(prop.Name, prop.Value);
    }

    public void SetPropertyValue(string key, object value)
    {
        values[key] = JToken.FromObject(value);
    }

    public T GetPropertyValue<T>(string key)
    {
        if (values.ContainsKey(key) == false)
            return default;
        var v = values[key];
        return v.ToObject<T>();
    }

    public List<JProperty> GetProperties()
    {
        return new List<JProperty>(values.Properties());
    }
}