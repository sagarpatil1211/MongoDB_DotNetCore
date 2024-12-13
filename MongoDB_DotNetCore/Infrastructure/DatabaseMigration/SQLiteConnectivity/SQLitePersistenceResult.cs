//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.SQLiteConnectivity;

public class SQLitePersistenceResult<T> : IDBCADataPersistenceResult<T>
{
    public SQLitePersistenceResult()
    {
        Successful = false;
        ExceptionMessage = string.Empty;
        Tag = default;
    }

    public SQLitePersistenceResult(bool successful, string exceptionMessage, T tag)
    {
        Successful = successful;
        ExceptionMessage = exceptionMessage;
        Tag = tag;
    }

    public string ExceptionMessage { get; set; }
    public bool Successful { get; set; }
    public T Tag { get; set; }
}
//}