using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
//using VJ1Core.Infrastructure.DatabaseMigration.Instructions;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration.SQLiteConnectivity;


public class SQLiteDataAccessService : DataAccessServiceBase
{
    private readonly SQLiteConnection cnn = null;
    private const string InstructionRegisterTableName = "InstructionRegister";
    private readonly Action<string, string> ErrorMessageDisplayer = null;
    // 1st Parameter = Error Message
    // 2nd Parameter = Message Box Title

    public SQLiteDataAccessService(string folderPath, Action<string, string> ErrorMessageDisplayer)
    {
        this.ErrorMessageDisplayer = ErrorMessageDisplayer;
        string instructionFilePath = string.Empty;

        if (folderPath.Trim().Length > 0)
        {
            //if (!folderPath.EndsWith("\\")) folderPath += "\\";
            instructionFilePath = Path.Combine(folderPath, @"SSDBCAinst.db");
        }
        else
        {
            instructionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SSDBCAinst.db");
        }

        Console.WriteLine($"Folder Path : {folderPath}");
        Console.WriteLine($"Instruction File Path : {instructionFilePath}");

        cnn = new SQLiteConnection($"Data Source = {instructionFilePath}");

        CheckAndFormulateInstructionDBStructureIfNecessary();
    }

    private void CheckAndFormulateInstructionDBStructureIfNecessary()
    {
        cnn.Open();

        try
        {
            using SQLiteCommand cmd = cnn.CreateCommand();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""AddColumnToTableInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""ColumnName"" TEXT, ""TypeNameWithPrecisionAndScale"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""ChangeColumnTypeInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""ColumnName"" TEXT, ""OldColumnType"" TEXT, ""NewColumnType"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""ChangeFunctionDefinitionInstructions"" (""InstructionRef"" INTEGER, ""FunctionName"" TEXT, ""NewQuery"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""ChangeViewDefinitionInstructions"" (""InstructionRef"" INTEGER, ""ViewName"" TEXT, ""NewQuery"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""CreateFunctionInstructions"" (""InstructionRef"" INTEGER, ""FunctionName"" TEXT, ""Query"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""CreateTableInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""Query"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""CreateViewInstructions"" (""InstructionRef"" INTEGER, ""ViewName"" TEXT, ""Query"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""DropColumnInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""ColumnName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""DropFunctionInstructions"" (""InstructionRef"" INTEGER, ""FunctionName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""DropTableInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""DropViewInstructions"" (""InstructionRef"" INTEGER, ""ViewName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""ExecuteCustomCodeInstructions"" (""InstructionRef"" INTEGER, ""CodeKey"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""ExecuteCustomQueryInstructions"" (""InstructionRef"" INTEGER, ""Query"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""InstructionRegister"" (""Ref"" INTEGER, ""InstructionTypeRef"" INTEGER, ""LogicalOrder"" INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""LogicalOrder"" (""LogicalOrder"" INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RefMaster"" (""Ref"" INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RegenerateFunctionInstructions"" (""InstructionRef"" INTEGER, ""FunctionName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RegenerateViewInstructions"" (""InstructionRef"" INTEGER, ""ViewName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RenameColumnInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""OldColumnName"" TEXT, ""NewColumnName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RenameFunctionInstructions"" (""InstructionRef"" INTEGER, ""OldFunctionName"" TEXT, ""NewFunctionName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RenameTableInstructions"" (""InstructionRef"" INTEGER, ""OldTableName"" TEXT, ""NewTableName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""RenameViewInstructions"" (""InstructionRef"" INTEGER, ""OldViewName"" TEXT, ""NewViewName"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""SetPrimaryKeyOnTableInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""Columns"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ""SetIndexOnTableInstructions"" (""InstructionRef"" INTEGER, ""TableName"" TEXT, ""Columns"" TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("AddColumnToTableInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("ChangeColumnTypeInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("ChangeFunctionDefinitionInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("ChangeViewDefinitionInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("CreateFunctionInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("CreateTableInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("CreateViewInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("DropColumnInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("DropFunctionInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("DropTableInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("DropViewInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("ExecuteCustomCodeInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("ExecuteCustomQueryInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("RegenerateFunctionInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("RegenerateViewInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("RenameColumnInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("RenameFunctionInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("RenameTableInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("RenameViewInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("SetPrimaryKeyOnTableInstructions");
            cmd.ExecuteNonQuery();

            cmd.CommandText = FormulateViewCreationQuery("SetIndexOnTableInstructions");
            cmd.ExecuteNonQuery();

            using var dta = new SQLiteDataAdapter(cmd);

            {
                cmd.CommandText = "SELECT * FROM InstructionRegister";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("UniqueId"))
                {
                    cmd.CommandText = "ALTER TABLE InstructionRegister ADD UniqueId TEXT NOT NULL DEFAULT ''";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "SELECT * FROM InstructionRegister WHERE length(UniqueId) = 0 ORDER BY Ref";

                using var dtbCheckForBlankUniqueId = new DataTable();
                dta.Fill(dtbCheckForBlankUniqueId);

                foreach (DataRow dr in dtbCheckForBlankUniqueId.Rows)
                {
                    int instructionRef = GetInteger(dr["Ref"]);
                    string newUniqueId = Guid.NewGuid().ToString();

                    cmd.CommandText = $"UPDATE InstructionRegister SET UniqueId = '{newUniqueId}' WHERE Ref = {instructionRef}";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM AddColumnToTableInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE AddColumnToTableInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM ChangeColumnTypeInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE ChangeColumnTypeInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM ChangeFunctionDefinitionInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE ChangeFunctionDefinitionInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM ChangeViewDefinitionInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE ChangeViewDefinitionInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM DropColumnInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE DropColumnInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM RenameColumnInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE RenameColumnInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM RenameTableInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE RenameTableInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM RenameViewInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE RenameViewInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }

            {
                cmd.CommandText = "SELECT * FROM RenameFunctionInstructions";

                using var dtbCheck = new DataTable();
                dta.FillSchema(dtbCheck, SchemaType.Mapped);

                if (!dtbCheck.Columns.Contains("SkipDependentObjectReformulation"))
                {
                    cmd.CommandText = "ALTER TABLE RenameFunctionInstructions ADD SkipDependentObjectReformulation INTEGER NOT NULL DEFAULT 0";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessageDisplayer?.Invoke(ex.Message, "Error");
        }
        finally
        {
            cnn.Close();
        }
    }

    private static string FormulateViewCreationQuery(string tableName)
    {
        return $@"CREATE VIEW IF NOT EXISTS ""vw{tableName}"" AS SELECT InstructionRegister.*, {tableName}.* FROM InstructionRegister, {tableName} WHERE {tableName}.InstructionRef = InstructionRegister.Ref";
    }

    private static int GetAndAssignNextLogicalOrder(SQLiteCommand cmd)
    {
        //cmd.CommandText = "SELECT LogicalOrder FROM LogicalOrder";
        cmd.CommandText = "SELECT MAX(LogicalOrder) FROM InstructionRegister";
        int logicalOrder = GetInteger(cmd.ExecuteScalar());
        logicalOrder += 1;
        cmd.CommandText = "DELETE FROM LogicalOrder";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO LogicalOrder VALUES (" + Convert.ToString(logicalOrder) + ")";
        cmd.ExecuteNonQuery();
        return logicalOrder;
    }

    private static void SetNextLogicalOrderIfRequired(SQLiteCommand cmd, IDBCAInstruction instr)
    {
        //if (instr.Ref <= 0 || instr.LogicalOrder <= 0)
        if (instr.LogicalOrder <= 0)
            instr.LogicalOrder = GetAndAssignNextLogicalOrder(cmd);

        //if (instr.Ref <= 0)
        //{

        cmd.CommandText = $"SELECT LogicalOrder FROM InstructionRegister WHERE LogicalOrder = {instr.LogicalOrder}";
        if (GetInteger(cmd.ExecuteScalar()) > 0)
        {
            cmd.CommandText = $"UPDATE InstructionRegister SET LogicalOrder = LogicalOrder + 1 " +
            $"WHERE LogicalOrder >= {instr.LogicalOrder}";
            cmd.ExecuteNonQuery();
        }
        //}
    }

    private static int GetAndAssignNextRef(SQLiteCommand cmd)
    {
        cmd.CommandText = "SELECT Ref FROM RefMaster";
        int Ref = GetInteger(cmd.ExecuteScalar());
        Ref += 1;
        cmd.CommandText = "DELETE FROM RefMaster";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO RefMaster VALUES (" + Convert.ToString(Ref) + ")";
        cmd.ExecuteNonQuery();
        return Ref;
    }

    private static void SetNextRefIfRequired(SQLiteCommand cmd, IDBCAInstruction instr)
    {
        if (instr.Ref <= 0)
            instr.Ref = GetAndAssignNextRef(cmd);
    }

    private static void SetUniqueIdIfRequired(IDBCAInstruction instr)
    {
        if (instr.UniqueId.Trim().Length <= 0)
            instr.UniqueId = Guid.NewGuid().ToString();
    }

    private static void InsertInstructionRegisterEntry(SQLiteCommand cmd, InstructionRegisterEntry entry)
    {
        cmd.CommandText = "INSERT INTO InstructionRegister (Ref, InstructionTypeRef, LogicalOrder, UniqueId) VALUES (" + entry.Ref.ToString() + "," + entry.InstructionTypeRef.ToString() + "," + entry.LogicalOrder.ToString() + ", '" + entry.UniqueId + "')";
        cmd.ExecuteNonQuery();
    }

    private static IDBCAPersistenceColumnMapper CreatePersistenceColumnMapper(DataRow dr)
    {
        return new DataRowPersistenceColumnMapper(dr);
    }

    private static DataTable GetSchemaTable(SQLiteCommand cmd, string tableName)
    {
        cmd.CommandText = "SELECT * FROM " + tableName;
        using SQLiteDataAdapter dta = new(cmd);
        using DataTable dtb = new();
        dta.FillSchema(dtb, SchemaType.Mapped);
        return dtb;
    }

    private static void DeleteExistingInstruction(SQLiteCommand cmd, IDBCAInstruction instr, string instructionSpecificTableName)
    {
        cmd.CommandText = "DELETE FROM " + InstructionRegisterTableName + " WHERE Ref = " + instr.Ref.ToString();
        cmd.ExecuteNonQuery();
        cmd.CommandText = "DELETE FROM " + instructionSpecificTableName + " WHERE InstructionRef = " + instr.Ref.ToString();
        cmd.ExecuteNonQuery();
    }

    private static void InsertInstructionSpecificEntry(SQLiteCommand cmd, IDBCAInstruction instr, string instructionSpecificTableName)
    {
        using DataTable dtbSchema = GetSchemaTable(cmd, instructionSpecificTableName);
        var dr = dtbSchema.NewRow();
        DataRowPersistenceColumnMapper mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
        instr.SetPropertyValuesToPersistenceColumnMapper(mapper);
        mapper.GenerateInsertCommand(cmd, instructionSpecificTableName);
        cmd.ExecuteNonQuery();
    }

    private IDBCADataPersistenceResult<T> SaveInstruction<T>(T instr, string instructionSpecificTableName, ChangeInstructionTypes instructionType) where T : IDBCAInstruction
    {
        var result = new SQLitePersistenceResult<T>();
        cnn.Open();
        using (SQLiteTransaction trs = cnn.BeginTransaction())
        {
            try
            {
                using (var cmd = new SQLiteCommand(string.Empty, cnn, trs))
                {
                    DeleteExistingInstruction(cmd, instr, instructionSpecificTableName);
                    SetNextLogicalOrderIfRequired(cmd, instr);
                    SetNextRefIfRequired(cmd, instr);
                    SetUniqueIdIfRequired(instr);
                    InsertInstructionRegisterEntry(cmd, InstructionRegisterEntry.DeriveFromInstruction(instr, instructionType));
                    InsertInstructionSpecificEntry(cmd, instr, instructionSpecificTableName);
                }

                trs.Commit();
                result.Successful = true;
                result.Tag = instr;
            }
            catch (Exception ex)
            {
                trs.Rollback();
                result.Successful = false;
                result.ExceptionMessage = ex.Message;
            }
            finally
            {
                cnn.Close();
            }
        }

        return result;
    }

    public override IDBCADataPersistenceResult<string> DeleteInstruction(IDBCAInstruction instr)
    {
        var switchExpr = instr.InstructionTypeRef;
        switch (switchExpr)
        {
            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.AddColumnToTable):
                {
                    return DeleteInstructionInternal(instr, "AddColumnToTableInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ChangeColumnType):
                {
                    return DeleteInstructionInternal(instr, "ChangeColumnTypeInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ChangeFunctionDefinition):
                {
                    return DeleteInstructionInternal(instr, "ChangeFunctionDefinitionInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ChangeViewDefinition):
                {
                    return DeleteInstructionInternal(instr, "ChangeViewDefinitionInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.CreateFunction):
                {
                    return DeleteInstructionInternal(instr, "CreateFunctionInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.CreateTable):
                {
                    return DeleteInstructionInternal(instr, "CreateTableInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.CreateView):
                {
                    return DeleteInstructionInternal(instr, "CreateViewInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropColumn):
                {
                    return DeleteInstructionInternal(instr, "DropColumnInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropFunction):
                {
                    return DeleteInstructionInternal(instr, "DropFunctionInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropTable):
                {
                    return DeleteInstructionInternal(instr, "DropTableInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropView):
                {
                    return DeleteInstructionInternal(instr, "DropViewInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ExecuteCustomCode):
                {
                    return DeleteInstructionInternal(instr, "ExecuteCustomCodeInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ExecuteCustomQuery):
                {
                    return DeleteInstructionInternal(instr, "ExecuteCustomQueryInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RegenerateFunction):
                {
                    return DeleteInstructionInternal(instr, "RegenerateFunctionInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RegenerateView):
                {
                    return DeleteInstructionInternal(instr, "RegenerateViewInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameColumn):
                {
                    return DeleteInstructionInternal(instr, "RenameColumnInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameFunction):
                {
                    return DeleteInstructionInternal(instr, "RenameFunctionInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameTable):
                {
                    return DeleteInstructionInternal(instr, "RenameTableInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameView):
                {
                    return DeleteInstructionInternal(instr, "RenameViewInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.SetPrimaryKeyOnTable):
                {
                    return DeleteInstructionInternal(instr, "SetPrimaryKeyOnTableInstructions");
                }

            case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.SetIndexOnTable):
                {
                    return DeleteInstructionInternal(instr, "SetIndexOnTableInstructions");
                }
        }

        return new SQLitePersistenceResult<string> { Successful = false, ExceptionMessage = "Invalid Instruction Type" };
    }

    private IDBCADataPersistenceResult<string> DeleteInstructionInternal(IDBCAInstruction instr, string instructionSpecificTableName)
    {
        var result = new SQLitePersistenceResult<string>();

        cnn.Open();
        using (var trs = cnn.BeginTransaction())
        {
            try
            {
                using (var cmd = new SQLiteCommand(string.Empty, cnn, trs))
                {
                    DeleteExistingInstruction(cmd, instr, instructionSpecificTableName);
                }

                trs.Commit();
                result.Successful = true;
            }
            catch (Exception ex)
            {
                trs.Rollback();
                result.Successful = false;
                result.ExceptionMessage = ex.Message;
            }
            finally
            {
                cnn.Close();
            }
        }

        return result;
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveAddColumnToTableInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((AddColumnToTableInstruction)instr, "AddColumnToTableInstructions", ChangeInstructionTypes.AddColumnToTable);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateTableInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((CreateTableInstruction)instr, "CreateTableInstructions", ChangeInstructionTypes.CreateTable);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameTableInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((RenameTableInstruction)instr, "RenameTableInstructions", ChangeInstructionTypes.RenameTable);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveDropTableInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((DropTableInstruction)instr, "DropTableInstructions", ChangeInstructionTypes.DropTable);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameColumnInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((RenameColumnInstruction)instr, "RenameColumnInstructions", ChangeInstructionTypes.RenameColumn);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveDropColumnInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((DropColumnInstruction)instr, "DropColumnInstructions", ChangeInstructionTypes.DropColumn);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeColumnTypeInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((ChangeColumnTypeInstruction)instr, "ChangeColumnTypeInstructions", ChangeInstructionTypes.ChangeColumnType);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateViewInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((CreateViewInstruction)instr, "CreateViewInstructions", ChangeInstructionTypes.CreateView);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameViewInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((RenameViewInstruction)instr, "RenameViewInstructions", ChangeInstructionTypes.RenameView);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeViewDefinitionInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((ChangeViewDefinitionInstruction)instr, "ChangeViewDefinitionInstructions", ChangeInstructionTypes.ChangeViewDefinition);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveDropViewInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((DropViewInstruction)instr, "DropViewInstructions", ChangeInstructionTypes.DropView);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveRegenerateViewInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((RegenerateViewInstruction)instr, "RegenerateViewInstructions", ChangeInstructionTypes.RegenerateView);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveCreateFunctionInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((CreateFunctionInstruction)instr, "CreateFunctionInstructions", ChangeInstructionTypes.CreateFunction);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveRenameFunctionInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((RenameFunctionInstruction)instr, "RenameFunctionInstructions", ChangeInstructionTypes.RenameFunction);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveChangeFunctionDefinitionInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((ChangeFunctionDefinitionInstruction)instr, "ChangeFunctionDefinitionInstructions", ChangeInstructionTypes.ChangeFunctionDefinition);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveDropFunctionInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((DropFunctionInstruction)instr, "DropFunctionInstructions", ChangeInstructionTypes.DropFunction);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveRegenerateFunctionInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((RegenerateFunctionInstruction)instr, "RegenerateFunctionInstructions", ChangeInstructionTypes.RegenerateFunction);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveExecuteCustomQueryInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((ExecuteCustomQueryInstruction)instr, "ExecuteCustomQueryInstructions", ChangeInstructionTypes.ExecuteCustomQuery);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveExecuteCustomCodeInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((ExecuteCustomCodeInstruction)instr, "ExecuteCustomCodeInstructions", ChangeInstructionTypes.ExecuteCustomCode);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveSetPrimaryKeyOnTableInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((SetPrimaryKeyOnTableInstruction)instr, "SetPrimaryKeyOnTableInstructions", ChangeInstructionTypes.SetPrimaryKeyOnTable);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCADataPersistenceResult<IDBCAInstruction> SaveSetIndexOnTableInstruction(IDBCAInstruction instr)
    {
        var rInternal = SaveInstruction((SetIndexOnTableInstruction)instr, "SetIndexOnTableInstructions", ChangeInstructionTypes.SetIndexOnTable);
        return new SQLitePersistenceResult<IDBCAInstruction>(rInternal.Successful, rInternal.ExceptionMessage, rInternal.Tag);
    }

    public override IDBCAInstruction GetCreateTableInstruction(string tableName)
    {
        IDBCAInstruction result = null;

        cnn.Open();

        try
        {
            DataRowPersistenceColumnMapper mapper = null;

            CreateTableInstruction instrCreateTable = null;

            using SQLiteCommand cmd = new(string.Empty, cnn);
            using SQLiteDataAdapter dta = new(cmd);
            string fString = $"TableName = '{tableName}'";
            cmd.CommandText = "SELECT * FROM vwCreateTableInstructions WHERE " + fString;
            using DataTable dtb = new();
            dta.Fill(dtb);
            foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
            {
                mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                instrCreateTable = new CreateTableInstruction(this);
                instrCreateTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                result = instrCreateTable;
            }
        }
        finally
        {
            cnn.Close();
        }

        return result;
    }

    public override IDBCAInstruction GetSetPrimaryKeyOnTableInstruction(string tableName, string columns)
    {
        IDBCAInstruction result = null;

        cnn.Open();

        try
        {
            DataRowPersistenceColumnMapper mapper = null;

            SetPrimaryKeyOnTableInstruction instrSetPrimaryKeyOnTable = null;

            using SQLiteCommand cmd = new(string.Empty, cnn);
            using SQLiteDataAdapter dta = new(cmd);
            string fString = $"TableName = '{tableName}' AND Columns = '{columns}'";
            cmd.CommandText = "SELECT * FROM vwSetPrimaryKeyOnTableInstructions WHERE " + fString;
            using DataTable dtb = new();
            dta.Fill(dtb);
            foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
            {
                mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                instrSetPrimaryKeyOnTable = new SetPrimaryKeyOnTableInstruction(this);
                instrSetPrimaryKeyOnTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                result = instrSetPrimaryKeyOnTable;
            }
        }
        finally
        {
            cnn.Close();
        }

        return result;
    }


    public override IDBCAInstruction GetSetIndexOnTableInstruction(string tableName, string columns)
    {
        IDBCAInstruction result = null;

        cnn.Open();

        try
        {
            DataRowPersistenceColumnMapper mapper = null;

            SetIndexOnTableInstruction instrSetIndexOnTable = null;

            using SQLiteCommand cmd = new SQLiteCommand(string.Empty, cnn);
            using SQLiteDataAdapter dta = new SQLiteDataAdapter(cmd);
            string fString = $"TableName = '{tableName}' AND Columns = '{columns}'";
            cmd.CommandText = "SELECT * FROM vwSetIndexOnTableInstructions WHERE " + fString;
            using DataTable dtb = new DataTable();
            dta.Fill(dtb);
            foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
            {
                mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                instrSetIndexOnTable = new SetIndexOnTableInstruction(this);
                instrSetIndexOnTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                result = instrSetIndexOnTable;
            }
        }
        finally
        {
            cnn.Close();
        }

        return result;
    }

    public override IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionList()
    {
        return GetInstructionListInternal(string.Empty);
    }

    public override IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListByLogicalOrderOnwards(int fromLogicalOrderOnwards)
    {
        return GetInstructionListInternal("LogicalOrder >= " + fromLogicalOrderOnwards.ToString());
    }

    public override IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListByLogicalOrderRange(int fromLogicalOrder, int toLogicalOrder)
    {
        return GetInstructionListInternal("LogicalOrder >= " + fromLogicalOrder.ToString() + " AND LogicalOrder <= " + toLogicalOrder.ToString());
    }

    private IDBCADataQueryResult<List<IDBCAInstruction>> GetInstructionListInternal(string filterString)
    {
        var result = new SQLiteQueryResult<List<IDBCAInstruction>>();
        string fString = string.Empty;
        if (filterString.Length > 0)
            fString = " WHERE " + filterString;
        cnn.Open();
        try
        {
            result.Tag = new List<IDBCAInstruction>();
            DataRowPersistenceColumnMapper mapper = null;

            CreateTableInstruction instrCreateTable = null;
            RenameTableInstruction instrRenameTable = null;
            DropTableInstruction instrDropTable = null;
            AddColumnToTableInstruction instrAddColumn = null;
            RenameColumnInstruction instrRenameColumn = null;
            DropColumnInstruction instrDropColumn = null;
            ChangeColumnTypeInstruction instrChangeColumnType = null;
            CreateViewInstruction instrCreateView = null;
            RenameViewInstruction instrRenameView = null;
            ChangeViewDefinitionInstruction instrChangeViewDefinition = null;
            DropViewInstruction instrDropView = null;
            RegenerateViewInstruction instrRegenerateView = null;
            CreateFunctionInstruction instrCreateFunction = null;
            RenameFunctionInstruction instrRenameFunction = null;
            ChangeFunctionDefinitionInstruction instrChangeFunctionDefinition = null;
            DropFunctionInstruction instrDropFunction = null;
            RegenerateFunctionInstruction instrRegenerateFunction = null;
            ExecuteCustomQueryInstruction instrExecuteCustomQuery = null;
            ExecuteCustomCodeInstruction instrExecuteCustomCode = null;
            SetPrimaryKeyOnTableInstruction instrSetPrimaryKeyOnTable = null;
            SetIndexOnTableInstruction instrSetIndexOnTable = null;

            using (SQLiteCommand cmd = new SQLiteCommand(string.Empty, cnn))
            {
                using var dta = new SQLiteDataAdapter(cmd);
                cmd.CommandText = "SELECT * FROM vwCreateTableInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrCreateTable = new CreateTableInstruction(this);
                        instrCreateTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrCreateTable);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwRenameTableInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrRenameTable = new RenameTableInstruction(this);
                        instrRenameTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrRenameTable);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwDropTableInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrDropTable = new DropTableInstruction(this);
                        instrDropTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrDropTable);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwAddColumnToTableInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrAddColumn = new AddColumnToTableInstruction(this);
                        instrAddColumn.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrAddColumn);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwRenameColumnInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrRenameColumn = new RenameColumnInstruction(this);
                        instrRenameColumn.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrRenameColumn);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwDropColumnInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrDropColumn = new DropColumnInstruction(this);
                        instrDropColumn.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrDropColumn);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwChangeColumnTypeInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrChangeColumnType = new ChangeColumnTypeInstruction(this);
                        instrChangeColumnType.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrChangeColumnType);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwCreateViewInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrCreateView = new CreateViewInstruction(this);
                        instrCreateView.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrCreateView);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwRenameViewInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrRenameView = new RenameViewInstruction(this);
                        instrRenameView.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrRenameView);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwChangeViewDefinitionInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrChangeViewDefinition = new ChangeViewDefinitionInstruction(this);
                        instrChangeViewDefinition.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrChangeViewDefinition);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwDropViewInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrDropView = new DropViewInstruction(this);
                        instrDropView.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrDropView);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwRegenerateViewInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrRegenerateView = new RegenerateViewInstruction(this);
                        instrRegenerateView.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrRegenerateView);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwCreateFunctionInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrCreateFunction = new CreateFunctionInstruction(this);
                        instrCreateFunction.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrCreateFunction);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwRenameFunctionInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrRenameFunction = new RenameFunctionInstruction(this);
                        instrRenameFunction.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrRenameFunction);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwChangeFunctionDefinitionInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrChangeFunctionDefinition = new ChangeFunctionDefinitionInstruction(this);
                        instrChangeFunctionDefinition.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrChangeFunctionDefinition);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwDropFunctionInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrDropFunction = new DropFunctionInstruction(this);
                        instrDropFunction.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrDropFunction);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwRegenerateFunctionInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrRegenerateFunction = new RegenerateFunctionInstruction(this);
                        instrRegenerateFunction.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrRegenerateFunction);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwExecuteCustomQueryInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrExecuteCustomQuery = new ExecuteCustomQueryInstruction(this);
                        instrExecuteCustomQuery.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrExecuteCustomQuery);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwExecuteCustomCodeInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrExecuteCustomCode = new ExecuteCustomCodeInstruction(this);
                        instrExecuteCustomCode.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrExecuteCustomCode);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwSetPrimaryKeyOnTableInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrSetPrimaryKeyOnTable = new SetPrimaryKeyOnTableInstruction(this);
                        instrSetPrimaryKeyOnTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrSetPrimaryKeyOnTable);
                    }
                }

                cmd.CommandText = "SELECT * FROM vwSetIndexOnTableInstructions" + fString;
                using (var dtb = new DataTable())
                {
                    dta.Fill(dtb);
                    foreach (DataRow dr in dtb.Select(string.Empty, "LogicalOrder"))
                    {
                        mapper = (DataRowPersistenceColumnMapper)CreatePersistenceColumnMapper(dr);
                        instrSetIndexOnTable = new SetIndexOnTableInstruction(this);
                        instrSetIndexOnTable.GetPropertyValuesFromPersistenceColumnMapper(mapper);
                        result.Tag.Add(instrSetIndexOnTable);
                    }
                }
            }

            result.Successful = true;
            var comp = new Comparison<IDBCAInstruction>((i1, i2) => Convert.ToInt32(i1.LogicalOrder < i2.LogicalOrder ? -1 : Convert.ToInt32(i1.LogicalOrder > i2.LogicalOrder ? 2 : 0)));
            result.Tag.Sort(comp);
        }
        catch (Exception ex)
        {
            result.Successful = false;
            result.Tag = null;
            result.ExceptionMessage = ex.Message;
        }
        finally
        {
            cnn.Close();
        }

        return result;
    }

    private static int GetInteger(object input)
    {
        if (input == null || input == DBNull.Value || !int.TryParse(input.ToString(), out _)) return 0;
        return Convert.ToInt32(input);
    }
}
