using Newtonsoft.Json;
using System.Data;

public class RetainedStreamingMessageInfo
{
    public RetainedStreamingMessageInfo(string payloadType, string topic, string message)
    {
        PayloadType = payloadType;
        Topic = topic;
        Message = message;
    }

    public string PayloadType { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static RetainedStreamingMessageInfo FromDataRow(DataRow dr)
    {
        return new RetainedStreamingMessageInfo(Utils.GetString(dr["PayloadType"]),
            Utils.GetString(dr["Topic"]), Utils.GetString(dr["Message"]));
    }

    public static DecimalPacketWrapper GetDecimalPacketWrapper(RetainedStreamingMessageInfo rmi)
    {
        var pkt = JsonConvert.DeserializeObject<DecimalPacket>(rmi.Message);
        return new DecimalPacketWrapper { Topic = rmi.Topic, DecimalPacket = pkt };
    }

    public static IntegerPacketWrapper GetIntegerPacketWrapper(RetainedStreamingMessageInfo rmi)
    {
        var pkt = JsonConvert.DeserializeObject<IntegerPacket>(rmi.Message);
        return new IntegerPacketWrapper { Topic = rmi.Topic, IntegerPacket = pkt };
    }

    public static LongPacketWrapper GetLongPacketWrapper(RetainedStreamingMessageInfo rmi)
    {
        var pkt = JsonConvert.DeserializeObject<LongPacket>(rmi.Message);
        return new LongPacketWrapper { Topic = rmi.Topic, LongPacket = pkt };
    }

    public static StringPacketWrapper GetStringPacketWrapper(RetainedStreamingMessageInfo rmi)
    {
        var pkt = JsonConvert.DeserializeObject<StringPacket>(rmi.Message);
        return new StringPacketWrapper { Topic = rmi.Topic, StringPacket = pkt };
    }

    public static PayloadPacketWrapper GetPayloadPacketWrapper(RetainedStreamingMessageInfo rmi)
    {
        var pkt = JsonConvert.DeserializeObject<PayloadPacket>(rmi.Message);
        return new PayloadPacketWrapper { Topic = rmi.Topic, PayloadPacket = pkt };
    }
}
