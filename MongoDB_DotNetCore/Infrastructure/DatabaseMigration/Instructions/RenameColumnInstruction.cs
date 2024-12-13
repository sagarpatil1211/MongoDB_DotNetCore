using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;


public class RenameColumnInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";
    protected const string OldColumnNamePropertyKey = "OldColumnName";
    protected const string NewColumnNamePropertyKey = "NewColumnName";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public RenameColumnInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private RenameColumnInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public string ColumnName { get; set; }

    public RenameColumnInstruction CreateCopy()
    {
        return new RenameColumnInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Rename column '" + OldColumnName + "' in table '" + TableName + "' to '" + NewColumnName + "'";
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

    public string OldColumnName
    {
        get
        {
            return pvm.GetPropertyValue<string>(OldColumnNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(OldColumnNamePropertyKey, value);
        }
    }

    public string NewColumnName
    {
        get
        {
            return pvm.GetPropertyValue<string>(NewColumnNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(NewColumnNamePropertyKey, value);
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

    public IDBCADataPersistenceResult<RenameColumnInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<RenameColumnInstruction>(lstEx);
        var rInternal = service.SaveRenameColumnInstruction(this);
        return new PersistenceResult<RenameColumnInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (RenameColumnInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name not set.");
        if (OldColumnName.Trim().Length == 0)
            lst.Add("Old Column Name cannot be blank.");
        if (NewColumnName.Trim().Length == 0)
            lst.Add("New Column Name cannot be blank.");
        return lst;
    }
}
