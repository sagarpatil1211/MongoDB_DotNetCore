using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class ExecuteCustomQueryInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string QueryPropertyKey = "Query";

    public ExecuteCustomQueryInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private ExecuteCustomQueryInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public ExecuteCustomQueryInstruction CreateCopy()
    {
        return new ExecuteCustomQueryInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Execute the following query : '" + Query + "'";
    }

    public string Query
    {
        get
        {
            return pvm.GetPropertyValue<string>(QueryPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(QueryPropertyKey, value.Trim());
        }
    }

    public IDBCADataPersistenceResult<ExecuteCustomQueryInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<ExecuteCustomQueryInstruction>(lstEx);
        var rInternal = service.SaveExecuteCustomQueryInstruction(this);
        return new PersistenceResult<ExecuteCustomQueryInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (ExecuteCustomQueryInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (Query.Trim().Length == 0)
            lst.Add("Query cannot be blank.");
        return lst;
    }
}
