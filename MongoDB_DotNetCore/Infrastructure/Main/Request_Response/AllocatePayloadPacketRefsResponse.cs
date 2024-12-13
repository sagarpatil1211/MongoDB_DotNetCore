using Newtonsoft.Json;
using System.Collections.Generic;

public struct AllocatePayloadPacketRefsResponse
{
    public List<long> Refs;

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
