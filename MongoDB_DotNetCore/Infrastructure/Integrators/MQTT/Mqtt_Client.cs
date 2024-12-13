//using Akka.Actor;
//using MQTTnet;
//using MQTTnet.Client.Options;
//using MQTTnet.Extensions.ManagedClient;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//public class Mqtt_Client : ReceiveActor
//{
//    public const string StartClientInstanceRequest = "StartClientInstance";
//    public const string StopClientInstanceRequest = "StopClientInstance";

//    private IManagedMqttClient m_clientInstance = null;

//    private readonly string m_clientId = string.Empty;

//    private MqttConnectionConfig m_conf = null;

//    public Mqtt_Client(string clientId)
//    {
//        m_clientId = clientId;

//        Receive<string>(async input =>
//        {
//            switch (input)
//            {
//                case StartClientInstanceRequest:
//                    string startResult = await StartClientInstance();
//                    Sender.Tell(startResult);
//                    break;

//                case StopClientInstanceRequest:
//                    await StopClientInstance();
//                    break;
//            }
//        });

//        Receive<PublishPayloadPacketRequest>(async req => await PublishPayloadPacket(req.Pkt, req.Retain));
//        Receive<PublishPayloadPacketWithTopicRequest>(async req => await PublishPayloadPacket(req.Topic, req.Pkt, req.Retain));
//        Receive<PublishMQTTMessageRequest>(async req => await Publish(req.Topic, req.Payload, req.Retain));
//    }

//    public static IActorRef CreateAndStartInstance(string clientId, string actorName)
//    {
//        var props = Props.Create<Mqtt_Client>(clientId);
        
//        var result = SessionController.ActorSystem.ActorOf(props, actorName);
//        result.Tell(StartClientInstanceRequest);

//        return result;
//    }

//    public static IActorRef CreateAndStartInstance(string clientId, IActorContext parent, string actorName)
//    {
//        var props = Props.Create<Mqtt_Client>(clientId);

//        var result = parent.ActorOf(props, actorName);
//        result.Tell(StartClientInstanceRequest);

//        return result;
//    }

//    protected override void PreStart()
//    {
//        m_clientInstance = new MqttFactory().CreateManagedMqttClient();

//        m_clientInstance.UseDisconnectedHandler(args =>
//        {
//            if (args.Exception != null)
//            {
//                Console.WriteLine($"{m_clientId} : ERROR : " + args.Exception.Message);
//                System.Diagnostics.Debug.WriteLine($"{m_clientId} : ERROR : " + args.Exception.Message);
//            }
//        });
//    }

//    protected override void PostStop()
//    {
//        Task.WaitAll(StopClientInstance());
//    }

//    private async Task<string> StartClientInstance()
//    {
//        string result = string.Empty;

//        try
//        {
//            m_conf = MqttConnectionConfig.ReadFromFileOrCreateNew();
//            if (m_clientId.Trim().Length > 0) m_conf.ClientId = m_clientId;

//            MqttClientOptionsBuilder clientOptionsBuilder
//                = new MqttClientOptionsBuilder().WithClientId(m_conf.ClientId)
//                    .WithWebSocketServer(optionsBuilder =>
//                    {
//                        optionsBuilder.Uri = m_conf.Uri;
//                    })
//                    .WithCredentials(m_conf.UserId, m_conf.Password);

//            if (m_conf.WithTls) clientOptionsBuilder = clientOptionsBuilder.WithTls();

//            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
//                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
//                    .WithClientOptions(clientOptionsBuilder.Build())
//                    .Build();

//            await m_clientInstance?.StartAsync(options);
//        }
//        catch (Exception ex)
//        {
//            result = ex.Message;
//        }

//        return result;
//    }

//    private async Task StopClientInstance()
//    {
//        await m_clientInstance?.StopAsync();
//    }

//    public async Task PublishPayloadPacket(PayloadPacket pkt, bool retain)
//    {
//        await PublishPayloadPacket(pkt.Topic, pkt, retain);
//    }

//    public async Task PublishPayloadPacket(string topic, PayloadPacket pkt, bool retain)
//    {
//        string payload = pkt.Serialize();
//        await Publish(topic, payload, retain);
//    }

//    public async Task Publish(string topic, string payload, bool retain)
//    {
//        var message = new MqttApplicationMessageBuilder()
//            .WithTopic(topic)
//            .WithPayload(payload)
//            .WithAtMostOnceQoS()
//            .Build();

//        message.Retain = retain;

//        await m_clientInstance.PublishAsync(message, CancellationToken.None);
//    }

//    private static IActorRef CommonInstance { get; set; } = null;

//    public static void InstantiateAndStartCommonInstance()
//    {
//        if (CommonInstance != null) return;
//        CommonInstance = CreateAndStartInstance(string.Empty, "Mqtt_Client_CommonInstance");
//    }

//    public static void PublishPayloadPacketThroughCommonInstance(PayloadPacket pkt, bool retain)
//    {
//        CommonInstance?.Tell(new PublishPayloadPacketRequest(pkt, retain));
//    }

//    public static void PublishPayloadPacketThroughCommonInstance(string topic, PayloadPacket pkt, bool retain)
//    {
//        CommonInstance?.Tell(new PublishPayloadPacketWithTopicRequest(topic, pkt, retain));
//    }

//    public static void PublishThroughCommonInstance(string topic, string payload, bool retain)
//    {
//        CommonInstance?.Tell(new PublishMQTTMessageRequest(topic, payload, retain));
//    }

//    public static void StopCommonInstance()
//    {
//        CommonInstance?.Tell(StopClientInstanceRequest);
//    }
//}
