//name1space VJCore.Infrastructure.DatabaseMigration.Interfaces;

public interface IDBCADataQueryResult<T>
{
    bool Successful { get; set; }
    string ExceptionMessage { get; set; }
    T Tag { get; set; }
}
//}