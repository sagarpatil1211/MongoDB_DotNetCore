//name1space VJCore.Infrastructure.DatabaseMigration.Interfaces;

public interface IDBCAInstruction
{
    int Ref { get; set; }
    int LogicalOrder { get; set; }
    string UniqueId { get; set; }

    void GetPropertyValuesFromPersistenceColumnMapper(IDBCAPersistenceColumnMapper mapper);
    void SetPropertyValuesToPersistenceColumnMapper(IDBCAPersistenceColumnMapper mapper);

    int InstructionTypeRef { get; set; }

    string GetDescriptiveStringWithLogicalOrder();
    string GetDescriptiveStringWithoutLogicalOrder();
}