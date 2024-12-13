using Newtonsoft.Json;

public class StringPacket
{
    public StringPacket()
    {

    }

    public StringPacket(string value)
    {
        V = value;
    }

    public StringPacket(string timeStamp, string value)
    {
        T = timeStamp;
        V = value;
    }

    public string T { get; set; } = string.Empty;
    public string V { get; set; } = string.Empty;

    public static StringPacket DeserializeFrom(string input)
    {
        return JsonConvert.DeserializeObject<StringPacket>(input);
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class StringPacketWrapper
{
    public string Topic { get; set; } = string.Empty;
    public StringPacket StringPacket { get; set; } = new StringPacket();
}
