using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;

//name1space VJCore.Infrastructure.Hubs;

public partial class RequestResponseHub : Hub
{
    private static readonly ConcurrentDictionary<string, Func<PayloadPacket, PayloadPacket>> TopicwiseHandlers
        = new ConcurrentDictionary<string, Func<PayloadPacket, PayloadPacket>>();

    public static void RegisterTopicwiseHandler(string topic, Func<PayloadPacket, PayloadPacket> handler)
    {
        TopicwiseHandlers.AddOrUpdate(topic, handler, (t, a) => handler);
    }

    public static Func<PayloadPacket, PayloadPacket> GetTopicwiseHandler(string topic)
    {
        bool hasHandler = TopicwiseHandlers.TryGetValue(topic, out Func<PayloadPacket, PayloadPacket> result);
        if (hasHandler) return result;
        return null;
    }
}
