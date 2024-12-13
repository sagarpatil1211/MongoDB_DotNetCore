using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class RenameTableInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string OldTableNamePropertyKey = "OldTableName";
    protected const string NewTableNamePropertyKey = "NewTableName";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public RenameTableInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private RenameTableInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public RenameTableInstruction CreateCopy()
    {
        return new RenameTableInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Rename table " + OldTableName + " to " + NewTableName;
    }

    public string OldTableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(OldTableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(OldTableNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public string NewTableName
    {
        get
        {
            return pvm.GetPropertyValue<string>(NewTableNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(NewTableNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
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

    public IDBCADataPersistenceResult<RenameTableInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<RenameTableInstruction>(lstEx);
        var rInternal = service.SaveRenameTableInstruction(this);
        return new PersistenceResult<RenameTableInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (RenameTableInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (OldTableName.Trim().Length == 0)
            lst.Add("Old Table Name cannot be blank.");
        if (NewTableName.Trim().Length == 0)
            lst.Add("New Table Name cannot be blank.");
        return lst;
    }
}
