using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Akka.Actor;
//using Serilog;
using System.Collections.Concurrent;

public partial class StreamingMessageHub : Hub
{
    public const string HubName = "messagehub";

    private const string PayloadType_DecimalPacket = "DecimalPacket";
    private const string PayloadType_IntegerPacket = "IntegerPacket";
    private const string PayloadType_LongPacket = "LongPacket";
    private const string PayloadType_StringPacket = "StringPacket";
    private const string PayloadType_PayloadPacket = "PayloadPacket";

    private static IActorRef StreamingMessageHubMessageRetainer = null;

    private static readonly ConcurrentDictionary<string, Tuple<string, StringPacket, bool>> m_lastWillStringPacketMessages
        = new ConcurrentDictionary<string, Tuple<string, StringPacket, bool>>();

    private static readonly ConcurrentDictionary<string, Tuple<string, LongPacket, bool>> m_lastWillLongPacketMessages
        = new ConcurrentDictionary<string, Tuple<string, LongPacket, bool>>();

    private static readonly ConcurrentDictionary<string, Tuple<string, IntegerPacket, bool>> m_lastWillIntegerPacketMessages
        = new ConcurrentDictionary<string, Tuple<string, IntegerPacket, bool>>();

    private static readonly ConcurrentDictionary<string, Tuple<string, DecimalPacket, bool>> m_lastWillDecimalPacketMessages
        = new ConcurrentDictionary<string, Tuple<string, DecimalPacket, bool>>();

    private static readonly ConcurrentDictionary<string, Tuple<string, PayloadPacket, bool>> m_lastWillPayloadPacketMessages
        = new ConcurrentDictionary<string, Tuple<string, PayloadPacket, bool>>();

    // In the tuple, 1st Parameter is the topic, 2nd Parameter is the actual payload,
    // 3rd parameter is the retain flag

    private static ActorSystem m_actorSystem = null;

    public static ActorSystem ActorSystem
    {
        get
        {
            return m_actorSystem;
        }
        private set
        {
            m_actorSystem = value;
        }
    }

    private static void InstantiateActorSystem()
    {
        if (m_actorSystem == null)
        {
            m_actorSystem = ActorSystem.Create("StreamingMessageHubActorSystem");
        }
    }

    public static void InstantiateHub()
    {
        InstantiateActorSystem();
        StreamingMessageHubMessageRetainer = ActorSystem.ActorOf<StreamingMessageHubMessageRetainer>();
    }

    static StreamingMessageHub()
    {
        cnnRetainedStreamingMessages = DataAccessUtils.ProvisionAndCreateNewDBConnection("StreamingMessageHub");
        PerformDBMigrationForRetainedStreamingMessagesTable();
        PopulateRetainedStreamingMessagesInMemory();
    }

    private readonly IHubContext<StreamingMessageHub> m_ctx = null;

    public StreamingMessageHub(IHubContext<StreamingMessageHub> ctx)
    {
        m_ctx = ctx;
    }

    public override async Task OnConnectedAsync()
    {
        await Task.Run(() =>
        {
            //Log.Information($"Connected : {Context.ConnectionId} at {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
        });
        
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        await Task.Run(async () =>
        {
            //Log.Information($"Disconnected : {Context.ConnectionId} at {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");

            {
                var lwPkt = GetLastWillStringPacketMessage(Context.ConnectionId);
                if (lwPkt != null) await PostStringPacketToTopic(lwPkt.Item1, lwPkt.Item2, lwPkt.Item3);
            }

            {
                var lwPkt = GetLastWillIntegerPacketMessage(Context.ConnectionId);
                if (lwPkt != null) await PostIntegerPacketToTopic(lwPkt.Item1, lwPkt.Item2, lwPkt.Item3);
            }

            {
                var lwPkt = GetLastWillLongPacketMessage(Context.ConnectionId);
                if (lwPkt != null) await PostLongPacketToTopic(lwPkt.Item1, lwPkt.Item2, lwPkt.Item3);
            }

            {
                var lwPkt = GetLastWillDecimalPacketMessage(Context.ConnectionId);
                if (lwPkt != null) await PostDecimalPacketToTopic(lwPkt.Item1, lwPkt.Item2, lwPkt.Item3);
            }

            {
                var lwPkt = GetLastWillPayloadPacketMessage(Context.ConnectionId);
                if (lwPkt != null)
                {
                    var cdt = DateTime.Now;
                    long pktRef = Convert.ToInt64(cdt.Ticks / 1000);
                    lwPkt.Item2.Ref = pktRef;

                    await PostPayloadPacketToTopic(lwPkt.Item1, lwPkt.Item2, lwPkt.Item3);
                }
            }
        });
    }

