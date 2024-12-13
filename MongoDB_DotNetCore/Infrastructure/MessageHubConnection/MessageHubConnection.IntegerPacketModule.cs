using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

public partial class MessageHubConnection
{
    private void HandleIntegerPacketWrapper(IntegerPacketWrapper wrapper)
    {
        foreach (Action<IntegerPacket> handler in GetIntegerPacketHandlersForTopic(wrapper.Topic))
        {
            try
            {
                handler(wrapper.IntegerPacket);
            }
            catch (Exception ex)
            {
                var ex1 = ex;
                // SWALLOW FOR NOW
            }
        }
    }

    private SynchronizedCollection<Action<IntegerPacket>> GetIntegerPacketHandlersForTopic(string topic)
    {
        bool topicExists = m_integerPacketHandlers.TryGetValue(topic, out var result);
        if (topicExists) return result;
        return new SynchronizedCollection<Action<IntegerPacket>>();
    }

    private void RegisterIntegerPacketSubscription(string topic, Action<IntegerPacket> handler)
    {
        m_integerPacketHandlers.AddOrUpdate(topic, 
            t => new SynchronizedCollection<Action<IntegerPacket>> { handler },
            (t, existingHandlers) =>
            {
                if (!existingHandlers.Contains(handler)) existingHandlers.Add(handler);
                return existingHandlers;
            });
    }

    public void SubscribeToTopicForIntegerPacket(string topic, Action<IntegerPacket> handler)
    {
        try
        {
            RegisterIntegerPacketSubscription(topic, handler);
            RegisterInterestInTopicOnHub(topic);
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }
}
