using Newtonsoft.Json.Linq;

public class Billdesk_PostActionObject
{
    public static string Object_Type = "Billdesk_PostActionObject";

    public class Options
    {
        public bool enableChildWindowPosting { get; set; } = false;
        public bool enablePaymentRetry { get; set; } = false;
        public int retry_attempt_count { get; set; } = 0;
        //public string txtPayCategory { get; set; } = string.Empty;
    }

    public string msg { get; set; } = string.Empty;
    public Options options { get; set; } = new Options();
    public string callbackUrl { get; set; }= string.Empty;

    public void MergeIntoTransportData(TransportData td)
    {
        var coll = td.MainData.GetOrCreateCollection(Object_Type);
        coll.Entries.Add(JObject.FromObject(this));
    }
}