using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MessageHubConnection
{
    private void HandlePayloadPacketWrapper(PayloadPacketWrapper wrapper)
    {
        foreach (Action<PayloadPacket> handler in GetPayloadPacketHandlersForTopic(wrapper.Topic))
        {
            try
            {
                handler(wrapper.PayloadPacket);
            }
            catch (Exception ex)
            {
                var ex1 = ex;
                // SWALLOW FOR NOW
            }
        }
    }

    private SynchronizedCollection<Action<PayloadPacket>> GetPayloadPacketHandlersForTopic(string topic)
    {
        bool topicExists = m_payloadPacketHandlers.TryGetValue(topic, out var result);
        if (topicExists) return result;
        return new SynchronizedCollection<Action<PayloadPacket>>();
    }

    private void RegisterPayloadPacketSubscription(string topic, Action<PayloadPacket> handler)
    {
        m_payloadPacketHandlers.AddOrUpdate(topic, 
            t => new SynchronizedCollection<Action<PayloadPacket>> { handler },
            (t, existingHandlers) =>
            {
                if (!existingHandlers.Contains(handler)) existingHandlers.Add(handler);
                return existingHandlers;
            });
    }

    public void SubscribeToTopicForPayloadPacket(string topic, Action<PayloadPacket> handler)
    {
        try
        {
            RegisterPayloadPacketSubscription(topic, handler);
            RegisterInterestInTopicOnHub(topic);
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }
}
