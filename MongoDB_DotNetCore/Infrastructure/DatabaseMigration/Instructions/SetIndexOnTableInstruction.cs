using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class SetIndexOnTableInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";
    protected const string ColumnsPropertyKey = "Columns";

    public SetIndexOnTableInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private SetIndexOnTableInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public SetIndexOnTableInstruction CreateCopy()
    {
        return new SetIndexOnTableInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return $"Set Index on Table {TableName} with columns {Columns}";
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

    public IDBCADataPersistenceResult<SetIndexOnTableInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<SetIndexOnTableInstruction>(lstEx);
        var rInternal = service.SaveSetIndexOnTableInstruction(this);
        return new PersistenceResult<SetIndexOnTableInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (SetIndexOnTableInstruction)rInternal.Tag);
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
