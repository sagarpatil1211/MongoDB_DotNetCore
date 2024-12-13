using Newtonsoft.Json;
using System.Collections.Generic;

public struct CurrentDateTimeResponse
{
    public string DateTimeValue;

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
