using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class TransportData
{
    public string RequestType { get; set; } = string.Empty;
    public string ProcessToken { get; set; } = string.Empty;

    public Dictionary<string, object> TempTags = new Dictionary<string, object>();

    public List<string> InMemoryDataRepositoryNamesToBeFetched { get; private set; } = new List<string>();

    public DataContainer MainData { get; private set; } = new DataContainer();

    public string ConvertToJsonObject()
    {
        JObject result = new JObject
        {
            { "RequestType", RequestType },
            { "ProcessToken", ProcessToken },
            { "InMemoryDataRepositoryNamesToBeFetched", JToken.FromObject(InMemoryDataRepositoryNamesToBeFetched) },
            { "MainData", MainData.ConvertToJsonObject() }
        };

        return JsonConvert.SerializeObject(result);
    }

    public List<Action<TransportData>> CustomProcessPostCommitActions { get; } = new List<Action<TransportData>>();

    private static TransportData FromJsonObject(JObject data)
    {
        var result = new TransportData();

        if (data.ContainsKey("RequestType")) result.RequestType = data["RequestType"].ToString();
        if (data.ContainsKey("ProcessToken")) result.ProcessToken = data["ProcessToken"].ToString();
        if (data.ContainsKey("InMemoryDataRepositoryNamesToBeFetched")) result.InMemoryDataRepositoryNamesToBeFetched = new List<string>(data["InMemoryDataRepositoryNamesToBeFetched"].ToObject<string[]>());
        if (data.ContainsKey("MainData")) result.MainData = DataContainer.FromJsonObject(data["MainData"].ToObject<JObject>());

        if (result.MainData != null)
        {
            foreach(string collName in result.MainData.GetKeys())
            {
                var coll = result.MainData.GetCollection(collName);
                var dtbStructure = DataAccessUtils.GetTableRawStructure(collName);

                if (dtbStructure != null)
                {
                    foreach (JObject dObject in coll.Entries)
                    {
                        ClassService.SanitizeDataObject(dtbStructure, dObject);
                    }
                }
            }
        }

        return result;
    }

    public static TransportData FromPayloadPacket(PayloadPacket pkt)
    {
        TransportData result = FromJsonObject(JObject.Parse(Utils.GetString(pkt.Payload)));
        result.RequestedBySender = pkt.Sender;
        result.ProcessToken = pkt.ProcessToken;
        return result;
    }

    [NonSerialized]
    public long RequestedBySender = 0L;

    #region "Internal Processing on the Server Only"

    [NonSerialized]
    private IFormFileCollection FileCollection = new FormFileCollection();

    public void SetFileCollection(IFormFileCollection fileCollection)
    {
        FileCollection = fileCollection;
    }

    public IFormFileCollection GetFileCollection()
    {
        return FileCollection;
    }

    [NonSerialized]
    private Dictionary<string, byte[]> FileBytes = new Dictionary<string, byte[]>();

    public void SetFileBytes(string fileName, byte[] bytes)
    {
        FileBytes[fileName] = bytes;
    }

    public byte[] GetFileBytes(string fileName)
    {
        if (!FileBytes.ContainsKey(fileName)) return new byte[] { };
        return FileBytes[fileName];
    }

    public List<string> GetFileNamesStoredAsBytes()
    {
        return FileBytes.Keys.ToList();
    }
    #endregion
}
