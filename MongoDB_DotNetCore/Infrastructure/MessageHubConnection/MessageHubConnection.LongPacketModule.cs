using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

public partial class MessageHubConnection
{
    private void HandleLongPacketWrapper(LongPacketWrapper wrapper)
    {
        foreach (Action<LongPacket> handler in GetLongPacketHandlersForTopic(wrapper.Topic))
        {
            try
            {
                handler(wrapper.LongPacket);
            }
            catch (Exception ex)
            {
                var ex1 = ex;
                // SWALLOW FOR NOW
            }
        }
    }

    private SynchronizedCollection<Action<LongPacket>> GetLongPacketHandlersForTopic(string topic)
    {
        bool topicExists = m_longPacketHandlers.TryGetValue(topic, out var result);
        if (topicExists) return result;
        return new SynchronizedCollection<Action<LongPacket>>();
    }

    private void RegisterLongPacketSubscription(string topic, Action<LongPacket> handler)
    {
        m_longPacketHandlers.AddOrUpdate(topic, 
            t => new SynchronizedCollection<Action<LongPacket>> { handler },
            (t, existingHandlers) =>
            {
                if (!existingHandlers.Contains(handler)) existingHandlers.Add(handler);
                return existingHandlers;
            });
    }

    public void SubscribeToTopicForLongPacket(string topic, Action<LongPacket> handler)
    {
        try
        {
            RegisterLongPacketSubscription(topic, handler);
            RegisterInterestInTopicOnHub(topic);
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }
}
