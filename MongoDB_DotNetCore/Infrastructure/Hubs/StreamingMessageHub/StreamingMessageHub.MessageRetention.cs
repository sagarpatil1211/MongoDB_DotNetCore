using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using VJCore.Infrastructure.Extensions;

public partial class StreamingMessageHub
{
    private static readonly DbConnection cnnRetainedStreamingMessages = null;

    private static readonly string retainedMessagesTableName = "StreamingMessageHubRetainedStreamingMessages";

    private static readonly ConcurrentDictionary<string, RetainedStreamingMessageInfo> m_retainedMessages = new ConcurrentDictionary<string, RetainedStreamingMessageInfo>();

    private static void PerformDBMigrationForRetainedStreamingMessagesTable()
    {
        cnnRetainedStreamingMessages.Open();

        using var trs = cnnRetainedStreamingMessages.BeginTransaction();

        try
        {
            using var cmd = cnnRetainedStreamingMessages.CreateCommand();
            cmd.Transaction = trs;

            cmd.CommandText = $"SELECT COUNT(name) FROM sysobjects WHERE name = '{retainedMessagesTableName}' AND xType = 'U'";
            if (Utils.GetInt32(cmd.ExecuteScalar2()) == 0)
            {
                cmd.CommandText = $"CREATE TABLE {retainedMessagesTableName} (PayloadType varchar(256) NOT NULL, Topic varchar(768) NOT NULL PRIMARY KEY CLUSTERED, Message nvarchar(max) NOT NULL)";
                cmd.ExecuteNonQuery2();
            }

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

    private static RetainedStreamingMessageInfo GetRetainedStreamingMessage(string topic)
    {
        if (m_retainedMessages.TryGetValue(topic, out RetainedStreamingMessageInfo result)) return result;
        return null;
    }

    private static void PopulateRetainedStreamingMessagesInMemory()
    {
        cnnRetainedStreamingMessages.Open();

        using var trs = cnnRetainedStreamingMessages.BeginTransaction();

        try
        {
            using var cmd = cnnRetainedStreamingMessages.CreateCommand();
            cmd.Transaction = trs;

            using var dta = DataAccessUtils.CreateDataAdapter(cmd);
            using var dtb = new DataTable();

            cmd.CommandText = $"SELECT * FROM {retainedMessagesTableName}";
            dta.Fill2(dtb);

            foreach (DataRow dr in dtb.Rows)
            {
                var rmi = RetainedStreamingMessageInfo.FromDataRow(dr);
                _ = m_retainedMessages.TryAdd(rmi.Topic, rmi);
            }

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