    public async Task SubscribeToTopic(List<string> topics)
    {
        foreach (string topic in topics)
        {
            await m_ctx.Groups.AddToGroupAsync(Context.ConnectionId, topic);

            var rmi = GetRetainedStreamingMessage(topic);
            if (rmi != null)
            {
                switch (rmi.PayloadType)
                {
                    case PayloadType_DecimalPacket:
                        var decimalWrapper = RetainedStreamingMessageInfo.GetDecimalPacketWrapper(rmi);
                        await m_ctx.Clients.Client(Context.ConnectionId).SendAsync(rmi.PayloadType, decimalWrapper);
                        break;

                    case PayloadType_IntegerPacket:
                        var integerWrapper = RetainedStreamingMessageInfo.GetIntegerPacketWrapper(rmi);
                        await m_ctx.Clients.Client(Context.ConnectionId).SendAsync(rmi.PayloadType, integerWrapper);
                        break;

                    case PayloadType_LongPacket:
                        var longWrapper = RetainedStreamingMessageInfo.GetLongPacketWrapper(rmi);
                        await m_ctx.Clients.Client(Context.ConnectionId).SendAsync(rmi.PayloadType, longWrapper);
                        break;

                    case PayloadType_StringPacket:
                        var stringWrapper = RetainedStreamingMessageInfo.GetStringPacketWrapper(rmi);
                        await m_ctx.Clients.Client(Context.ConnectionId).SendAsync(rmi.PayloadType, stringWrapper);
                        break;

                    case PayloadType_PayloadPacket:
                        var payloadWrapper = RetainedStreamingMessageInfo.GetPayloadPacketWrapper(rmi);
                        await m_ctx.Clients.Client(Context.ConnectionId).SendAsync(rmi.PayloadType, payloadWrapper);
                        break;
                }
            }
        }
    }

    public async Task UnsubscribeFromTopic(List<string> topics)
    {
        foreach (string topic in topics)
        {
            await m_ctx.Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
        }
    }

