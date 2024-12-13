using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public class ExecuteCustomCodeInstruction : DBCAInstructionBase
{
    private readonly IDBCADataAccessService service = null;
    protected const string CodeKeyPropertyKey = "CodeKey";

    public ExecuteCustomCodeInstruction(IDBCADataAccessService service) : base()
    {
        this.service = service;
    }

    private ExecuteCustomCodeInstruction(IDBCADataAccessService service, PropertyValueManager pvm) : base(pvm)
    {
        this.service = service;
    }

    public ExecuteCustomCodeInstruction CreateCopy()
    {
        return new ExecuteCustomCodeInstruction(service, pvm);
    }

    public override string GetDescriptiveStringWithoutLogicalOrder()
    {
        return "Execute Custom Code '" + CodeKey + "'";
    }

    public string CodeKey
    {
        get
        {
            return pvm.GetPropertyValue<string>(CodeKeyPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(CodeKeyPropertyKey, value.Replace(ControlChars.NewLine, string.Empty));
        }
    }

    public IDBCADataPersistenceResult<ExecuteCustomCodeInstruction> Save()
    {
        var lstEx = Validate();
        if (lstEx.Count > 0)
            return new PersistenceValidationResult<ExecuteCustomCodeInstruction>(lstEx);
        var rInternal = service.SaveExecuteCustomCodeInstruction(this);
        return new PersistenceResult<ExecuteCustomCodeInstruction>(rInternal.Successful, rInternal.ExceptionMessage, (ExecuteCustomCodeInstruction)rInternal.Tag);
    }

    public override List<string> Validate()
    {
        var lst = new List<string>();
        if (CodeKey.Trim().Length == 0)
            lst.Add("Code Key cannot be blank.");
        return lst;
    }
}
