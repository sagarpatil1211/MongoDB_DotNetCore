using Microsoft.VisualBasic.CompilerServices;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class InstructionRegisterEntry
{
    private readonly PropertyValueManager pvm = null;

    private InstructionRegisterEntry()
    {
        pvm = new PropertyValueManager();
    }

    private const string RefPropertyKey = "Ref";
    private const string InstructionTypePropertyKey = "InstructionTypeRef";
    private const string LogicalOrderPropertyKey = "LogicalOrder";
    private const string UniqueIdPropertyKey = "UniqueId";

    public int Ref
    {
        get
        {
            return pvm.GetPropertyValue<int>(RefPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(RefPropertyKey, value);
        }
    }

    public ChangeInstructionTypes InstructionType
    {
        get
        {
            return pvm.GetPropertyValue<ChangeInstructionTypes>(InstructionTypePropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(InstructionTypePropertyKey, Conversions.ToInteger(value));
        }
    }

    public int InstructionTypeRef
    {
        get
        {
            return Conversions.ToInteger(InstructionType);
        }
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

    public string UniqueId
    {
        get
        {
            return pvm.GetPropertyValue<string>(UniqueIdPropertyKey);
        }

        set
        {
            pvm.SetPropertyValue(UniqueIdPropertyKey, value);
        }
    }



    public static InstructionRegisterEntry DeriveFromInstruction(IDBCAInstruction instr, ChangeInstructionTypes instructionType)
    {
        return new InstructionRegisterEntry()
        {
            Ref = instr.Ref,
            InstructionType = instructionType,
            LogicalOrder = instr.LogicalOrder,
            UniqueId = instr.UniqueId
        };
    }
}
