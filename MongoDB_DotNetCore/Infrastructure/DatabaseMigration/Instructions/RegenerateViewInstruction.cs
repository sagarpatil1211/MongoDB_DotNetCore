using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class RegenerateViewInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string ViewNamePropertyKey = "ViewName";

    public RegenerateViewInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private RegenerateViewInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public RegenerateViewInstruction CreateCopy()
    {
        return new RegenerateViewInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Regenerate view " + ViewName;
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

    public IDBCADataPersistenceResult<RegenerateViewInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<RegenerateViewInstruction>(lstEx);
        var rInternal = service.SaveRegenerateViewInstruction(this);
        return new PersistenceResult<RegenerateViewInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (RegenerateViewInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (ViewName.Trim().Length == 0)
            lst.Add("View Name cannot be blank.");
        return lst;
    }
}
