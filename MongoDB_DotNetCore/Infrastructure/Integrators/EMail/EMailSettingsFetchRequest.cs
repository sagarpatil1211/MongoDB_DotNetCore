using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

public class EMailSettingsFetchRequest
{
    public static readonly string FetchRequestType = "EMailSettingsFetchRequest";

    public List<long> EMailSettingsRefs { get; set; } = new List<long>();
    public string EMailSettingsRefsString { get { return Utils.FormulateRefsString(EMailSettingsRefs, true); } }

    public static EMailSettingsFetchRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<EMailSettingsFetchRequest>(JsonConvert.SerializeObject(input));
    }
}
