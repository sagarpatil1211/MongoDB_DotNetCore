using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;


public class ChangeViewDefinitionInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string ViewNamePropertyKey = "ViewName";
    protected const string NewQueryPropertyKey = "NewQuery";
    protected const string SkipDependentObjectReformulationPropertyKey = "SkipDependentObjectReformulation";

    public ChangeViewDefinitionInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private ChangeViewDefinitionInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public ChangeViewDefinitionInstruction CreateCopy()
    {
        return new ChangeViewDefinitionInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Change the definition of " + ViewName + " as '" + NewQuery + "'";
    }

    public string ViewName
    {
        get
        {
            return pvm.GetPropertyValue<string>(ViewNamePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(ViewNamePropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
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
            pvm.SetPropertyValue(NewQueryPropertyKey, DBCAService.FormatViewCreationQuery(value));
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

    public IDBCADataPersistenceResult<ChangeViewDefinitionInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<ChangeViewDefinitionInstruction>(lstEx);
        var rInternal = service.SaveChangeViewDefinitionInstruction(this);
        return new PersistenceResult<ChangeViewDefinitionInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (ChangeViewDefinitionInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (ViewName.Trim().Length == 0)
            lst.Add("View Name cannot be blank.");
        if (NewQuery.Trim().Length == 0)
            lst.Add("New Query cannot be blank.");
        return lst;
    }
}