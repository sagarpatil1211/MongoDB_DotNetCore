using Newtonsoft.Json;
using System;

public partial class PayloadPacket
{
    public PayloadPacket()
    {

    }

    public long Ref { get; set; } = 0L;
    public int PartNo { get; set; } = 0;
    public int TotalPartCount { get; set; } = 0;
    public long Sender { get; set; } = 0;
    public string Topic { get; set; } = string.Empty;
    public string PayloadDescriptor { get; set; } = string.Empty;
    public object Payload { get; set; } = null;
    public string TargetMethod { get; set; } = string.Empty;
    public string ProcessToken { get; set; } = string.Empty;

    public static PayloadPacket DeserializeFrom(string input)
    {
        string normalizedInput = JsonConvert.DeserializeObject(input).ToString();
        return JsonConvert.DeserializeObject<PayloadPacket>(normalizedInput);
        //return JsonConvert.DeserializeObject<PayloadPacket>(input);
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    private static long CurrentRef = 0L;

    public static PayloadPacket CreateNewInstance()
    {
        if (CurrentRef == 0)
        {
            var cdt = DateTime.Now;
            CurrentRef = Convert.ToInt64(cdt.Ticks / 1000);
        }

        CurrentRef++;

        var result = new PayloadPacket
        {
            Ref = CurrentRef
        };

        return result;
    }
}


[Serializable]
public class PayloadPacketWrapper
{
    public string Topic { get; set; } = string.Empty;
    public PayloadPacket PayloadPacket { get; set; } = new PayloadPacket();
}