using Newtonsoft.Json;

public class IntegerPacket
{
    public IntegerPacket()
    {

    }

    public IntegerPacket(int value)
    {
        V = value;
    }

    public IntegerPacket(string timeStamp, int value)
    {
        T = timeStamp;
        V = value;
    }

    public string T { get; set; } = string.Empty;
    public int V { get; set; } = 0;

    public static IntegerPacket DeserializeFrom(string input)
    {
        return JsonConvert.DeserializeObject<IntegerPacket>(input);
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class IntegerPacketWrapper
{
    public string Topic { get; set; } = string.Empty;
    public IntegerPacket IntegerPacket { get; set; } = new IntegerPacket();
}