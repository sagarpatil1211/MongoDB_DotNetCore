using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

public class TestSMSIntegrationRequest
{
    public static readonly string CustomProcessRequestType = "TestSMSIntegrationRequest";

    public string Sender { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PhoneNumbers { get; set; } = string.Empty;

    public static TestSMSIntegrationRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<TestSMSIntegrationRequest>(JsonConvert.SerializeObject(input));
    }
}
