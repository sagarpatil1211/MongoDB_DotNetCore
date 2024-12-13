using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class CreateFunctionInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string QueryPropertyKey = "Query";
    protected const string FunctionNamePropertyKey = "FunctionName";

    public CreateFunctionInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private CreateFunctionInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public CreateFunctionInstruction CreateCopy()
    {
        return new CreateFunctionInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Create Function '" + FunctionName + "' as follows : '" + Query + "'";
    }

    public string FunctionName
    {
        get
        {
            return pvm.GetPropertyValue<string>(FunctionNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(FunctionNamePropertyKey, value.Trim());
        }
    }

    public string Query
    {
        get
        {
            return pvm.GetPropertyValue<string>(QueryPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(QueryPropertyKey, DBCAService.FormatQuery(value));
        }
    }

    public IDBCADataPersistenceResult<CreateFunctionInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<CreateFunctionInstruction>(lstEx);
        var rInternal = service.SaveCreateFunctionInstruction(this);
        return new PersistenceResult<CreateFunctionInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (CreateFunctionInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (FunctionName.Trim().Length == 0)
            lst.Add("Function Name cannot be blank.");
        if (Query.Trim().Length == 0)
            lst.Add("Query cannot be blank.");
        return lst;
    }
}
