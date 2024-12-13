using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class DropViewInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string ViewNamePropertyKey = "ViewName";

    public DropViewInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private DropViewInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public DropViewInstruction CreateCopy()
    {
        return new DropViewInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Drop view " + ViewName;
    }

    public string ViewName
    {
        get
        {
            return pvm.GetPropertyValue<string>(ViewNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(ViewNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public IDBCADataPersistenceResult<DropViewInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<DropViewInstruction>(lstEx);
        var rInternal = service.SaveDropViewInstruction(this);
        return new PersistenceResult<DropViewInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (DropViewInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (ViewName.Trim().Length == 0)
            lst.Add("View Name cannot be blank.");
        return lst;
    }
}
