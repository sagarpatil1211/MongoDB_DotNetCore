using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;


public class CreateViewInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string QueryPropertyKey = "Query";
    protected const string ViewNamePropertyKey = "ViewName";

    public CreateViewInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private CreateViewInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public CreateViewInstruction CreateCopy()
    {
        return new CreateViewInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Create view '" + ViewName + "' as follows : '" + Query + "'";
    }

    public string ViewName
    {
        get
        {
            return pvm.GetPropertyValue<string>(ViewNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(ViewNamePropertyKey, value.Trim());
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
            pvm.SetPropertyValue(QueryPropertyKey, DBCAService.FormatViewCreationQuery(value));
        }
    }

    public IDBCADataPersistenceResult<CreateViewInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<CreateViewInstruction>(lstEx);
        var rInternal = service.SaveCreateViewInstruction(this);
        return new PersistenceResult<CreateViewInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (CreateViewInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (ViewName.Trim().Length == 0)
            lst.Add("View Name cannot be blank.");
        if (Query.Trim().Length == 0)
            lst.Add("Query cannot be blank.");
        return lst;
    }
}
