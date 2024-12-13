using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public struct CurrentDateTimeRequest
{
    public long SystemUserLoginToken;

    public static CurrentDateTimeRequest Deserialize(string input)
    {
        var result = JsonConvert.DeserializeObject<CurrentDateTimeRequest>(input);
        return result;
    }
}
