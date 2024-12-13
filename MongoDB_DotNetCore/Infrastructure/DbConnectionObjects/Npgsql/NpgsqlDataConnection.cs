//using Npgsql;
//using System.Data;
//using System.Data.Common;

//public class NpgsqlDataConnection : DbConnection
//{
//    protected DbConnection Connection;
//    private bool _disposed;


//    public override string ConnectionString
//    {
//        get => Connection.ConnectionString;
//        set => Connection.ConnectionString = value;
//    }


//    public override string Database => Connection.Database;


//    public override string DataSource => Connection.DataSource;


//    public override string ServerVersion => Connection.ServerVersion;


//    public override ConnectionState State => Connection.State;


//    public NpgsqlDataConnection(string connectionString)
//    {
//        Connection = new NpgsqlConnection(connectionString);
//    }

//    ~NpgsqlDataConnection() => Dispose(false);


//    protected override void Dispose(bool Disposing)
//    {
//        if (_disposed) return;
//        if (Disposing)
//        {
//            // No managed resources to release.
//        }
//        // Release unmanaged resources.
//        Connection?.Dispose();
//        Connection = null;
//        // Do not release logger.  Its lifetime is controlled by caller.
//        _disposed = true;
//    }


//    public override void ChangeDatabase(string DatabaseName)
//    {
//        Connection.ChangeDatabase(DatabaseName);
//    }


//    public override void Close()
//    {
//        Connection.Close();
//    }


//    public override void Open()
//    {
//        Connection.Open();
//    }


//    protected override DbTransaction BeginDbTransaction(IsolationLevel IsolationLevel)
//    {
//        return Connection.BeginTransaction();
//    }

//    protected override DbCommand CreateDbCommand()
//    {
//        return new SqlDataCommand(Connection.CreateCommand());
//    }
//}