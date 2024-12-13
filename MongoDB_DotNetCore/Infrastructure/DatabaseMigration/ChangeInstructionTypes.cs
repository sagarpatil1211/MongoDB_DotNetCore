//name1space VJCore.Infrastructure.DatabaseMigration;

public enum ChangeInstructionTypes
{
    CreateTable = 1,
    RenameTable = 2,
    DropTable = 3,
    AddColumnToTable = 4,
    RenameColumn = 5,
    DropColumn = 6,
    ChangeColumnType = 7,
    CreateView = 8,
    RenameView = 9,
    ChangeViewDefinition = 10,
    DropView = 11,
    RegenerateView = 12,
    CreateFunction = 13,
    ChangeFunctionDefinition = 14,
    RenameFunction = 15,
    DropFunction = 16,
    RegenerateFunction = 17,
    ExecuteCustomCode = 18,
    ExecuteCustomQuery = 19,
    SetPrimaryKeyOnTable = 20,
    SetIndexOnTable = 21
}
