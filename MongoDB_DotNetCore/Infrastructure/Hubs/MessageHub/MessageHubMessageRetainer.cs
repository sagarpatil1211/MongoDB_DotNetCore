using Akka.Actor;
using System.Data.Common;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Infrastructure.Extensions;

//name1space VJCore.Infrastructure.Hubs;

public class MessageHubMessageRetainer : ReceiveActor
{
    private DbConnection cnnRetainedMessages = null;

    private readonly string retainedMessagesTableName = "MessageHubRetainedMessages";

    protected override SupervisorStrategy SupervisorStrategy()
    {
        return new OneForOneStrategy(_ => Directive.Resume);
    }

    protected override void PreStart()
    {

    }

    private void Initialize()
    {
        cnnRetainedMessages = DataAccessUtils.ProvisionAndCreateNewDBConnection("MessageHub");
    }

    public MessageHubMessageRetainer()
    {
        Initialize();

        Receive<RetainedMessageInfo>(msg =>
        {
            try
            {
                SaveRetainedMessage(msg);
            }
            catch // (Exception ex)
            {
                //Log.Error(ex.Message);
            }
        });
    }

    private void SaveRetainedMessage(RetainedMessageInfo msg)
    {
        string payloadType = msg.PayloadType;
        string topic = msg.Topic;
        string message = msg.Message;

        cnnRetainedMessages.Open();

        using var trs = cnnRetainedMessages.BeginTransaction();

        try
        {
            using var cmd = cnnRetainedMessages.CreateCommand();
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
            cnnRetainedMessages.Close();
        }
    }
}