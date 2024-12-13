using System.Collections.Generic;

//name1space VJCore.Infrastructure.DatabaseMigration.Interfaces;

public interface IDBCAPersistenceColumnMapper
{
    void SetPropertyValueAtColumnName<T>(T value, string columnName, bool ignoreIfColumnNotFound = true);
    T GetPropertyValueFromColumnName<T>(string columnName);
    Dictionary<string, object> GetColumnNameAndValueCollection();
}
