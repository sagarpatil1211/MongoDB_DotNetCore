using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

public class CacheDataChangeLevelResponse
{
    public Dictionary<string, long> ChangeLevels { get; set; } = new Dictionary<string, long>();

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
