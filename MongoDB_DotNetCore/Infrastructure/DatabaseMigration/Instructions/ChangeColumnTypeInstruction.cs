using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class ChangeColumnTypeInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string TableNamePropertyKey = "TableName";
    protected const string ColumnNamePropertyKey = "ColumnName";
    protected const string OldColumnTypePropertyKey = "OldColumnType";
    protected const string NewColumnTypePropertyKey = "NewColumnType";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public ChangeColumnTypeInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private ChangeColumnTypeInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public ChangeColumnTypeInstruction CreateCopy()
    {
        return new ChangeColumnTypeInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Change the type of column '" + ColumnName + "' in table '" + TableName + "' from '" + OldColumnType + "' to '" + NewColumnType + "'";
    }

    public string TableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(TableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(TableNamePropertyKey, value);
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
            pvm.SetPropertyValue(ColumnNamePropertyKey, value);
        }
    }

    public string OldColumnType
    {
        get
        {
            return pvm.GetPropertyValue<string>(OldColumnTypePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(OldColumnTypePropertyKey, value);
        }
    }

    public string NewColumnType
    {
        get
        {
            return pvm.GetPropertyValue<string>(NewColumnTypePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(NewColumnTypePropertyKey, value);
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

    public IDBCADataPersistenceResult<ChangeColumnTypeInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<ChangeColumnTypeInstruction>(lstEx);
        var rInternal = service.SaveChangeColumnTypeInstruction(this);
        return new PersistenceResult<ChangeColumnTypeInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (ChangeColumnTypeInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (TableName.Trim().Length == 0)
            lst.Add("Table Name not set.");
        if (ColumnName.Trim().Length == 0)
            lst.Add("Column Name cannot be blank.");
        if (OldColumnType.Trim().Length == 0)
            lst.Add("Old Column Type cannot be blank.");
        if (NewColumnType.Trim().Length == 0)
            lst.Add("New Column Type cannot be blank.");
        return lst;
    }
}
