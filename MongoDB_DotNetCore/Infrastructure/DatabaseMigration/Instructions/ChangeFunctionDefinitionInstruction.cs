using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class ChangeFunctionDefinitionInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string FunctionNamePropertyKey = "FunctionName";
    protected const string NewQueryPropertyKey = "NewQuery";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public ChangeFunctionDefinitionInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private ChangeFunctionDefinitionInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public ChangeFunctionDefinitionInstruction CreateCopy()
    {
        return new ChangeFunctionDefinitionInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Change the definition of " + FunctionName + " as '" + NewQuery + "'";
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

    public string NewQuery
    {
        get
        {
            return pvm.GetPropertyValue<string>(NewQueryPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(NewQueryPropertyKey, DBCAService.FormatQuery(value));
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

    public IDBCADataPersistenceResult<ChangeFunctionDefinitionInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<ChangeFunctionDefinitionInstruction>(lstEx);
        var rInternal = service.SaveChangeFunctionDefinitionInstruction(this);
        return new PersistenceResult<ChangeFunctionDefinitionInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (ChangeFunctionDefinitionInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (FunctionName.Trim().Length == 0)
            lst.Add("Function Name cannot be blank.");
        if (NewQuery.Trim().Length == 0)
            lst.Add("New Query cannot be blank.");
        return lst;
    }
}