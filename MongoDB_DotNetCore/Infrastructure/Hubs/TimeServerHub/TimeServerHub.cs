using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Akka.Actor;

public partial class TimeServerHub : Hub
{
    public const string HubName = "timeserver";

    public const string NotifyTimeTopic = "NotifyTime";

    private const string PayloadType_StringPacket = "StringPacket";

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

    public static void InstantiateActorSystem()
    {
        if (m_actorSystem == null)
        {
            m_actorSystem = ActorSystem.Create("TimeServerActorSystem");
        }
    }

    public static IActorRef tspgActor = null;

    public static void InstantiateHub()
    {
        InstantiateActorSystem();

        tspgActor = ActorSystem.ActorOf<TimeServerPulseGenerator>();

        ActorSystem.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(512), tspgActor, TimeServerPulseGenerator.GeneratePulseCommand, ActorRefs.NoSender);
    }

    private readonly IHubContext<TimeServerHub> m_ctx = null;

    public TimeServerHub(IHubContext<TimeServerHub> ctx)
    {
        m_ctx = ctx;
    }

    public async Task NotifyTime(string strDTNow)
    {
        try
        {
            await m_ctx.Clients.All.SendAsync(PayloadType_StringPacket, 
                new StringPacketWrapper { Topic = "NotifyTime", StringPacket = { T = "Time", V = strDTNow } });
        }
        catch (Exception ex)
        {
            var ex1 = ex;
            // SWALLOW FOR NOW
        }
    }
}
