using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class DropColumnInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";
    protected const string ColumnNamePropertyKey = "ColumnName";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public DropColumnInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private DropColumnInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public DropColumnInstruction CreateCopy()
    {
        return new DropColumnInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Drop column '" + ColumnName + "' in table '" + TableName;
    }

    public string TableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(TableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(TableNamePropertyKey, value);
        }
    }

    public string ColumnName
    {
        get
        {
            return pvm.GetPropertyValue<string>(ColumnNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(ColumnNamePropertyKey, value);
        }
    }

    public bool SkipDependentObjectReformulation
    {
        get
        {
            return pvm.GetPropertyValue<bool>(SkipDependentObjectReformulationPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(SkipDependentObjectReformulationPropertyKey, value);
        }
    }

    public IDBCADataPersistenceResult<DropColumnInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<DropColumnInstruction>(lstEx);
        var rInternal = service.SaveDropColumnInstruction(this);
        return new PersistenceResult<DropColumnInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (DropColumnInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name not set.");
        if (ColumnName.Trim().Length == 0)
            lst.Add("Column Name cannot be blank.");
        return lst;
    }
}
