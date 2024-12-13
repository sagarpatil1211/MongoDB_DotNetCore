using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;


public class DropFunctionInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string FunctionNamePropertyKey = "FunctionName";

    public DropFunctionInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private DropFunctionInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public DropFunctionInstruction CreateCopy()
    {
        return new DropFunctionInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Drop Function " + FunctionName;
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

    public IDBCADataPersistenceResult<DropFunctionInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<DropFunctionInstruction>(lstEx);
        var rInternal = service.SaveDropFunctionInstruction(this);
        return new PersistenceResult<DropFunctionInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (DropFunctionInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (FunctionName.Trim().Length == 0)
            lst.Add("Function Name cannot be blank.");
        return lst;
    }
}
