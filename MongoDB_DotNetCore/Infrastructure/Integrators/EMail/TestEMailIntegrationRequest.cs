using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
//using VJ1Core.Infrastructure.Main;

public class TestEMailIntegrationRequest
{
    public static readonly string CustomProcessRequestType = "TestEMailIntegrationRequest";

    public string SenderFriendlyName { get; set; } = string.Empty;
    public string EMailID { get; set; } = string.Empty;
    public string SMTPHost { get; set; } = string.Empty;
    public int SMTPPort { get; set; } = 0;
    public bool SSLRequired { get; set; } = false;
    public EMailSSLConnectionOptions SSLConnectionOption { get; set; } = EMailSSLConnectionOptions.None;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SendToEMailId { get; set; } = string.Empty;

    public static TestEMailIntegrationRequest FromJsonObject(JObject input)
    {
        return JsonConvert.DeserializeObject<TestEMailIntegrationRequest>(JsonConvert.SerializeObject(input));
    }
}
