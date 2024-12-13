using Newtonsoft.Json;
using System.Collections.Generic;

public struct AllocateProcessTokensResponse
{
    public List<long> ProcessTokens;

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
