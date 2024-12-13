using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public struct AllocateRefsRequest
{
    public long Count;

    public static AllocateRefsRequest Deserialize(string input)
    {
        var result = JsonConvert.DeserializeObject<AllocateRefsRequest>(input);
        if (result.Count <= 0) result.Count = 1;

        return result;
    }
}
