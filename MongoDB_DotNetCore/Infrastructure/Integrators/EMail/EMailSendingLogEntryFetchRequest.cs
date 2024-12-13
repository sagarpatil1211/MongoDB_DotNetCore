using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

public class EMailSendingLogEntryFetchRequest
{
    public static readonly string FetchRequestType = "EMailSendingLogEntryFetchRequest";

    public List<string> TransDates { get; set; } = new List<string>();
    public string FromDate { get; set; } = string.Empty;
    public string ToDate { get; set; } = string.Empty;
    public List<string> EMailIds { get; set; } = new List<string>();

    public string TransDatesString 
    { 
        get 
        { 
            var result = Utils.CombineListIntoSingleString(TransDates, "','");
            if (result.Length > 0) result = "'" + result + "'";
            return result;
        } 
    }

    public string EMailIdsString 
    {
        get
        {
            var result = Utils.CombineListIntoSingleString(EMailIds, "','");
            if (result.Length > 0) result = "'" + result + "'";
            return result;
        }
    }

    public static EMailSendingLogEntryFetchRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<EMailSendingLogEntryFetchRequest>(JsonConvert.SerializeObject(input));
    }
}
