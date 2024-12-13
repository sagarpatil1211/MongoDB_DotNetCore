using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class DecimalPacket
{
    public DecimalPacket() {}
    public DecimalPacket(decimal value) { V = value; }
    public DecimalPacket(string timeStamp, decimal value) { T = timeStamp; V = value; }

    public string T { get; set; } = string.Empty;
    public decimal V { get; set; } = 0.0M;

    public static DecimalPacket DeserializeFrom(string input)
    {
        return JsonConvert.DeserializeObject<DecimalPacket>(input);
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public JObject ConverttoJObject()
    {
        return JObject.Parse(Serialize());
    }
}


public class DecimalPacketWrapper
{
    public string Topic { get; set; } = string.Empty;
    public DecimalPacket DecimalPacket { get; set; } = new DecimalPacket();
}
