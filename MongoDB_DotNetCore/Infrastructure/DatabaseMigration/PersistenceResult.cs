//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class PersistenceResult<T> : IDBCADataPersistenceResult<T>
{
    public PersistenceResult(bool successful, string exceptionMessage, T tag)
    {
        Successful = successful;
        ExceptionMessage = exceptionMessage;
        Tag = tag;
    }

    public string ExceptionMessage { get; set; }
    public bool Successful { get; set; }
    public T Tag { get; set; }
}
