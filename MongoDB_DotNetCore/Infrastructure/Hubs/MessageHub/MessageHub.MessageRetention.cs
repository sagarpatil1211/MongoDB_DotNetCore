using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Infrastructure.Extensions;

//name1space VJCore.Infrastructure.Hubs;

public partial class MessageHub
{
    private static readonly DbConnection cnnRetainedMessages = null;

    private static readonly string retainedMessagesTableName = "MessageHubRetainedMessages";

    private static readonly ConcurrentDictionary<string, RetainedMessageInfo> m_retainedMessages = new ConcurrentDictionary<string, RetainedMessageInfo>();

    private static void PerformDBMigrationForRetainedMessagesTable()
    {
        cnnRetainedMessages.Open();

        using var trs = cnnRetainedMessages.BeginTransaction();

        try
        {
            using var cmd = cnnRetainedMessages.CreateCommand();
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
            cnnRetainedMessages.Close();
        }
    }

    private static RetainedMessageInfo GetRetainedMessage(string topic)
    {
        if (m_retainedMessages.TryGetValue(topic, out RetainedMessageInfo result)) return result;
        return null;
    }

    private static void PopulateRetainedMessagesInMemory()
    {
        cnnRetainedMessages.Open();

        using var trs = cnnRetainedMessages.BeginTransaction();

        try
        {
            using var cmd = cnnRetainedMessages.CreateCommand();
            cmd.Transaction = trs;

            using var dta = DataAccessUtils.CreateDataAdapter(cmd);
            using var dtb = new DataTable();

            cmd.CommandText = $"SELECT * FROM {retainedMessagesTableName}";
            dta.Fill2(dtb);

            foreach (DataRow dr in dtb.Rows)
            {
                var rmi = RetainedMessageInfo.FromDataRow(dr);
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
            cnnRetainedMessages.Close();
        }
    }
}
