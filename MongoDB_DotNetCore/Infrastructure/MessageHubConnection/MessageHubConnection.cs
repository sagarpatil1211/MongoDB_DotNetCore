using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Akka.Actor;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

public partial class MessageHubConnection
{
    protected readonly HubConnection m_hubConnection = null;

    private readonly ConcurrentDictionary<string, SynchronizedCollection<Action<DecimalPacket>>> m_decimalPacketHandlers = new();
    private readonly ConcurrentDictionary<string, SynchronizedCollection<Action<LongPacket>>> m_longPacketHandlers = new();
    private readonly ConcurrentDictionary<string, SynchronizedCollection<Action<IntegerPacket>>> m_integerPacketHandlers = new();
    private readonly ConcurrentDictionary<string, SynchronizedCollection<Action<StringPacket>>> m_stringPacketHandlers = new();
    private readonly ConcurrentDictionary<string, SynchronizedCollection<Action<PayloadPacket>>> m_payloadPacketHandlers = new();

    public event Action Connected;
    public event Action<string> Disconnected;
    public event Action Reconnecting;
    public event Action Reconnected;

    public bool AutoReconnect { get; set; } = true;

    public MessageHubConnection(string url, string hubUri, IActorRef parentActor = null)
    {
        //.AddMessagePackProtocol()

        m_hubConnection = new HubConnectionBuilder()
               .WithUrl($"{url}/{hubUri}", (options) =>
               {
                   options.WebSocketConfiguration = conf =>
                   {
                       conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
                   };
                   options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                   };
               })
               .AddNewtonsoftJsonProtocol()
               .ConfigureLogging(logging => logging.AddConsole())
               .WithAutomaticReconnect(new MessageHubConnectionRetryPolicy())
               .Build();

        m_hubConnection.ServerTimeout = TimeSpan.FromDays(1);

        m_hubConnection.Closed += async (Exception ex) =>
        {
            await Task.Run(() =>
            {
                if (ex != null)
                {
                    Disconnected?.Invoke(ex.Message);
                }
                else
                {
                    Disconnected?.Invoke("Disconnected");
                }

                if (ex != null)
                {
                    foreach (var wrapper in m_lastWillStringPacketMessages)
                    {
                        HandleStringPacketWrapper(wrapper);
                    }

                    foreach (var wrapper in m_lastWillIntegerPacketMessages)
                    {
                        HandleIntegerPacketWrapper(wrapper);
                    }

                    foreach (var wrapper in m_lastWillLongPacketMessages)
                    {
                        HandleLongPacketWrapper(wrapper);
                    }

                    foreach (var wrapper in m_lastWillDecimalPacketMessages)
                    {
                        HandleDecimalPacketWrapper(wrapper);
                    }

                    foreach (var wrapper in m_lastWillPayloadPacketMessages)
                    {
                        HandlePayloadPacketWrapper(wrapper);
                    }

                    //await Task.Delay(TimeSpan.FromSeconds(5));
                    //Thread.Sleep(TimeSpan.FromSeconds(1));
                    //Connect();
                }
            });
            

            //return Task.CompletedTask;
        };

