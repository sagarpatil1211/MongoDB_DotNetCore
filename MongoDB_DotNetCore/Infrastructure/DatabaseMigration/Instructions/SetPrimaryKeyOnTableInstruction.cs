using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class SetPrimaryKeyOnTableInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";
    protected const string ColumnsPropertyKey = "Columns";

    public SetPrimaryKeyOnTableInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private SetPrimaryKeyOnTableInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public SetPrimaryKeyOnTableInstruction CreateCopy()
    {
        return new SetPrimaryKeyOnTableInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return $"Set Primary Key on Table {TableName} with columns {Columns}";
    }

    public string TableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(TableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(TableNamePropertyKey, value.Trim());
        }
    }

    public string Columns
    {
        get
        {
            return pvm.GetPropertyValue<string>(ColumnsPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(ColumnsPropertyKey, value.Trim());
        }
    }

    public IDBCADataPersistenceResult<SetPrimaryKeyOnTableInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<SetPrimaryKeyOnTableInstruction>(lstEx);
        var rInternal = service.SaveSetPrimaryKeyOnTableInstruction(this);
        return new PersistenceResult<SetPrimaryKeyOnTableInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (SetPrimaryKeyOnTableInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name not set.");
        if (Columns.Trim().Length == 0)
            lst.Add("Columns cannot be blank.");
        return lst;
    }
}
