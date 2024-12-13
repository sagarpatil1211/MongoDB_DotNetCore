using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public struct AllocatePayloadPacketRefsRequest
{
    public long Count;

    public static AllocatePayloadPacketRefsRequest Deserialize(string input)
    {
        var result = JsonConvert.DeserializeObject<AllocatePayloadPacketRefsRequest>(input);
        if (result.Count <= 0) result.Count = 1;

        return result;
    }
}
