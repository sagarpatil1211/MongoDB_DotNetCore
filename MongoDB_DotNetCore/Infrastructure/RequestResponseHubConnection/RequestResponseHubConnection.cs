using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Akka.Actor;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

public partial class RequestResponseHubConnection
{
    public const string DefaultHubName = "requestresponse";

    protected readonly HubConnection m_hubConnection = null;

    public event Action Connected;
    public event Action<string> Disconnected;
    public event Action Reconnecting;
    public event Action Reconnected;

    public bool AutoReconnect { get; set; } = true;

    public RequestResponseHubConnection(string url, string hubUri, IActorRef parentActor = null)
    {
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
               .WithAutomaticReconnect(new RequestResponseHubConnectionRetryPolicy())
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
            });

            //if (ex != null)
            //{
            //    foreach (var wrapper in m_lastWillPayloadPacketMessages)
            //    {
            //        HandlePayloadPacketWrapper(wrapper);
            //    }
            //}
        };

        m_hubConnection.Reconnecting += (Exception ex) =>
        {
            if (ex != null)
            {
                File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}RequestResponseHubReconnectingLog.txt",
                    new List<string> { $"{DateTime.Now} : {ex.Message}" });
            }

            Reconnecting?.Invoke();

            return Task.CompletedTask;
        };

        m_hubConnection.Reconnected += (string str) =>
        {
            File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}RequestResponseHubReconnectedLog.txt",
                new List<string> { $"{DateTime.Now} : {str}" });

            Reconnected?.Invoke();

            return Task.CompletedTask;
        };

        ParentActor = parentActor;
    }

    public IActorRef ParentActor { get; private set; } = null;

    public void Connect()
    {
        try
        {
            m_hubConnection.StartAsync().GetAwaiter().GetResult();
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

    public async Task<string> SendStandardRequest(string input)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                string strResult = await m_hubConnection.InvokeAsync<string>("AcceptStandardRequest", input);
                return strResult;
            }
            catch
            {
                // SWALLOW FOR NOW
                return string.Empty;
            }
        }
        else
        {
            return string.Empty;
        }
    }

    public async Task<PayloadPacket> SendStandardRequest(PayloadPacket payload)
    {
        string strResult = await SendStandardRequest(payload.Serialize());
        if (strResult.Trim().Length == 0) return new PayloadPacket();
        return PayloadPacket.DeserializeFrom(strResult);
    }

    public async Task<string> SendLockFreeRequest(string input)
    {
        if (m_hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                string strResult = await m_hubConnection.InvokeAsync<string>("AcceptLockFreeRequest", input);
                return strResult;
            }
            catch
            {
                // SWALLOW FOR NOW
                return string.Empty;
            }
        }
        else
        {
            return string.Empty;
        }
    }

    public async Task<PayloadPacket> SendLockFreeRequest(PayloadPacket payload)
    {
        string strResult = await SendLockFreeRequest(payload.Serialize());
        if (strResult.Trim().Length == 0) return new PayloadPacket();
        return PayloadPacket.DeserializeFrom(strResult);
    }

    // ********* LAST WILL **********
    //private SynchronizedCollection<PayloadPacketWrapper> m_lastWillPayloadPacketMessages = new SynchronizedCollection<PayloadPacketWrapper>();

    public void RegisterLastWillPayloadPacketMessage(string topic, PayloadPacket message, bool retain)
    {
        var wrapper = new PayloadPacketWrapper { Topic = topic, PayloadPacket = message };
        //m_lastWillPayloadPacketMessages.Add(wrapper);

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

    //private readonly ConcurrentDictionary<string, SynchronizedCollection<Action<PayloadPacket>>> m_payloadPacketHandlers = new ConcurrentDictionary<string, SynchronizedCollection<Action<PayloadPacket>>>();

    //private void HandlePayloadPacketWrapper(PayloadPacketWrapper wrapper)
    //{
    //    foreach (Action<PayloadPacket> handler in GetPayloadPacketHandlersForTopic(wrapper.Topic))
    //    {
    //        try
    //        {
    //            handler(wrapper.PayloadPacket);
    //        }
    //        catch (Exception ex)
    //        {
    //            var ex1 = ex;
    //            // SWALLOW FOR NOW
    //        }
    //    }
    //}

    //private SynchronizedCollection<Action<PayloadPacket>> GetPayloadPacketHandlersForTopic(string topic)
    //{
    //    bool topicExists = m_payloadPacketHandlers.TryGetValue(topic, out var result);
    //    if (topicExists) return result;
    //    return new SynchronizedCollection<Action<PayloadPacket>>();
    //}
}
