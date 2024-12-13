//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.SQLiteConnectivity;
public class SQLiteQueryResult<T> : IDBCADataQueryResult<T>
{
    public SQLiteQueryResult()
    {
        Successful = false;
        ExceptionMessage = string.Empty;
        Tag = default;
    }

    public string ExceptionMessage { get; set; }
    public bool Successful { get; set; }
    public T Tag { get; set; }
}