    private async Task PostDecimalPacketToTopicInternal(string topic, DecimalPacket payload, bool retain)
    {
        if (retain)
        {
            var rmi = new RetainedStreamingMessageInfo(PayloadType_DecimalPacket, topic, payload.Serialize());

            m_retainedMessages.AddOrUpdate(topic, rmi, (t, msg) => rmi);
            StreamingMessageHubMessageRetainer?.Tell(rmi);
        }

        try
        {
            await m_ctx.Clients.Group(topic).SendAsync(PayloadType_DecimalPacket,
                new DecimalPacketWrapper { Topic = topic, DecimalPacket = payload });
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }

    public async Task PostDecimalPacketToTopic(string topic, DecimalPacket payload, bool retain)
    {
        await PostDecimalPacketToTopicInternal(topic, payload, retain);
    }

    private async Task PostIntegerPacketToTopicInternal(string topic, IntegerPacket payload, bool retain)
    {
        if (retain)
        {
            var rmi = new RetainedStreamingMessageInfo(PayloadType_IntegerPacket, topic, payload.Serialize());

            m_retainedMessages.AddOrUpdate(topic, rmi, (t, msg) => rmi);
            StreamingMessageHubMessageRetainer?.Tell(rmi);
        }

        try
        {
            await m_ctx.Clients.Group(topic).SendAsync(PayloadType_IntegerPacket,
                new IntegerPacketWrapper { Topic = topic, IntegerPacket = payload });
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }

    public async Task PostIntegerPacketToTopic(string topic, IntegerPacket payload, bool retain)
    {
        await PostIntegerPacketToTopicInternal(topic, payload, retain);
    }

    private async Task PostLongPacketToTopicInternal(string topic, LongPacket payload, bool retain)
    {
        if (retain)
        {
            var rmi = new RetainedStreamingMessageInfo(PayloadType_LongPacket, topic, payload.Serialize());

            m_retainedMessages.AddOrUpdate(topic, rmi, (t, msg) => rmi);
            StreamingMessageHubMessageRetainer?.Tell(rmi);
        }

        try
        {
            await m_ctx.Clients.Group(topic).SendAsync(PayloadType_LongPacket,
                new LongPacketWrapper { Topic = topic, LongPacket = payload });
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }

    public async Task PostLongPacketToTopic(string topic, LongPacket payload, bool retain)
    {
        await PostLongPacketToTopicInternal(topic, payload, retain);
    }

    public async Task PostStringPacketToTopicInternal(string topic, StringPacket payload, bool retain)
    {
        if (retain)
        {
            var rmi = new RetainedStreamingMessageInfo(PayloadType_StringPacket, topic, payload.Serialize());

            m_retainedMessages.AddOrUpdate(topic, rmi, (t, msg) => rmi);
            StreamingMessageHubMessageRetainer?.Tell(rmi);
        }

        try
        {
            await m_ctx.Clients.Group(topic).SendAsync(PayloadType_StringPacket,
                new StringPacketWrapper { Topic = topic, StringPacket = payload });
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }

    public async Task PostStringPacketToTopic(string topic, StringPacket payload, bool retain)
    {
        await PostStringPacketToTopicInternal(topic, payload, retain);
    }

    private async Task PostPayloadPacketToTopicInternal(string topic, PayloadPacket payload, bool retain)
    {
        if (retain)
        {
            var rmi = new RetainedStreamingMessageInfo(PayloadType_PayloadPacket, topic, payload.Serialize());

            m_retainedMessages.AddOrUpdate(topic, rmi, (t, msg) => rmi);
            StreamingMessageHubMessageRetainer?.Tell(rmi);
        }

        try
        {
            await m_ctx.Clients.Group(topic).SendAsync(PayloadType_PayloadPacket,
                new PayloadPacketWrapper { Topic = topic, PayloadPacket = payload });
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }

    public async Task PostPayloadPacketToTopic(string topic, PayloadPacket payload, bool retain)
    {
        await PostPayloadPacketToTopicInternal(topic, payload, retain);
    }


    // *********************************************************
    public async Task RegisterLastWillStringPacketMessage(string topic, StringPacket message, bool retain)
    {
        await Task.Run(() =>
        {
            var tNew = Tuple.Create(topic, message, retain);
            m_lastWillStringPacketMessages.AddOrUpdate(Context.ConnectionId, tNew, (s, t) => tNew);
        });
    }

    public async Task RegisterLastWillLongPacketMessage(string topic, LongPacket message, bool retain)
    {
        await Task.Run(() =>
        {
            var tNew = Tuple.Create(topic, message, retain);
            m_lastWillLongPacketMessages.AddOrUpdate(Context.ConnectionId, tNew, (s, t) => tNew);
        });
    }

    public async Task RegisterLastWillIntegerPacketMessage(string topic, IntegerPacket message, bool retain)
    {
        await Task.Run(() =>
        {
            var tNew = Tuple.Create(topic, message, retain);
            m_lastWillIntegerPacketMessages.AddOrUpdate(Context.ConnectionId, tNew, (s, t) => tNew);
        });
    }

    public async Task RegisterLastWillDecimalPacketMessage(string topic, DecimalPacket message, bool retain)
    {
        await Task.Run(() =>
        {
            var tNew = Tuple.Create(topic, message, retain);
            m_lastWillDecimalPacketMessages.AddOrUpdate(Context.ConnectionId, tNew, (s, t) => tNew);
        });
    }

    public async Task RegisterLastWillPayloadPacketMessage(string topic, PayloadPacket message, bool retain)
    {
        await Task.Run(() =>
        {
            var tNew = Tuple.Create(topic, message, retain);
            m_lastWillPayloadPacketMessages.AddOrUpdate(Context.ConnectionId, tNew, (s, t) => tNew);
        });
    }

    // ************************************************

    // ************************************************
    private Tuple<string, StringPacket, bool> GetLastWillStringPacketMessage(string connectionId)
    {
        bool hasValue = m_lastWillStringPacketMessages.TryGetValue(connectionId, out var result);
        if (hasValue) return result;
        return null;
    }

    private Tuple<string, IntegerPacket, bool> GetLastWillIntegerPacketMessage(string connectionId)
    {
        bool hasValue = m_lastWillIntegerPacketMessages.TryGetValue(connectionId, out var result);
        if (hasValue) return result;
        return null;
    }

    private Tuple<string, LongPacket, bool> GetLastWillLongPacketMessage(string connectionId)
    {
        bool hasValue = m_lastWillLongPacketMessages.TryGetValue(connectionId, out var result);
        if (hasValue) return result;
        return null;
    }

    private Tuple<string, DecimalPacket, bool> GetLastWillDecimalPacketMessage(string connectionId)
    {
        bool hasValue = m_lastWillDecimalPacketMessages.TryGetValue(connectionId, out var result);
        if (hasValue) return result;
        return null;
    }

    private Tuple<string, PayloadPacket, bool> GetLastWillPayloadPacketMessage(string connectionId)
    {
        bool hasValue = m_lastWillPayloadPacketMessages.TryGetValue(connectionId, out var result);
        if (hasValue) return result;
        return null;
    }
    // ************************************************
}
