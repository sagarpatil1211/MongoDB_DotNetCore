using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class AddColumnToTableInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";
    protected const string ColumnNamePropertyKey = "ColumnName";
    protected const string TypeNameWithPrecisionAndScalePropertyKey = "TypeNameWithPrecisionAndScale";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public AddColumnToTableInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private AddColumnToTableInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public AddColumnToTableInstruction CreateCopy()
    {
        return new AddColumnToTableInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Add column '" + ColumnName + "' (" + TypeNameWithPrecisionAndScale + ") to table '" + TableName + "'";
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

    public string ColumnName
    {
        get
        {
            return pvm.GetPropertyValue<string>(ColumnNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(ColumnNamePropertyKey, value.Trim());
        }
    }

    public string TypeNameWithPrecisionAndScale
    {
        get
        {
            return pvm.GetPropertyValue<string>(TypeNameWithPrecisionAndScalePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(TypeNameWithPrecisionAndScalePropertyKey, value.Trim());
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

    public IDBCADataPersistenceResult<AddColumnToTableInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<AddColumnToTableInstruction>(lstEx);
        var rInternal = service.SaveAddColumnToTableInstruction(this);
        return new PersistenceResult<AddColumnToTableInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (AddColumnToTableInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name not set.");
        if (ColumnName.Trim().Length == 0)
            lst.Add("Column Name cannot be blank.");
        if (TypeNameWithPrecisionAndScale.Trim().Length == 0)
            lst.Add("Type Name with Precision not set.");
        return lst;
    }
}
