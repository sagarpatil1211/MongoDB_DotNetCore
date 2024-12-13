using Newtonsoft.Json;

public class LongPacket
{
    public LongPacket()
    {

    }

    public LongPacket(long value)
    {
        V = value;
    }

    public LongPacket(string timeStamp, long value)
    {
        T = timeStamp;
        V = value;
    }

    public string T { get; set; } = string.Empty;
    public long V { get; set; } = 0L;

    public static LongPacket DeserializeFrom(string input)
    {
        return JsonConvert.DeserializeObject<LongPacket>(input);
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}


public class LongPacketWrapper
{
    public string Topic { get; set; } = string.Empty;
    public LongPacket LongPacket { get; set; } = new LongPacket();
}