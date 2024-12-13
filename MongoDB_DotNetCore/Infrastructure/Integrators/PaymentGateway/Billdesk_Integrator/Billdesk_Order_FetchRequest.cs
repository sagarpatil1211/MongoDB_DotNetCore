using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class Billdesk_Order_FetchRequest
{
    public static readonly string FetchRequestType = "Billdesk_OrderFetch_Request";

    public List<long> Billdesk_Order_Refs { get; set; } = new List<long>();
    public string Billdesk_Order_RefsString { get { return Utils.FormulateRefsString(Billdesk_Order_Refs, true); } }

    public static Billdesk_Order_FetchRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<Billdesk_Order_FetchRequest>(JsonConvert.SerializeObject(input));
    }
}
