using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

public partial class MessageHubConnection
{
    private void HandleDecimalPacketWrapper(DecimalPacketWrapper wrapper)
    {
        foreach (Action<DecimalPacket> handler in GetDecimalPacketHandlersForTopic(wrapper.Topic))
        {
            try
            {
                handler(wrapper.DecimalPacket);
            }
            catch (Exception ex)
            {
                var ex1 = ex;
                // SWALLOW FOR NOW
            }
        }
    }

    private SynchronizedCollection<Action<DecimalPacket>> GetDecimalPacketHandlersForTopic(string topic)
    {
        bool topicExists = m_decimalPacketHandlers.TryGetValue(topic, out var result);
        if (topicExists) return result;
        return new SynchronizedCollection<Action<DecimalPacket>>();
    }

    private void RegisterDecimalPacketSubscription(string topic, Action<DecimalPacket> handler)
    {
        m_decimalPacketHandlers.AddOrUpdate(topic, 
            t => new SynchronizedCollection<Action<DecimalPacket>> { handler },
            (t, existingHandlers) =>
            {
                if (!existingHandlers.Contains(handler)) existingHandlers.Add(handler);
                return existingHandlers;
            });
    }

    public void SubscribeToTopicForDecimalPacket(string topic, Action<DecimalPacket> handler)
    {
        try
        {
            RegisterDecimalPacketSubscription(topic, handler);
            RegisterInterestInTopicOnHub(topic);
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }
}
