//name1space VJCore.Infrastructure.DatabaseMigration.Interfaces;

public interface IDBCADataPersistenceResult<T>
{
    bool Successful { get; set; }
    string ExceptionMessage { get; set; }
    T Tag { get; set; }
}
//}