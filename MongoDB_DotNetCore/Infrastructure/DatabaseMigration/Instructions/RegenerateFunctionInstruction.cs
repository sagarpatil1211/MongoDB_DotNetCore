using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class RegenerateFunctionInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string FunctionNamePropertyKey = "FunctionName";

    public RegenerateFunctionInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private RegenerateFunctionInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public RegenerateFunctionInstruction CreateCopy()
    {
        return new RegenerateFunctionInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Regenerate Function " + FunctionName;
    }

    public string FunctionName
    {
        get
        {
            return pvm.GetPropertyValue<string>(FunctionNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(FunctionNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public IDBCADataPersistenceResult<RegenerateFunctionInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<RegenerateFunctionInstruction>(lstEx);
        var rInternal = service.SaveRegenerateFunctionInstruction(this);
        return new PersistenceResult<RegenerateFunctionInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (RegenerateFunctionInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (FunctionName.Trim().Length == 0)
            lst.Add("Function Name cannot be blank.");
        return lst;
    }
}
