//using System.Data;
//using System.Data.Common;

//public class SqlDataCommand : DbCommand
//{
//    private DbCommand _command;
//    private bool _disposed;


//    public override string CommandText
//    {
//        get => _command.CommandText;
//        set => _command.CommandText = value;
//    }


//    public override int CommandTimeout
//    {
//        get => _command.CommandTimeout;
//        set => _command.CommandTimeout = value;
//    }


//    public override CommandType CommandType
//    {
//        get => _command.CommandType;
//        set => _command.CommandType = value;
//    }


//    public override UpdateRowSource UpdatedRowSource
//    {
//        get => _command.UpdatedRowSource;
//        set => _command.UpdatedRowSource = value;
//    }


//    protected override DbConnection DbConnection
//    {
//        get => _command.Connection;
//        set => _command.Connection = value;
//    }


//    protected override DbParameterCollection DbParameterCollection => _command.Parameters;


//    protected override DbTransaction DbTransaction
//    {
//        get => _command.Transaction;
//        set => _command.Transaction = value;
//    }


//    public override bool DesignTimeVisible
//    {
//        get => _command.DesignTimeVisible;
//        set => _command.DesignTimeVisible = value;
//    }


//    public SqlDataCommand(DbCommand Command)
//    {
//        _command = Command;
//    }


//    ~SqlDataCommand() => Dispose(false);


//    protected override void Dispose(bool Disposing)
//    {
//        if (_disposed) return;
//        if (Disposing)
//        {
//            // No managed resources to release.
//        }
//        // Release unmanaged resources.
//        _command?.Dispose();
//        _command = null;
//        // Do not release logger.  Its lifetime is controlled by caller.
//        _disposed = true;
//    }


//    public override void Cancel()
//    {
//        _command.Cancel();
//    }


//    public override int ExecuteNonQuery()
//    {
//        int result = _command.ExecuteNonQuery2();
//        return result;
//    }


//    public override object ExecuteScalar()
//    {
//        return _command.ExecuteScalar2();
//    }


//    public override void Prepare()
//    {
//        _command.Prepare();
//    }


//    protected override DbParameter CreateDbParameter() => _command.CreateParameter();


//    protected override DbDataReader ExecuteDbDataReader(CommandBehavior Behavior)
//    {
//        return _command.ExecuteReader(Behavior);
//    }
//}