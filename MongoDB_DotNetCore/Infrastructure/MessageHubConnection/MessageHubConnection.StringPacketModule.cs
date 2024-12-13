using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;

public partial class MessageHubConnection
{
    private void HandleStringPacketWrapper(StringPacketWrapper wrapper)
    {
        foreach (Action<StringPacket> handler in GetStringPacketHandlersForTopic(wrapper.Topic))
        {
            try
            {
                handler(wrapper.StringPacket);
            }
            catch (Exception ex)
            {
                var ex1 = ex;
                // SWALLOW FOR NOW
            }
        }
    }

    private SynchronizedCollection<Action<StringPacket>> GetStringPacketHandlersForTopic(string topic)
    {
        bool topicExists = m_stringPacketHandlers.TryGetValue(topic, out var result);
        if (topicExists) return result;
        return new SynchronizedCollection<Action<StringPacket>>();
    }

    private void RegisterStringPacketSubscription(string topic, Action<StringPacket> handler)
    {
        m_stringPacketHandlers.AddOrUpdate(topic, 
            t => new SynchronizedCollection<Action<StringPacket>> { handler },
            (t, existingHandlers) =>
            {
                if (!existingHandlers.Contains(handler)) existingHandlers.Add(handler);
                return existingHandlers;
            });
    }

    public void SubscribeToTopicForStringPacket(string topic, Action<StringPacket> handler)
    {
        try
        {
            RegisterStringPacketSubscription(topic, handler);
            RegisterInterestInTopicOnHub(topic);
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }

    public void SubscribeLocallyToTopicForStringPacket(string topic, Action<StringPacket> handler)
    {
        try
        {
            RegisterStringPacketSubscription(topic, handler);
            //RegisterInterestInTopicOnHub(topic);
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }
}
