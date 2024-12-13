using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public struct Msg91_SMS_Request_v2
{
    public struct Msg91_MessageAndTargetNumber
    {
        public string message;
        public List<string> to;
    }

    public string sender;
    public string route;
    public string country;
    public List<Msg91_MessageAndTargetNumber> sms;

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
