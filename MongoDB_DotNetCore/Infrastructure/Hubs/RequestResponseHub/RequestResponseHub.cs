using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

//name1space VJCore.Infrastructure.Hubs;

public partial class RequestResponseHub : Hub
{
    public const string HubName = "requestresponse";

    private static readonly ConcurrentDictionary<string, Tuple<string, PayloadPacket, bool>> m_lastWillPayloadPacketMessages
        = new ConcurrentDictionary<string, Tuple<string, PayloadPacket, bool>>();

    // In the tuple, 1st Parameter is the topic, 2nd Parameter is the actual payload,
    // 3rd parameter is the retain flag

    public static void InstantiateHub()
    {
    }

    private readonly IHubContext<RequestResponseHub> m_ctx = null;

    public RequestResponseHub(IHubContext<RequestResponseHub> ctx)
    {
        m_ctx = ctx;
    }

    public async Task<string> AcceptStandardRequest(string input)
    {
        return await Task.Run(async () =>
        {
            if (input == "TEST")
            {
                await Task.Delay(2000);
                return $"TEST SUCCESSFUL AT {DTU.ConvertToString(DateTime.Now)}";
            }

            if (!SessionController.IsInitialized())
            {
                TransactionResult tr = new TransactionResult
                {
                    Successful = false,
                    Message = "System in pre-initialization stage"
                };

                PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr);
                return pktResult.Serialize();
            }

            SessionController.ObtainProcessLock();

            string result = string.Empty;

            try
            {
                var incomingPkt = JsonConvert.DeserializeObject<PayloadPacket>(input);

                Func<PayloadPacket, PayloadPacket> topicwiseHandler = GetTopicwiseHandler(incomingPkt.Topic);
                if (topicwiseHandler != null)
                {
                    PayloadPacket pktResult = topicwiseHandler(incomingPkt);
                    result = pktResult.Serialize();
                }
                else
                {
                    PayloadPacket pktResult = SessionController.DIS.AcceptRequest(incomingPkt, null);
                    result = pktResult.Serialize();
                }
            }
            catch (Exception ex)
            {
                TransactionResult tr = new TransactionResult
                {
                    Successful = false,
                    Message = ex.Message
                };

                PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr);
                result = pktResult.Serialize();
            }
            finally
            {
                SessionController.ReleaseProcessLock();
            }

            return result;
        });
    }

    public async Task<string> AcceptLockFreeRequest(string input)
    {
        return await Task.Run(async () =>
        {
            if (input == "TEST")
            {
                await Task.Delay(2000);
                return $"TEST SUCCESSFUL AT {DTU.ConvertToString(DateTime.Now)}";
            }

            if (!SessionController.IsInitialized())
            {
                TransactionResult tr = new TransactionResult
                {
                    Successful = false,
                    Message = "System in pre-initialization stage"
                };

                PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr);
                return pktResult.Serialize();
            }

            string result = string.Empty;

            try
            {
                var incomingPkt = JsonConvert.DeserializeObject<PayloadPacket>(input);

                Func<PayloadPacket, PayloadPacket> topicwiseHandler = GetTopicwiseHandler(incomingPkt.Topic);
                if (topicwiseHandler != null)
                {
                    PayloadPacket pktResult = topicwiseHandler(incomingPkt);
                    result = pktResult.Serialize();
                }
            }
            catch (Exception ex)
            {
                TransactionResult tr = new TransactionResult
                {
                    Successful = false,
                    Message = ex.Message
                };

                PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr);
                result = pktResult.Serialize();
            }

            return result;
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

    private Tuple<string, PayloadPacket, bool> GetLastWillPayloadPacketMessage(string connectionId)
    {
        bool hasValue = m_lastWillPayloadPacketMessages.TryGetValue(connectionId, out var result);
        if (hasValue) return result;
        return null;
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        await Task.Run(() =>
        {
            //Log.Information($"Disconnected from Req-Resp Hub : {Context.ConnectionId} at {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");

            {
                var tupl = GetLastWillPayloadPacketMessage(Context.ConnectionId);
                if (tupl != null)
                {
                    var cdt = DateTime.Now;
                    long pktRef = Convert.ToInt64(cdt.Ticks / 1000);
                    tupl.Item2.Ref = pktRef;

                    PayloadPacket pkt = tupl.Item2;

                    try
                    {
                        Func<PayloadPacket, PayloadPacket> topicwiseHandler = GetTopicwiseHandler(pkt.Topic);
                        if (topicwiseHandler != null)
                        {
                            PayloadPacket _ = topicwiseHandler(pkt);
                            // RESULT IGNORED
                        }
                    }
                    catch // (Exception ex)
                    {
                        // SWALLOW FOR NOW
                    }
                }
            }
        });
    }
}
