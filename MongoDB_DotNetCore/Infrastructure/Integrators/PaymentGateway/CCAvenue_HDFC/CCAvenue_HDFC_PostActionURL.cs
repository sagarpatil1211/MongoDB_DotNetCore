using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CCAvenue_HDFC_PostActionURL
{
    public static string Object_Type = "CCAvenue_HDFC_PostActionURL";

    public string URLString = string.Empty;
    public string GeneratedOrderId = string.Empty;

    public void MergeIntoTransportData(TransportData td)
    {
        var coll = td.MainData.GetOrCreateCollection(Object_Type);
        coll.Entries.Add(JObject.FromObject(this));
    }
}