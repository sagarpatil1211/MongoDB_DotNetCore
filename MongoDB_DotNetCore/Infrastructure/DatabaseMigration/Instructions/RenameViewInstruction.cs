using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;


public class RenameViewInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string OldViewNamePropertyKey = "OldViewName";
    protected const string NewViewNamePropertyKey = "NewViewName";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public RenameViewInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private RenameViewInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public RenameViewInstruction CreateCopy()
    {
        return new RenameViewInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Rename view '" + OldViewName + "' to '" + NewViewName + "'";
    }

    public string OldViewName
    {
        get
        {
            return pvm.GetPropertyValue<string>(OldViewNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(OldViewNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public string NewViewName
    {
        get
        {
            return pvm.GetPropertyValue<string>(NewViewNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(NewViewNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
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

    public IDBCADataPersistenceResult<RenameViewInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<RenameViewInstruction>(lstEx);
        var rInternal = service.SaveRenameViewInstruction(this);
        return new PersistenceResult<RenameViewInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (RenameViewInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (OldViewName.Trim().Length == 0)
            lst.Add("Old View Name cannot be blank.");
        if (NewViewName.Trim().Length == 0)
            lst.Add("New View Name cannot be blank.");
        return lst;
    }
}
