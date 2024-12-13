using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class CreateTableInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string QueryPropertyKey = "Query";
    protected const string TableNamePropertyKey = "TableName";

    public CreateTableInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private CreateTableInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public CreateTableInstruction CreateCopy()
    {
        return new CreateTableInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Create table '" + TableName + "' as follows : '" + Query + "'";
    }

    public string TableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(TableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(TableNamePropertyKey, value.Trim());
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
            pvm.SetPropertyValue(QueryPropertyKey, DBCAService.FormatTableCreationQuery(value));
        }
    }

    public IDBCADataPersistenceResult<CreateTableInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<CreateTableInstruction>(lstEx);
        var rInternal = service.SaveCreateTableInstruction(this);
        return new PersistenceResult<CreateTableInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (CreateTableInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name cannot be blank.");
        if (Query.Trim().Length == 0)
            lst.Add("Query cannot be blank.");
        return lst;
    }
}