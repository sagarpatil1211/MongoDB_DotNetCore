using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class RenameFunctionInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string OldFunctionNamePropertyKey = "OldFunctionName";
    protected const string NewFunctionNamePropertyKey = "NewFunctionName";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public RenameFunctionInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private RenameFunctionInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public RenameFunctionInstruction CreateCopy()
    {
        return new RenameFunctionInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Rename function '" + OldFunctionName + "' to '" + NewFunctionName + "'";
    }

    public string OldFunctionName
    {
        get
        {
            return pvm.GetPropertyValue<string>(OldFunctionNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(OldFunctionNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public string NewFunctionName
    {
        get
        {
            return pvm.GetPropertyValue<string>(NewFunctionNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(NewFunctionNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
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

    public IDBCADataPersistenceResult<RenameFunctionInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<RenameFunctionInstruction>(lstEx);
        var rInternal = service.SaveRenameFunctionInstruction(this);
        return new PersistenceResult<RenameFunctionInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (RenameFunctionInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (OldFunctionName.Trim().Length == 0)
            lst.Add("Old Function Name cannot be blank.");
        if (NewFunctionName.Trim().Length == 0)
            lst.Add("New Function Name cannot be blank.");
        return lst;
    }
}
