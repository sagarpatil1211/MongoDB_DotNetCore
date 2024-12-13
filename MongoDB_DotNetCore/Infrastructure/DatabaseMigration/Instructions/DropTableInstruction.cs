using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;


public class DropTableInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";

    public DropTableInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private DropTableInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public DropTableInstruction CreateCopy()
    {
        return new DropTableInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Drop table " + TableName;
    }

    public string TableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(TableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(TableNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public IDBCADataPersistenceResult<DropTableInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<DropTableInstruction>(lstEx);
        var rInternal = service.SaveDropTableInstruction(this);
        return new PersistenceResult<DropTableInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (DropTableInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name cannot be blank.");
        return lst;
    }
}
