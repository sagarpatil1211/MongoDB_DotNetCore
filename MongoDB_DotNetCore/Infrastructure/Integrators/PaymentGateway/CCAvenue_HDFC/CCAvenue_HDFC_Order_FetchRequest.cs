using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class CCAvenue_HDFC_Order_FetchRequest
{
    public static readonly string FetchRequestType = "CCAvenue_HDFC_OrderFetch_Request";

    public List<long> CCAvenue_HDFC_Order_Refs { get; set; } = new List<long>();
    public string CCAvenue_HDFC_Order_RefsString { get { return Utils.FormulateRefsString(CCAvenue_HDFC_Order_Refs, true); } }

    public static CCAvenue_HDFC_Order_FetchRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<CCAvenue_HDFC_Order_FetchRequest>(JsonConvert.SerializeObject(input));
    }
}
