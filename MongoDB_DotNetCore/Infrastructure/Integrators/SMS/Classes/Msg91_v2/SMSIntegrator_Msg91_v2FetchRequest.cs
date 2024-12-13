using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

public class SMSIntegrator_Msg91_v2FetchRequest
{
    public static readonly string FetchRequestType = "SMSIntegrator_Msg91_v2FetchRequest";

    public List<long> SMSIntegrator_Msg91_v2Refs { get; set; } = new List<long>();
    public string SMSIntegrator_Msg91_v2RefsString { get { return Utils.FormulateRefsString(SMSIntegrator_Msg91_v2Refs, true); } }

    public static SMSIntegrator_Msg91_v2FetchRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<SMSIntegrator_Msg91_v2FetchRequest>(JsonConvert.SerializeObject(input));
    }
}
