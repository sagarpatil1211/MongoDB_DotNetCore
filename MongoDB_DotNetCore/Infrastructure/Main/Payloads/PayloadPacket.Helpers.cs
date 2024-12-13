using Newtonsoft.Json;
using System;

public partial class PayloadPacket
{
    public static PayloadPacket FromTransactionResult(TransactionResult tr, PayloadPacket incomingPkt = null)
    {
        PayloadPacket result = CreateNewInstance();

        result.PartNo = 1;
        result.TotalPartCount = 1;
        result.Sender = 0;
        result.TargetMethod = string.Empty;
        result.Topic = Topics.ServerToClientResponse;
        result.PayloadDescriptor = "TransactionResult";
        result.Payload = tr.Serialize();
        result.ProcessToken = tr.ProcessToken;

        return result;
    }

    public static PayloadPacket FromTransportData(TransportData td, string topic)
    {
        PayloadPacket result = CreateNewInstance();

        result.PartNo = 1;
        result.TotalPartCount = 1;
        result.Sender = 0;
        result.TargetMethod = string.Empty;
        result.Topic = topic;
        result.PayloadDescriptor = "TransportData";
        result.Payload = td.ConvertToJsonObject();

        return result;
    }
}