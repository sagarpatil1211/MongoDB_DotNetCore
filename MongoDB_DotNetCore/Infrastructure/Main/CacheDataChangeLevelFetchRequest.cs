using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public class CacheDataChangeLevelFetchRequest
{
    public List<string> MasterTableNames { get; set; } = new List<string>();

    public string FormulateTableNamesString()
    {
        var sb = new StringBuilder();

        foreach(string tName in MasterTableNames)
        {
            if (sb.Length > 0) sb.Append(",");
            sb.Append($"'{tName}'");
        }
        
        return sb.ToString();
    }

    public static CacheDataChangeLevelFetchRequest Deserialize(string input)
    {
        var result = JsonConvert.DeserializeObject<CacheDataChangeLevelFetchRequest>(input);
        return result;
    }
}
