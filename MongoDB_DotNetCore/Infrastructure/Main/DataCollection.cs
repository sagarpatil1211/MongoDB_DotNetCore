using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class DataCollection
{
    public string Name { get; set; } = string.Empty;
    public JObject ColumnDefinitions { get; private set; } = new JObject();
    public List<JObject> Entries { get; } = new List<JObject>();

    public void CopyTo(DataCollection other, Predicate<JObject> selector = null)
    {
        other.Entries.Clear();
        if (selector == null)
        {
            other.Entries.AddRange(Entries.ToArray());
        }
        else
        {
            other.Entries.AddRange(Entries.FindAll(selector));
        }
    }

    public void CopyTo(DataContainer other, Predicate<JObject> selector = null)
    {
        CopyTo(other.GetOrCreateCollection(Name), selector);
    }

    public JObject ConvertToJsonObject()
    {
        JObject result = new JObject
        {
            { "Name", Name },
            { "ColumnDefinitions", ColumnDefinitions },
            { "Entries", new JArray(Entries.ToArray()) }
        };

        return result;
    }

    public static DataCollection FromJsonObject(JObject data)
    {
        var result = new DataCollection();

        if (data.ContainsKey("Name")) result.Name = data["Name"].ToString();
        if (data.ContainsKey("ColumnDefinitions")) result.ColumnDefinitions = data["ColumnDefinitions"].ToObject<JObject>();
        if (data.ContainsKey("Entries"))
        {
            result.Entries.AddRange(data["Entries"].Select(e => e.ToObject<JObject>()));
        }

        return result;
    }
}
