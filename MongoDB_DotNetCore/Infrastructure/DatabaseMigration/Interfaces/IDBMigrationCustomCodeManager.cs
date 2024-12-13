using System.Data.Common;

public interface IDBMigrationCustomCodeManager
{
    void ExecuteCustomCode(IDBCADataAccessService instructionDataAccessService, 
        string codeKey, DbConnection cnn, DbTransaction trs);
}
