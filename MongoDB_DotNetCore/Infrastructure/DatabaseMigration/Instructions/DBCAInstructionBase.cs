using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.Instructions;

public abstract class DBCAInstructionBase : IDBCAInstruction
{
    protected PropertyValueManager pvm = null;
    protected const string RefPropertyKey = "Ref";
    protected const string InstructionRefPropertyKey = "InstructionRef";
    protected const string InstructionTypeRefPropertyKey = "InstructionTypeRef";
    protected const string LogicalOrderPropertyKey = "LogicalOrder";
    protected const string ApplicableModuleRefPropertyKey = "ApplicableModuleRef";
    protected const string UniqueIdPropertyKey = "UniqueId";

    public abstract List<string> Validate();

    private void SetUniqueIdIfRequired()
    {
        if (UniqueId == null)
        {
            UniqueId = Guid.NewGuid().ToString();
        }
        else if (UniqueId.Trim().Length == 0)
        {
            UniqueId = Guid.NewGuid().ToString();
        }
    }

    protected DBCAInstructionBase()
    {
        pvm = new PropertyValueManager();
        SetUniqueIdIfRequired();
    }

    protected DBCAInstructionBase(PropertyValueManager pvm)
    {
        this.pvm = new PropertyValueManager(pvm);
        SetUniqueIdIfRequired();
    }

    public string GetDescriptiveStringWithLogicalOrder()
    {
        return LogicalOrder.ToString("00000") + ". " + GetDescriptiveStringWithoutLogicalOrder();
    }

    public abstract string GetDescriptiveStringWithoutLogicalOrder();

    public void GetPropertyValuesFromPersistenceColumnMapper(IDBCAPersistenceColumnMapper mapper)
    {
        foreach (KeyValuePair<string, object> kvp in mapper.GetColumnNameAndValueCollection())
            pvm.SetPropertyValue(kvp.Key, kvp.Value);
    }

    public int LogicalOrder
    {
        get
        {
            return pvm.GetPropertyValue<int>(LogicalOrderPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(LogicalOrderPropertyKey, value);
        }
    }

    public int InstructionTypeRef
    {
        get
        {
            return pvm.GetPropertyValue<int>(InstructionTypeRefPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(InstructionTypeRefPropertyKey, value);
        }
    }

    public int Ref
    {
        get
        {
            return pvm.GetPropertyValue<int>(RefPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(RefPropertyKey, value);
            pvm.SetPropertyValue(InstructionRefPropertyKey, value);
        }
    }

    public string UniqueId
    {
        get
        {
            return pvm.GetPropertyValue<string>(UniqueIdPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(UniqueIdPropertyKey, value);
            pvm.SetPropertyValue(InstructionRefPropertyKey, value);
        }
    }

    public void SetPropertyValuesToPersistenceColumnMapper(IDBCAPersistenceColumnMapper mapper)
    {
        foreach (JProperty prop in pvm.GetProperties())
            mapper.SetPropertyValueAtColumnName(prop.Value, prop.Name);
    }
}
