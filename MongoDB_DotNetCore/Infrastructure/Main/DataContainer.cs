using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;

public class DataContainer
{
    public List<string> UpdatedTableNames { get; set; } = new List<string>();

    private List<DataCollection> Collections { get; } = new List<DataCollection>();

    public void ClearAllCollections()
    {
        Collections.ForEach(coll => coll.Entries.Clear());
    }

    public void RemoveAllCollections()
    {
        Collections.Clear();
    }

    public int Count()
    {
        return Collections.Count;
    }

    public List<string> GetKeys()
    {
        return Collections.Select(dc => dc.Name).ToList();
    }

    public void CopyEntireContainerTo(DataContainer other)
    {
        foreach(var key in GetKeys())
        {
            CopyCollectionTo(other, key);
        }
    }

    public void CopyCollectionTo(DataContainer other, string collectionName, Predicate<JObject> selector = null)
    {
        var sourceCollection = GetOrCreateCollection(collectionName);
        sourceCollection.CopyTo(other, selector);
    }

    public DataCollection GetCollection(string key)
    {
        return Collections.Exists(dc => dc.Name == key) ? Collections.Find(dc => dc.Name == key) : new DataCollection();
    }

    public void CreateCollection(string key)
    {
        if (CollectionExists(key)) throw new DomainException($"A collection named {key} already exists.");
        Collections.Add(new DataCollection { Name = key });
    }

    public DataCollection GetOrCreateCollection(string key)
    {
        if (!CollectionExists(key)) CreateCollection(key);
        return GetCollection(key);
    }

    public bool CollectionExists(string key)
    {
        return Collections.Exists(dc => dc.Name == key);
    }

    public bool EntityExists(string entityType)
    {
        return CollectionExists(EntityTypes.GetDatabaseTableNameFromEntityType(entityType));
    }

    public void ClearCollection(string key)
    {
        if (CollectionExists(key)) GetCollection(key).Entries.Clear();
    }

    public JObject ConvertToJsonObject()
    {
        JObject result = new JObject();

        result.Add("UpdatedTableNames", new JArray(UpdatedTableNames));
        result.Add("Collections", new JArray(Collections.Select(dc => dc.ConvertToJsonObject())));

        return result;
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(ConvertToJsonObject());
    }

    public static DataContainer FromJsonObject(JObject data)
    {
        var result = new DataContainer();

        if (data.ContainsKey("UpdatedTableNames"))
        {
            foreach (var jt in data["UpdatedTableNames"].ToArray())
            {
                var tName = jt.ToObject<string>();
                result.UpdatedTableNames.Add(tName);
            }
        }

        if (data.ContainsKey("Collections"))
        {
            foreach (var jt in data["Collections"].ToArray())
            {
                var data_dc = jt.ToObject<JObject>();
                var dc = DataCollection.FromJsonObject(data_dc);
                result.Collections.Add(dc);
            }
        }

        return result;
    }

    public static DataContainer CreateOrGetFromTransportData(TransportData td)
    {
        if (td == null) return new DataContainer();
        return td.MainData;
    }
}