        m_hubConnection.Reconnecting += (Exception ex) =>
        {
            if (ex != null)
            {
                File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}MessageHubReconnectingLog.txt",
                    new List<string> { $"{DateTime.Now} : {ex.Message}" });
            }

            RegisterWrapperHandlers();

            Reconnecting?.Invoke();

            return Task.CompletedTask;
        };

        m_hubConnection.Reconnected += (string str) =>
        {
            File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}MessageHubReconnectedLog.txt",
                new List<string> { $"{DateTime.Now} : {str}" });

            RegisterInterestInAllTopicsOnHub();

            Reconnected?.Invoke();

            return Task.CompletedTask;
        };

        ParentActor = parentActor;
    }

    public IActorRef ParentActor { get; private set; } = null;

    private IDisposable m_decimalPacketSubscription = null;
    private IDisposable m_integerPacketSubscription = null;
    private IDisposable m_longPacketSubscription = null;
    private IDisposable m_stringPacketSubscription = null;
    private IDisposable m_payloadPacketSubscription = null;

    private void RegisterWrapperHandlers()
    {
        m_decimalPacketSubscription?.Dispose();
        m_integerPacketSubscription?.Dispose();
        m_longPacketSubscription?.Dispose();
        m_stringPacketSubscription?.Dispose();
        m_payloadPacketSubscription?.Dispose();

        m_decimalPacketSubscription = m_hubConnection.On<DecimalPacketWrapper>("DecimalPacket", HandleDecimalPacketWrapper);
        m_integerPacketSubscription = m_hubConnection.On<IntegerPacketWrapper>("IntegerPacket", HandleIntegerPacketWrapper);
        m_longPacketSubscription = m_hubConnection.On<LongPacketWrapper>("LongPacket", HandleLongPacketWrapper);
        m_stringPacketSubscription = m_hubConnection.On<StringPacketWrapper>("StringPacket", HandleStringPacketWrapper);
        
        m_payloadPacketSubscription = m_hubConnection.On<PayloadPacketWrapper>("PayloadPacket",
            pkt =>
            {
                HandlePayloadPacketWrapper(pkt);
            });
    }

    private void RegisterInterestInAllTopicsOnHub()
    {
        foreach (string topic in GetListOfAllTopicsSubscribed())
        {
            RegisterInterestInTopicOnHub(topic);
        }
    }

    private SynchronizedCollection<string> GetListOfAllTopicsSubscribed()
    {
        SynchronizedCollection<string> result = new();

        var decimalPacketTopics = m_decimalPacketHandlers.Keys;
        var integerPacketTopics = m_integerPacketHandlers.Keys;
        var longPacketTopics = m_longPacketHandlers.Keys;
        var stringPacketTopics = m_stringPacketHandlers.Keys;
        var payloadPacketTopics = m_payloadPacketHandlers.Keys;

        foreach (var topic in decimalPacketTopics) result.Add(topic);
        foreach (var topic in integerPacketTopics) result.Add(topic);
        foreach (var topic in longPacketTopics) result.Add(topic);
        foreach (var topic in stringPacketTopics) result.Add(topic);
        foreach (var topic in payloadPacketTopics) result.Add(topic);

        return result;
    }

    private void RegisterInterestInTopicOnHub(string topic)
    {
        RegisterInterestInTopicOnHub(new List<string> { topic });
    }

    private void RegisterInterestInTopicOnHub(List<string> topics)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("SubscribeToTopic", topics).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    private void UnregisterInterestInTopicOnHub(string topic)
    {
        UnregisterInterestInTopicOnHub(new List<string> { topic });
    }

    private void UnregisterInterestInTopicOnHub(List<string> topics)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("UnsubscribeFromTopic", topics).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void Connect()
    {
        try
        {
            RegisterWrapperHandlers();
            m_hubConnection.StartAsync().GetAwaiter().GetResult();
            RegisterInterestInAllTopicsOnHub();

            Connected?.Invoke();
        }
        catch (Exception ex)
        {
            Disconnected?.Invoke(ex.Message);

            if (AutoReconnect)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    Connect();
                });
            }
        }
    }

    public void Disconnect()
    {
        m_hubConnection.StopAsync().GetAwaiter().GetResult();
    }

    public void UnregisterAllPacketHandlersForTopic(string topic)
    {
        UnregisterInterestInTopicOnHub(topic);

        if (m_decimalPacketHandlers.ContainsKey(topic)) _ = m_decimalPacketHandlers.TryRemove(topic, out _);
        if (m_integerPacketHandlers.ContainsKey(topic)) _ = m_integerPacketHandlers.TryRemove(topic, out _);
        if (m_longPacketHandlers.ContainsKey(topic)) _ = m_longPacketHandlers.TryRemove(topic, out _);
        if (m_stringPacketHandlers.ContainsKey(topic)) _ = m_stringPacketHandlers.TryRemove(topic, out _);
        if (m_payloadPacketHandlers.ContainsKey(topic)) _ = m_payloadPacketHandlers.TryRemove(topic, out _);
    }


    // ********* PUBLISH SECTION **********
    public void PublishPayloadPacket(string topic, PayloadPacket payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("PostPayloadPacketToTopic", topic, payload, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void PublishString(string topic, string payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            PublishStringPacket(topic, new StringPacket(payload), retain);
        }
    }

    public void PublishStringPacket(string topic, StringPacket payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("PostStringPacketToTopic", topic, payload, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void PublishLong(string topic, long payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            PublishLongPacket(topic, new LongPacket(payload), retain);
        }
    }

    public void PublishLongPacket(string topic, LongPacket payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("PostLongPacketToTopic", topic, payload, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void PublishInteger(string topic, int payload, bool retain = false)
    {
        PublishIntegerPacket(topic, new IntegerPacket(payload), retain);
    }

    public void PublishInteger(string topic, string timeStamp, int payload, bool retain = false)
    {
        PublishIntegerPacket(topic, new IntegerPacket(timeStamp, payload), retain);
    }

    public void PublishIntegerPacket(string topic, IntegerPacket payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("PostIntegerPacketToTopic", topic, payload, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void PublishDecimal(string topic, decimal payload, bool retain = false)
    {
        PublishDecimalPacket(topic, new DecimalPacket(payload), retain);
    }

    public void PublishDecimalPacket(string topic, DecimalPacket payload, bool retain = false)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("PostDecimalPacketToTopic", topic, payload, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void CallMethodOnHub(string methodName)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync(methodName).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void CallMethodOnHub(string methodName, object arg1)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync(methodName, arg1).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }
    // ********* END PUBLISH SECTION **********

    // ********* LAST WILL **********
    private readonly SynchronizedCollection<StringPacketWrapper> m_lastWillStringPacketMessages = new();
    private readonly SynchronizedCollection<IntegerPacketWrapper> m_lastWillIntegerPacketMessages = new();
    private readonly SynchronizedCollection<LongPacketWrapper> m_lastWillLongPacketMessages = new();
    private readonly SynchronizedCollection<DecimalPacketWrapper> m_lastWillDecimalPacketMessages = new();
    private readonly SynchronizedCollection<PayloadPacketWrapper> m_lastWillPayloadPacketMessages = new();

    public void RegisterLastWillStringPacketMessage(string topic, StringPacket message, bool retain)
    {
        var wrapper = new StringPacketWrapper { Topic = topic, StringPacket = message };
        m_lastWillStringPacketMessages.Add(wrapper);

        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("RegisterLastWillStringPacketMessage", topic, message, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void RegisterLastWillLongPacketMessage(string topic, LongPacket message, bool retain)
    {
        var wrapper = new LongPacketWrapper { Topic = topic, LongPacket = message };
        m_lastWillLongPacketMessages.Add(wrapper);

        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("RegisterLastWillLongPacketMessage", topic, message, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void RegisterLastWillIntegerPacketMessage(string topic, IntegerPacket message, bool retain)
    {
        var wrapper = new IntegerPacketWrapper { Topic = topic, IntegerPacket = message };
        m_lastWillIntegerPacketMessages.Add(wrapper);

        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("RegisterLastWillIntegerPacketMessage", topic, message, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void RegisterLastWillDecimalPacketMessage(string topic, DecimalPacket message, bool retain)
    {
        var wrapper = new DecimalPacketWrapper { Topic = topic, DecimalPacket = message };
        m_lastWillDecimalPacketMessages.Add(wrapper);

        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("RegisterLastWillDecimalPacketMessage", topic, message, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }

    public void RegisterLastWillPayloadPacketMessage(string topic, PayloadPacket message, bool retain)
    {
        var wrapper = new PayloadPacketWrapper { Topic = topic, PayloadPacket = message };
        m_lastWillPayloadPacketMessages.Add(wrapper);

        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                m_hubConnection.SendAsync("RegisterLastWillPayloadPacketMessage", topic, message, retain).GetAwaiter().GetResult();
            }
            catch
            {
                // SWALLOW FOR NOW
            }
        }
    }
    // ******* END LAST WILL ********
}
