using Akka.Actor;
using System.Collections.Generic;
//using VJ1Core.Infrastructure.Config;

public class TimeServerMonitor
{
    private static TimeServerMonitor m_instance = null;

    private MessageHubConnection m_hubConnection = null;

    private SynchronizedCollection<IActorRef> m_actorSubscribers = new SynchronizedCollection<IActorRef>();

    private void AddActorSubscriber(IActorRef subscriber)
    {
        m_actorSubscribers.Add(subscriber);
    }

    private void RemoveActorSubscriber(IActorRef subscriber)
    {
        m_actorSubscribers.Remove(subscriber);
    }

    private void Initialize()
    {
        m_hubConnection = new MessageHubConnection(ServerHostingConfig.TimeServerUrl, TimeServerHub.HubName);
        m_hubConnection.Connect();
    }

    private TimeServerMonitor()
    {
        Initialize();

        m_hubConnection.SubscribeLocallyToTopicForStringPacket(TimeServerHub.NotifyTimeTopic,
           pkt =>
           {
               foreach (IActorRef subscriber in m_actorSubscribers)
               {
                   subscriber.Tell(pkt);
               }
           });
    }

    public static void InstantiateTimeServerMonitor()
    {
        m_instance = new TimeServerMonitor();
    }

    public static void SubscribeAsActor(IActorRef subscriber)
    {
        m_instance.AddActorSubscriber(subscriber);
    }

    public static void UnsubscribeAsActor(IActorRef subscriber)
    {
        m_instance.RemoveActorSubscriber(subscriber);
    }
}
