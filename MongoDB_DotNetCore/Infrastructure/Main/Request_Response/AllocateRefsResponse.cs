using Newtonsoft.Json;
using System.Collections.Generic;

public struct AllocateRefsResponse
{
    public List<long> Refs;

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
