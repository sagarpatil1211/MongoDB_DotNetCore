using System.Collections.Generic;

//name1space VJCore.Infrastructure.DatabaseMigration.Interfaces;

public interface IDBCADataAccessService
{
    IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionList();
    IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListByLogicalOrderRange(int fromLogicalOrder, int toLogicalOrder);
    IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListByLogicalOrderOnwards(int fromLogicalOrderOnwards);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateTableInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameTableInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveDropTableInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveAddColumnToTableInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameColumnInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveDropColumnInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeColumnTypeInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateViewInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameViewInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeViewDefinitionInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveDropViewInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveRegenerateViewInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateFunctionInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameFunctionInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeFunctionDefinitionInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveDropFunctionInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveRegenerateFunctionInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveExecuteCustomQueryInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveExecuteCustomCodeInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveSetPrimaryKeyOnTableInstruction(IDBCAInstruction instr);
    IDBCADataPersistenceResult<IDBCAInstruction> SaveSetIndexOnTableInstruction(IDBCAInstruction instr);

    IDBCAInstruction GetCreateTableInstruction(string tableName);
    IDBCAInstruction GetSetPrimaryKeyOnTableInstruction(string tableName, string columnNames);
    IDBCAInstruction GetSetIndexOnTableInstruction(string tableName, string columnNames);

    IDBCADataPersistenceResult<string> DeleteInstruction(IDBCAInstruction instr);
}
//}