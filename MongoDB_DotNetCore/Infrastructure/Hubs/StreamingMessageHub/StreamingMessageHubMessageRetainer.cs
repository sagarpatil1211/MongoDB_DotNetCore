using Akka.Actor;
//using Serilog;
using System;
using System.Data.Common;
using VJCore.Infrastructure.Extensions;

public class StreamingMessageHubMessageRetainer : ReceiveActor
{
    private DbConnection cnnRetainedStreamingMessages = null;

    private readonly string retainedMessagesTableName = "StreamingMessageHubRetainedStreamingMessages";

    protected override SupervisorStrategy SupervisorStrategy()
    {
        return new OneForOneStrategy(_ => Directive.Resume);
    }

    protected override void PreStart()
    {
        
    }

    private void Initialize()
    {
        cnnRetainedStreamingMessages = DataAccessUtils.ProvisionAndCreateNewDBConnection("StreamingMessageHub");
    }

    public StreamingMessageHubMessageRetainer()
    {
        Initialize();

        Receive<RetainedStreamingMessageInfo>(msg =>
        {
            try
            {
                SaveRetainedStreamingMessage(msg);
            }
            catch // (Exception ex)
            {
                //Log.Error(ex.Message);
            }
        });
    }

    private void SaveRetainedStreamingMessage(RetainedStreamingMessageInfo msg)
    {
        string payloadType = msg.PayloadType;
        string topic = msg.Topic;
        string message = msg.Message;

        cnnRetainedStreamingMessages.Open();

        using var trs = cnnRetainedStreamingMessages.BeginTransaction();

        try
        {
            using var cmd = cnnRetainedStreamingMessages.CreateCommand();
            cmd.Transaction = trs;

            cmd.CommandText = $"DELETE FROM {retainedMessagesTableName} WHERE Topic = @Topic";
            cmd.Parameters.Clear();
            cmd.AddVarcharParameter("@Topic", retainedMessagesTableName, topic);
            cmd.ExecuteNonQuery2();

            cmd.CommandText = $"INSERT INTO {retainedMessagesTableName} (PayloadType, Topic, Message) VALUES (@PayloadType, @Topic, @Message)";
            cmd.Parameters.Clear();
            cmd.AddVarcharMaxParameter("@PayloadType", payloadType);
            cmd.AddVarcharMaxParameter("@Topic", topic);
            cmd.AddVarcharMaxParameter("@Message", message);
            cmd.ExecuteNonQuery2();

            trs.Commit();
        }
        catch
        {
            trs.Rollback();
        }
        finally
        {
            cnnRetainedStreamingMessages.Close();
        }
    }
}
