using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public struct Msg91_SMS_Response_v2
{
    public string message;
    public string type;

    public static Msg91_SMS_Response_v2 Deserialize(string input)
    {
        return JsonConvert.DeserializeObject<Msg91_SMS_Response_v2>(input);
    }
}
