using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration;

public abstract class DataAccessServiceBase : IDBCADataAccessService
{
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateTableInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameTableInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveDropTableInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveAddColumnToTableInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameColumnInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveDropColumnInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeColumnTypeInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateViewInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameViewInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeViewDefinitionInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveDropViewInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveRegenerateViewInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateFunctionInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameFunctionInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeFunctionDefinitionInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveDropFunctionInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveRegenerateFunctionInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveExecuteCustomQueryInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveExecuteCustomCodeInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveSetPrimaryKeyOnTableInstruction(IDBCAInstruction instr);
    public abstract IDBCADataPersistenceResult<IDBCAInstruction> SaveSetIndexOnTableInstruction(IDBCAInstruction instr);

    public abstract IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionList();
    public abstract IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListByLogicalOrderOnwards(int fromLogicalOrderOnwards);
    public abstract IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListByLogicalOrderRange(int fromLogicalOrder, int toLogicalOrder);
    public abstract IDBCAInstruction GetCreateTableInstruction(string tableName);
    public abstract IDBCAInstruction GetSetPrimaryKeyOnTableInstruction(string tableName, string columnNames);
    public abstract IDBCAInstruction GetSetIndexOnTableInstruction(string tableName, string columnNames);

    public abstract IDBCADataPersistenceResult<string> DeleteInstruction(IDBCAInstruction instr);
}
