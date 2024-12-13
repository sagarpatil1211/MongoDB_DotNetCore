using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Reflection;
using VJCore.Infrastructure.Extensions;
using System.Data.Common;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;
//using VJ1Core.Infrastructure.Config;
//using VJ1Core.Infrastructure.DatabaseMigration.Instructions;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class SqlServerDatabaseChangeInstructionManager
{
    private readonly string strConnection = string.Empty;
    private readonly Action<string> processStartingHandler = null;
    private readonly Action<int, string> progressReportingMethod = null;
    private readonly Action<string> processCompletedHandler = null;
    private readonly Action<string> processExceptionHandler = null;
    private const int NonDeterminatePercentage = -1;
    private const int ExceptionPercentage = -2;
    private readonly IDBMigrationCustomCodeManager customCodeManager = null;
    private readonly IDBCADataAccessService instructionDataAccessService = null;
    private IDBCAInstruction exceptionContext = null;

    private static DBConnectionConfig DataConnectionConfig => DBConnectionConfig.ReadFromFileOrCreateNew();
    private readonly DbProviderFactory factory = DbProviderFactories.GetFactory(DataConnectionConfig.DatabaseType);

    public SqlServerDatabaseChangeInstructionManager(string strConnection, IDBCADataAccessService instructionDataAccessService, Action<string> processStartingHandler, Action<int, string> progressReportingMethod, Action<string> processCompletedHandler, Action<string> processExceptionHandler)
    {
        this.strConnection = strConnection;
        this.instructionDataAccessService = instructionDataAccessService;
        this.processStartingHandler = processStartingHandler;
        this.progressReportingMethod = progressReportingMethod;
        this.processCompletedHandler = processCompletedHandler;
        this.processExceptionHandler = processExceptionHandler;

        var assemblies = AssemblyManagement.GetAllAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                var ifaces = t.GetInterfaces();

                if (ifaces.Any(iface => iface.Name.Contains("IDBMigrationCustomCodeManager")))
                {
                    customCodeManager = (IDBMigrationCustomCodeManager)Activator.CreateInstance(t);
                }
            }
        }
    }

    public void ExecuteChanges(bool async = true)
    {
        if (async == true)
        {
            var bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            bw.ProgressChanged += bw_ProgressChanged;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync(strConnection);
        }
        else
        {
            ExecuteChangesSync(strConnection);
        }
    }

    private void bw_DoWork(object sender, DoWorkEventArgs e)
    {
        ExecuteChangesAsync((BackgroundWorker)sender, e.Argument.ToString());
    }

    private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        var switchExpr = e.ProgressPercentage;
        switch (switchExpr)
        {
            case object _ when switchExpr == 0:
                {
                    processStartingHandler.Invoke(e.UserState.ToString());
                    break;
                }

            case object _ when switchExpr == 100:
                {
                    processCompletedHandler.Invoke(e.UserState.ToString());
                    break;
                }

            case object _ when switchExpr == ExceptionPercentage:
                {
                    processExceptionHandler.Invoke(e.UserState.ToString());
                    break;
                }

            case object _ when switchExpr == NonDeterminatePercentage:
                {
                    progressReportingMethod.Invoke(e.ProgressPercentage, e.UserState.ToString());
                    break;
                }
        }
    }

    private void ExecuteChangesAsync(BackgroundWorker worker, string strConn)
    {
        using var cnn = new SqlConnection(strConn);
        try
        {
            cnn.Open();

            try
            {
                using var cmd = new SqlCommand(string.Empty, cnn);
                ExecuteDbChanges(worker, cnn);
            }

            catch
            {
                throw;
            }
            finally
            {
                cnn.Close();
            }
        }

        catch (Exception ex)
        {
            string exceptionString = "Error : " + ex.Message;
            if (exceptionContext is object)
                exceptionString = "Error at Logical Order " + exceptionContext.LogicalOrder.ToString() + " : " + ex.Message;
            worker.ReportProgress(ExceptionPercentage, exceptionString);
        }
    }

    private void ExecuteChangesSync(string strConn)
    {
        using var cnn = new SqlConnection(strConn);
        try
        {
            cnn.Open();

            try
            {
                using var cmd = new SqlCommand(string.Empty, cnn);
                ExecuteDbChanges(null, cnn);
            }

            catch
            {
                throw;
            }
            finally
            {
                cnn.Close();
            }
        }

        catch (Exception ex)
        {
            string exceptionString = "Error : " + ex.Message;
            if (exceptionContext is object)
                exceptionString = "Error at Logical Order " + exceptionContext.LogicalOrder.ToString() + " : " + ex.Message;
            throw new Exception(exceptionString);
        }
    }

    private void ExecuteDbChanges(BackgroundWorker worker, DbConnection cnn)
    {
        using var cmd = factory.CreateCommand();
        cmd.Connection = cnn;

        using var dta = DatabaseMigrationManager.CreateDataAdapter(cmd);
        var comp = new ProcessUtilityComponents(cnn, cmd, dta, worker);
        cmd.CommandTimeout = 0;
        comp.ReportProgress(0, "Changes Started ...");
        Thread.Sleep(1000);
        int lastLogicalOrder = 0;

        if (DoesStoredProcedureExist("DropAllColumnConstraints", comp) == false)
        {
            CreateStoredProcedure(comp, "DropAllColumnConstraints",
                $"CREATE PROCEDURE DropAllColumnConstraints @tableName varchar(128), @columnName varchar(128)" +
                $" AS set nocount on set xact_abort on" +
                $" while 0=0" +
                $" begin" +
                $" declare @constraintName varchar(128)" +
                $" set @constraintName = (select top 1 constraint_name from information_schema.constraint_column_usage where table_name = @tableName and column_name = @columnName)" +
                $" if @constraintName is null break exec ('alter table '+@tableName+' drop constraint '+@constraintName+'') end");
        }

        if (DoesTableExist("DBCLLO", comp) == false)
        {
            CreateTable(comp, "DBCLLO", "CREATE TABLE DBCLLO (LLO int NOT NULL)");
        }
        else
        {
            cmd.CommandText = "SELECT LLO FROM DBCLLO";
            var objLLO = cmd.ExecuteScalar();
            lastLogicalOrder = Conversions.ToInteger(Interaction.IIf(Information.IsDBNull(objLLO), 0, objLLO));
        }

        bool uidTableCreatedForTheFirstTime = false;

        if (DoesTableExist("DBCUIDEXEC", comp) == false)
        {
            CreateTable(comp, "DBCUIDEXEC", "CREATE TABLE DBCUIDEXEC (UID varchar(256) NOT NULL PRIMARY KEY)");
            uidTableCreatedForTheFirstTime = true;
        }
        else
        {
            cmd.CommandText = "SELECT COUNT(UID) FROM DBCUIDEXEC";
            var objCount = cmd.ExecuteScalar();
            int uidCound = Conversions.ToInteger(Interaction.IIf(Information.IsDBNull(objCount), 0, objCount));
            uidTableCreatedForTheFirstTime = uidCound == 0;
        }

        if (DoesTableExist("ViewDefinitions", comp) == false)
        {
            CreateTable(comp, "ViewDefinitions", "CREATE TABLE ViewDefinitions (ViewName varchar(256) NOT NULL, InstructionLogicalOrder int NOT NULL, DefinitionLogicalOrder int NOT NULL, Definition varchar(4096) NOT NULL)");
        }

        if (DoesTableExist("ViewDefinitions2", comp) == false)
        {
            CreateTable(comp, "ViewDefinitions2", "CREATE TABLE ViewDefinitions2 (ViewName varchar(256) NOT NULL, InstructionLogicalOrder int NOT NULL, Definition varchar(MAX) NOT NULL)");
        }

        if (DoesTableExist("FunctionDefinitions", comp) == false)
        {
            CreateTable(comp, "FunctionDefinitions", "CREATE TABLE FunctionDefinitions (FunctionName varchar(256) NOT NULL, InstructionLogicalOrder int NOT NULL, DefinitionLogicalOrder int NOT NULL, Definition varchar(4096) NOT NULL)");
        }

        if (DoesTableExist("FunctionDefinitions2", comp) == false)
        {
            CreateTable(comp, "FunctionDefinitions2", "CREATE TABLE FunctionDefinitions2 (FunctionName varchar(256) NOT NULL, InstructionLogicalOrder int NOT NULL, Definition varchar(MAX) NOT NULL)");
        }

        if (DoesTableExist("TablePrimaryKeys", comp) == false)
        {
            CreateTable(comp, "TablePrimaryKeys", "CREATE TABLE TablePrimaryKeys (TableName varchar(1024) NOT NULL, PrimaryKeyName varchar(256) NOT NULL, InstructionLogicalOrder int NOT NULL, Columns varchar(4096) NOT NULL)");
        }

        if (DoesTableExist("TableIndexNumbers", comp) == false)
        {
            CreateTable(comp, "TableIndexNumbers", "CREATE TABLE TableIndexNumbers (TableName varchar(1024) NOT NULL, IndexNo int NOT NULL)");
        }

        if (DoesTableExist("TableIndices", comp) == false)
        {
            CreateTable(comp, "TableIndices", "CREATE TABLE TableIndices (TableName varchar(1024) NOT NULL, IndexName varchar(256) NOT NULL, InstructionLogicalOrder int NOT NULL, Columns varchar(4096) NOT NULL)");
        }

        {
            if (uidTableCreatedForTheFirstTime)
            {
                var instructionsUptoLLOResult = instructionDataAccessService.GetInstructionListByLogicalOrderRange(0, lastLogicalOrder);
                if (instructionsUptoLLOResult.Successful == false)
                    throw new Exception(instructionsUptoLLOResult.ExceptionMessage);

                var instructionsUptoLLO = instructionsUptoLLOResult.Tag;

                using var trs = cnn.BeginTransaction();
                cmd.Transaction = trs;

                try
                {
                    foreach (IDBCAInstruction instr in instructionsUptoLLO)
                    {
                        exceptionContext = instr;

                        cmd.Parameters.Clear();
                        cmd.AddVarcharParameter("@UID", 256, instr.UniqueId);
                        cmd.CommandText = "DELETE FROM DBCUIDEXEC WHERE UID = @UID";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO DBCUIDEXEC VALUES (@UID)";
                        cmd.ExecuteNonQuery();
                    }

                    trs.Commit();
                }
                catch
                {
                    trs.Rollback();
                    throw;
                }
                finally
                {
                    cmd.Transaction = null;
                }
            }
        }

        var result = instructionDataAccessService.GetInstructionListByLogicalOrderOnwards(lastLogicalOrder + 1);
        if (result.Successful == false)
            throw new Exception(result.ExceptionMessage);
        var lstInstructions = result.Tag;
        int totalCount = lstInstructions.Count;

        foreach (IDBCAInstruction instr in lstInstructions)
        {
            exceptionContext = instr;
            using var trs = cnn.BeginTransaction();
            cmd.Transaction = trs;
            try
            {
                var switchExpr = instr.InstructionTypeRef;
                switch (switchExpr)
                {
                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.AddColumnToTable):
                        {
                            AddColumnToTableInstruction spInst = (AddColumnToTableInstruction)instr;
                            AddColumnToTable(comp, spInst.TableName, spInst.ColumnName,
                                spInst.TypeNameWithPrecisionAndScale,
                                spInst.SkipDependentObjectReformulation,
                                instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ChangeColumnType):
                        {
                            ChangeColumnTypeInstruction spInst = (ChangeColumnTypeInstruction)instr;
                            ChangeColumnTypeInTable(comp, spInst.TableName,
                                spInst.ColumnName, spInst.NewColumnType, spInst.SkipDependentObjectReformulation,
                                instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ChangeFunctionDefinition):
                        {
                            ChangeFunctionDefinitionInstruction spInst = (ChangeFunctionDefinitionInstruction)instr;
                            ChangeFunctionDefinition(comp, spInst.FunctionName,
                                spInst.NewQuery, spInst.SkipDependentObjectReformulation, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ChangeViewDefinition):
                        {
                            ChangeViewDefinitionInstruction spInst = (ChangeViewDefinitionInstruction)instr;
                            ChangeViewDefinition(comp, spInst.ViewName, spInst.NewQuery,
                                spInst.SkipDependentObjectReformulation, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.CreateFunction):
                        {
                            CreateFunctionInstruction spInst = (CreateFunctionInstruction)instr;
                            CreateFunction(comp, spInst.FunctionName, spInst.Query, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.CreateTable):
                        {
                            CreateTableInstruction spInst = (CreateTableInstruction)instr;
                            CreateTable(comp, spInst.TableName, spInst.Query);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.CreateView):
                        {
                            CreateViewInstruction spInst = (CreateViewInstruction)instr;
                            CreateView(comp, spInst.ViewName, spInst.Query, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropColumn):
                        {
                            DropColumnInstruction spInst = (DropColumnInstruction)instr;
                            DropColumnFromTable(comp, spInst.TableName, spInst.ColumnName,
                                spInst.SkipDependentObjectReformulation, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropFunction):
                        {
                            DropFunctionInstruction spInst = (DropFunctionInstruction)instr;
                            DropFunction(comp, spInst.FunctionName);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropTable):
                        {
                            DropTableInstruction spInst = (DropTableInstruction)instr;
                            DropTable(comp, spInst.TableName);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.DropView):
                        {
                            DropViewInstruction spInst = (DropViewInstruction)instr;
                            DropView(comp, spInst.ViewName);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ExecuteCustomCode):
                        {
                            ExecuteCustomCodeInstruction spInst = (ExecuteCustomCodeInstruction)instr;
                            ExecuteCustomCode(comp, spInst.CodeKey);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.ExecuteCustomQuery):
                        {
                            ExecuteCustomQueryInstruction spInst = (ExecuteCustomQueryInstruction)instr;
                            ExecuteCustomQuery(comp, spInst.Query);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RegenerateFunction):
                        {
                            RegenerateFunctionInstruction spInst = (RegenerateFunctionInstruction)instr;
                            RegenerateFunction(comp, spInst.FunctionName, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RegenerateView):
                        {
                            RegenerateViewInstruction spInst = (RegenerateViewInstruction)instr;
                            RegenerateView(comp, spInst.ViewName, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameColumn):
                        {
                            RenameColumnInstruction spInst = (RenameColumnInstruction)instr;
                            RenameColumnInTable(comp, spInst.TableName, spInst.OldColumnName,
                                spInst.NewColumnName, spInst.SkipDependentObjectReformulation,
                                instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameFunction):
                        {
                            RenameFunctionInstruction spInst = (RenameFunctionInstruction)instr;
                            RenameFunction(comp, spInst.OldFunctionName, spInst.NewFunctionName,
                                spInst.SkipDependentObjectReformulation, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameTable):
                        {
                            RenameTableInstruction spInst = (RenameTableInstruction)instr;
                            RenameTable(comp, spInst.OldTableName, spInst.NewTableName,
                                spInst.SkipDependentObjectReformulation, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.RenameView):
                        {
                            RenameViewInstruction spInst = (RenameViewInstruction)instr;
                            RenameView(comp, spInst.OldViewName, spInst.NewViewName,
                                spInst.SkipDependentObjectReformulation, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.SetPrimaryKeyOnTable):
                        {
                            SetPrimaryKeyOnTableInstruction spInst = (SetPrimaryKeyOnTableInstruction)instr;
                            SetPrimaryKeyOnTable(comp, spInst.TableName, spInst.Columns, instr.LogicalOrder);
                            break;
                        }

                    case object _ when switchExpr == Conversions.ToInteger(ChangeInstructionTypes.SetIndexOnTable):
                        {
                            SetIndexOnTableInstruction spInst = (SetIndexOnTableInstruction)instr;
                            SetIndexOnTable(comp, spInst.TableName, spInst.Columns, instr.LogicalOrder);
                            break;
                        }
                }

                cmd.CommandText = "DELETE FROM DBCLLO";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO DBCLLO VALUES (@LogicalOrder)";
                cmd.Parameters.Clear();
                cmd.AddIntegerParameter("@LogicalOrder", instr.LogicalOrder);
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.AddVarcharParameter("@UID", 256, instr.UniqueId);
                cmd.CommandText = "DELETE FROM DBCUIDEXEC WHERE UID = @UID";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO DBCUIDEXEC VALUES (@UID)";
                cmd.ExecuteNonQuery();

                trs.Commit();
            }
            catch
            {
                trs.Rollback();
                throw;
            }
            finally
            {
                cmd.Transaction = null;
            }
        }

        comp.ReportProgress(100, "Changes Completed!");
    }

    private static Dictionary<string, string> GetDependentObjectNames(ProcessUtilityComponents comp, string objectName)
    {
        var result = new Dictionary<string, string>();

        var strCommand = $"WITH DepTree (referenced_id, referenced_name, referencing_id, referencing_name, NestLevel)" +
            $" AS (SELECT o.[object_id] AS referenced_id, o.name AS referenced_name, o.[object_id] AS referencing_id," +
            $" o.name AS referencing_name, 0 AS NestLevel" +
            $" FROM sys.objects o" +
            $" WHERE o.name = '{objectName}'" +
            $" UNION ALL SELECT d1.referenced_id, OBJECT_NAME(d1.referenced_id), d1.referencing_id," +
            $" OBJECT_NAME(d1.referencing_id), NestLevel + 1" +
            $" FROM sys.sql_expression_dependencies d1" +
            $" JOIN DepTree r ON d1.referenced_id =  r.referencing_id)" +
            $" SELECT DISTINCT referenced_id, referenced_name, referencing_id, referencing_name, NestLevel" +
            $" FROM DepTree WHERE NestLevel > 0 AND referenced_name = '{objectName}'" +
            $" ORDER BY NestLevel, referencing_id";

        DbParameterContainer parameters = new DbParameterContainer();
        var dtb = comp.FetchDataTable(strCommand, parameters);
        foreach (DataRow dr in dtb.Rows)
        {
            var dependentObjectName = GetString(dr["referencing_name"]).Trim();
            parameters = new DbParameterContainer();
            var dependentObjectType = comp.ExecuteScalar<string>($"SELECT xType FROM sysobjects" +
                $" WHERE name = '{dependentObjectName}'", parameters).Trim();

            result.Add(dependentObjectName, dependentObjectType);
        }

        return result;
    }

    private void ReformulateAndSaveDependentObjectDefinitions(ProcessUtilityComponents comp, string objectName, int instructionLogicalOrder)
    {
        var deps = GetDependentObjectNames(comp, objectName);

        foreach (var kvp in deps)
        {
            var dependentObjectName = kvp.Key;
            var dependentObjectType = kvp.Value;

            if (dependentObjectType == "V")
            {
                RegenerateView(comp, dependentObjectName, instructionLogicalOrder);
            }
            else if (dependentObjectType == "TF")
            {
                RegenerateFunction(comp, dependentObjectName, instructionLogicalOrder);
            }
        }
    }

    private static string GetString(object input)
    {
        if (input == null || input == DBNull.Value) return string.Empty;
        return input.ToString();
    }

    private static int GetInt32(object input)
    {
        if (input == null || input == DBNull.Value || !long.TryParse(input.ToString(), out _)) return 0;
        return Convert.ToInt32(input);
    }

    private static long GetInt64(object input)
    {
        if (input == null || input == DBNull.Value || !long.TryParse(input.ToString(), out _)) return 0L;
        return Convert.ToInt64(input);
    }

    private static string GetViewDefinition(ProcessUtilityComponents comp, string viewName, int instructionLogicalOrder)
    {
        DbParameterContainer parameters = new DbParameterContainer();
        parameters.AddVarchar("@ViewName", 256, viewName);
        parameters.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);

        var maxLogicalOrder = comp.ExecuteScalar<int>($"SELECT MAX(InstructionLogicalOrder)" +
            $" FROM ViewDefinitions2" +
            $" WHERE ViewName = @ViewName" +
            $" AND InstructionLogicalOrder < @InstructionLogicalOrder", parameters);

        parameters = new DbParameterContainer();
        parameters.AddVarchar("@ViewName", 256, viewName);
        parameters.AddInteger("@InstructionLogicalOrder", maxLogicalOrder);

        var dtb = comp.FetchDataTable($"SELECT * FROM ViewDefinitions2" +
            $" WHERE ViewName = @ViewName" +
            $" AND InstructionLogicalOrder = @InstructionLogicalOrder", parameters);

        foreach (DataRow dr in dtb.Rows)
        {
            return GetString(dr["Definition"]).Trim();
        }

        return string.Empty;
    }

    public static string GetViewDefinition(DbCommand cmd, string viewName, int instructionLogicalOrder)
    {
        cmd.CommandText = $"SELECT MAX(InstructionLogicalOrder) FROM ViewDefinitions2" +
            $" WHERE ViewName = @ViewName" +
            $" AND InstructionLogicalOrder < @InstructionLogicalOrder";
        cmd.Parameters.Clear();
        cmd.AddVarcharParameter("@ViewName", 256, viewName);
        cmd.AddIntegerParameter("@InstructionLogicalOrder", instructionLogicalOrder);
        var maxLogicalOrder = GetInt32(cmd.ExecuteScalar());

        using DbDataAdapter dta = DatabaseMigrationManager.CreateDataAdapter(cmd);
        using DataTable dtb = new DataTable();

        cmd.CommandText = $"SELECT * FROM ViewDefinitions2" +
            $" WHERE ViewName = @ViewName" +
            $" AND InstructionLogicalOrder = @MaxLogicalOrder";
        cmd.Parameters.Clear();
        cmd.AddVarcharParameter("@ViewName", 256, viewName);
        cmd.AddIntegerParameter("@MaxLogicalOrder", maxLogicalOrder);

        dta.Fill(dtb);

        foreach (DataRow dr in dtb.Rows)
        {
            return GetString(dr["Definition"]).ToString();
        }

        return string.Empty;
    }

    private static void SaveViewDefinition(ProcessUtilityComponents comp, string viewName, string definition, int instructionLogicalOrder)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@ViewName", viewName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        comp.ExecuteNonQuery($"DELETE FROM ViewDefinitions2 WHERE ViewName = @ViewName" +
            $" AND InstructionLogicalOrder = @InstructionLogicalOrder", paramValues);

        paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@ViewName", viewName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        paramValues.AddVarcharMax("@Definition", definition);

        comp.ExecuteNonQuery($"INSERT INTO ViewDefinitions2 (ViewName, InstructionLogicalOrder, Definition) " +
            $"VALUES (@ViewName, @InstructionLogicalOrder, @Definition)", paramValues);
    }

    public static void SaveViewDefinition(DbCommand cmd, string viewName, string definition, int instructionLogicalOrder)
    {
        cmd.CommandText = $"DELETE FROM ViewDefinitions2 WHERE ViewName = @ViewName AND InstructionLogicalOrder = @InstructionLogicalOrder";
        cmd.Parameters.Clear();
        cmd.AddVarcharMaxParameter("@ViewName", viewName);
        cmd.AddIntegerParameter("@InstructionLogicalOrder", instructionLogicalOrder);
        cmd.ExecuteNonQuery();

        cmd.CommandText = $"INSERT INTO ViewDefinitions2 (ViewName, InstructionLogicalOrder, Definition) " +
                $"VALUES (@ViewName, @InstructionLogicalOrder, @Definition)";

        cmd.Parameters.Clear();
        cmd.AddVarcharMaxParameter("@ViewName", viewName);
        cmd.AddIntegerParameter("@InstructionLogicalOrder", instructionLogicalOrder);
        cmd.AddVarcharMaxParameter("@Definition", definition);

        cmd.ExecuteNonQuery();
    }

    private static void SaveTablePrimaryKeyEntry(ProcessUtilityComponents comp, string tableName, string primaryKeyName, string columns, int instructionLogicalOrder)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@TableName", tableName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        comp.ExecuteNonQuery($"DELETE FROM TablePrimaryKeys WHERE TableName = @TableName" +
            $" AND InstructionLogicalOrder = @InstructionLogicalOrder", paramValues);

        paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@TableName", tableName);
        paramValues.AddVarcharMax("@PrimaryKeyName", primaryKeyName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        paramValues.AddVarcharMax("@Columns", columns);

        comp.ExecuteNonQuery($"INSERT INTO TablePrimaryKeys (TableName, PrimaryKeyName, InstructionLogicalOrder, Columns) " +
                $"VALUES (@TableName, @PrimaryKeyName, @InstructionLogicalOrder, @Columns)", paramValues);
    }

    private static void SaveTableIndexEntry(ProcessUtilityComponents comp, string tableName, string indexName, int indexNo, string columns, int instructionLogicalOrder)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@TableName", tableName);
        comp.ExecuteNonQuery($"DELETE FROM TableIndexNumbers WHERE TableName = @TableName", paramValues);

        paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@TableName", tableName);
        paramValues.AddInteger("@IndexNo", indexNo);
        comp.ExecuteNonQuery($"INSERT INTO TableIndexNumbers (TableName, IndexNo) VALUES (@TableName, @IndexNo)", paramValues);

        paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@TableName", tableName);
        paramValues.AddVarcharMax("@IndexName", indexName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        paramValues.AddVarcharMax("@Columns", columns);

        comp.ExecuteNonQuery($"INSERT INTO TableIndices (TableName, IndexName, InstructionLogicalOrder, Columns) " +
                $"VALUES (@TableName, @IndexName, @InstructionLogicalOrder, @Columns)", paramValues);
    }

    private static string GetFunctionDefinition(ProcessUtilityComponents comp, string functionName, int instructionLogicalOrder)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarchar("@FunctionName", 256, functionName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        var maxLogicalOrder = comp.ExecuteScalar<int>($"SELECT MAX(InstructionLogicalOrder)" +
            $" FROM FunctionDefinitions2" +
            $" WHERE FunctionName = @FunctionName" +
            $" AND InstructionLogicalOrder < @InstructionLogicalOrder", paramValues);

        paramValues = new DbParameterContainer();
        paramValues.AddVarchar("@FunctionName", 256, functionName);
        paramValues.AddInteger("@MaxLogicalOrder", maxLogicalOrder);
        var dtb = comp.FetchDataTable($"SELECT * FROM FunctionDefinitions2" +
            $" WHERE FunctionName = @FunctionName" +
            $" AND InstructionLogicalOrder = @MaxLogicalOrder", paramValues);

        foreach (DataRow dr in dtb.Rows)
        {
            return GetString(dr["Definition"]).Trim();
        }

        return string.Empty;
    }

    private static void SaveFunctionDefinition(ProcessUtilityComponents comp, string functionName, 
        string definition, int instructionLogicalOrder)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@FunctionName", functionName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        comp.ExecuteNonQuery($"DELETE FROM FunctionDefinitions2 WHERE FunctionName = @FunctionName" +
            $" AND InstructionLogicalOrder = @InstructionLogicalOrder", paramValues);

        paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@FunctionName", functionName);
        paramValues.AddInteger("@InstructionLogicalOrder", instructionLogicalOrder);
        paramValues.AddVarcharMax("@Definition", definition);

        comp.ExecuteNonQuery($"INSERT INTO FunctionDefinitions2 (FunctionName, InstructionLogicalOrder, Definition) " +
            $"VALUES (@FunctionName, @InstructionLogicalOrder, @Definition)", paramValues);
    }

    private static void ExecuteCustomQuery(ProcessUtilityComponents comp, string query)
    {
        comp.ExecuteNonQuery(query);
        comp.ReportProgress(NonDeterminatePercentage, "Executed Custom Query '" + query + "'");
    }

    private static bool DoesStoredProcedureExist(string procedureName, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        string strCommand = $"SELECT Name FROM sysObjects WHERE Name = '{procedureName}' AND xType = 'P'";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == procedureName.ToUpper())
            return true;
        return false;
    }

    private static bool DoesTableExist(string tableName, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        string strCommand = $"SELECT Name FROM sysObjects WHERE Name = '{tableName}' AND xType = 'U'";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == tableName.ToUpper())
            return true;
        return false;
    }

    private static bool DoesViewExist(string viewName, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        string strCommand = $"SELECT Name FROM sysObjects WHERE Name = '{viewName}' AND xType = 'V'";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == viewName.ToUpper())
            return true;
        return false;
    }

    private static bool DoesPrimaryKeyExistOnTable(string tableName, string columns, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarchar("@TableName", 1024, tableName);
        paramValues.AddVarchar("@Columns", 4096, columns);

        string strCommand = $"SELECT TableName FROM TablePrimaryKeys WHERE TableName = @TableName" +
            $" AND Columns = @Columns";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == tableName.ToUpper())
            return true;
        return false;
    }

    private static bool DoesAnyPrimaryKeyExistOnTable(string tableName, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarchar("@TableName", 1024, tableName);

        string strCommand = $"SELECT TableName FROM TablePrimaryKeys WHERE TableName = @TableName";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == tableName.ToUpper())
            return true;
        return false;
    }

    private static bool DoesIndexExistOnTable(string tableName, string columns, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarchar("@TableName", 1024, tableName);
        paramValues.AddVarchar("@Columns", 4096, columns);

        string strCommand = $"SELECT TableName FROM TableIndices WHERE TableName = @TableName" +
            $" AND Columns = @Columns";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == tableName.ToUpper())
            return true;
        return false;
    }

    private static int GetHighestIndexNoForTable(string tableName, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        paramValues.AddVarchar("@TableName", 1024, tableName);

        string strCommand = $"SELECT IndexNo FROM TableIndexNumbers WHERE TableName = @TableName";
        return comp.ExecuteScalar<int>(strCommand, paramValues);
    }

    private static bool DoesFunctionExist(string functionName, ProcessUtilityComponents comp)
    {
        var paramValues = new DbParameterContainer();
        string strCommand = $"SELECT Name FROM sysObjects WHERE Name = '{functionName}' AND (xType = 'TF' OR xType = 'IF')";
        if (comp.ExecuteScalar<string>(strCommand, paramValues).ToUpper() == functionName.ToUpper())
            return true;
        return false;
    }

    private void AddColumnToTable(ProcessUtilityComponents comp, string tableName,
        string columnName, string typeNameWithPrecisionAndScale,
        bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesTableExist(tableName, comp) == false) throw new Exception($"Table {tableName} does not exist.");

        using (var dtb = comp.GetSchemaTable(tableName))
        {
            if (dtb.Columns.Contains(columnName.Trim()) == false)
            {
                string commandString = "ALTER TABLE " + tableName + " ADD " + columnName.Trim() + Strings.Space(1) + typeNameWithPrecisionAndScale;
                comp.ExecuteNonQuery(commandString);

                if (!skipDependentObjectFormulation) ReformulateAndSaveDependentObjectDefinitions(comp, tableName, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Added column " + columnName.Trim() + " to table " + tableName);
    }

    private void DropColumnFromTable(ProcessUtilityComponents comp,
        string tableName, string columnName, bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesTableExist(tableName, comp) == false) throw new Exception($"Table {tableName} does not exist.");

        using (var dtb = comp.GetSchemaTable(tableName))
        {
            if (dtb.Columns.Contains(columnName) == true)
            {
                var paramValues = new DbParameterContainer();
                var dtbConstraintNames = comp.FetchDataTable($"SELECT Name" +
                    $" FROM SYS.DEFAULT_CONSTRAINTS" +
                    $" WHERE PARENT_OBJECT_ID = OBJECT_ID('{tableName}')" +
                    $" AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns" +
                    $" WHERE NAME = '{columnName}' AND object_id = OBJECT_ID('{tableName}'))", paramValues);

                foreach (DataRow drConstraintName in dtbConstraintNames.Rows)
                {
                    var constraintName = GetString(drConstraintName["Name"]);
                    comp.ExecuteNonQuery($"ALTER TABLE {tableName} DROP CONSTRAINT {constraintName}");
                }

                paramValues = new DbParameterContainer();
                //paramValues.AddVarchar("@TableName", 1024, tableName);
                //paramValues.AddVarchar("@ColumnName", 1024, columnName);

                var dtbDependentIndices = comp.FetchDataTable($"SELECT IndexName FROM TableIndices " +
                    $"WHERE TableName = '{tableName}' AND Columns LIKE '%{columnName}%'", paramValues);

                foreach (DataRow drIdx in dtbDependentIndices.Rows)
                {
                    var idxName = GetString(drIdx["IndexName"]);
                    comp.ExecuteNonQuery($"DROP INDEX IF EXISTS {idxName} ON {tableName}");
                }

                comp.ExecuteNonQuery("ALTER TABLE " + tableName + " DROP COLUMN " + columnName);
                if (!skipDependentObjectFormulation) ReformulateAndSaveDependentObjectDefinitions(comp, tableName, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Dropped column " + columnName + " from table " + tableName);
    }

    private void RegenerateView(ProcessUtilityComponents comp, string viewName, int instructionLogicalOrder)
    {
        if (DoesViewExist(viewName, comp) == false) throw new Exception($"View {viewName} does not exist.");

        string defn = GetViewDefinition(comp, viewName, instructionLogicalOrder);
        if (defn.Trim().Length == 0) return;

        ChangeViewDefinition(comp, viewName, defn, true, instructionLogicalOrder);
        comp.ReportProgress(NonDeterminatePercentage, "Regenerated View " + viewName);
    }

    private void RegenerateFunction(ProcessUtilityComponents comp, string functionName, int instructionLogicalOrder)
    {
        if (DoesFunctionExist(functionName, comp) == false) throw new Exception($"Function {functionName} does not exist.");

        string defn = GetFunctionDefinition(comp, functionName, instructionLogicalOrder);
        if (defn.Trim().Length == 0) return;

        ChangeFunctionDefinition(comp, functionName, defn, true, instructionLogicalOrder);
        comp.ReportProgress(NonDeterminatePercentage, "Regenerated Function " + functionName);
    }

    private void ChangeColumnTypeInTable(ProcessUtilityComponents comp,
        string tableName, string columnName, string typeNameWithPrecisionAndScale,
        bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesTableExist(tableName, comp) == false) throw new Exception($"Table {tableName} does not exist.");

        using (var dtb = comp.GetSchemaTable(tableName))
        {
            if (dtb.Columns.Contains(columnName) == true)
            {
                var paramValues = new DbParameterContainer();
                var dtbConstraintNames = comp.FetchDataTable($"SELECT Name" +
                    $" FROM SYS.DEFAULT_CONSTRAINTS" +
                    $" WHERE PARENT_OBJECT_ID = OBJECT_ID('{tableName}')" +
                    $" AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns" +
                    $" WHERE NAME = '{columnName}' AND object_id = OBJECT_ID('{tableName}'))", paramValues);

                foreach (DataRow drConstraintName in dtbConstraintNames.Rows)
                {
                    var constraintName = GetString(drConstraintName["Name"]);
                    comp.ExecuteNonQuery($"ALTER TABLE {tableName} DROP CONSTRAINT {constraintName}");
                }

                paramValues = new DbParameterContainer();
                paramValues.AddVarchar("@TableName", 1024, tableName);
                paramValues.AddVarchar("@ColumnName", 1024, columnName);

                var dtbDependentIndices = comp.FetchDataTable($"SELECT IndexName FROM TableIndices " +
                    $"WHERE TableName = '{tableName}' AND Columns LIKE '%{columnName}%'", paramValues);

                foreach (DataRow drIdx in dtbDependentIndices.Rows)
                {
                    var idxName = GetString(drIdx["IndexName"]);
                    comp.ExecuteNonQuery($"DROP INDEX IF EXISTS {idxName} ON {tableName}");
                }

                comp.ExecuteNonQuery("ALTER TABLE " + tableName + " ALTER COLUMN " + columnName + Strings.Space(1) + typeNameWithPrecisionAndScale);
                if (!skipDependentObjectFormulation) ReformulateAndSaveDependentObjectDefinitions(comp, tableName, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Changed the type of column " + columnName + " in table " + tableName + " to " + typeNameWithPrecisionAndScale);
    }

    private void RenameColumnInTable(ProcessUtilityComponents comp,
        string tableName, string oldColumnName, string newColumnName,
        bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesTableExist(tableName, comp) == false) throw new Exception($"Table {tableName} does not exist.");

        using (var dtb = comp.GetSchemaTable(tableName))
        {
            if (dtb.Columns.Contains(oldColumnName) == true)
            {
                comp.ExecuteNonQuery("EXEC sp_rename '" + tableName + ".[" + oldColumnName + "]', '" + newColumnName + "', 'COLUMN'");
                if (!skipDependentObjectFormulation) ReformulateAndSaveDependentObjectDefinitions(comp, tableName, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Changed the name of column " + oldColumnName + " in table " + tableName + " to " + newColumnName);
    }

    private static void CreateStoredProcedure(ProcessUtilityComponents comp, string procedureName, string procedureCreationQuery)
    {
        if (DoesStoredProcedureExist(procedureName, comp) == true)
            return;
        comp.ExecuteNonQuery(procedureCreationQuery);
        comp.ReportProgress(NonDeterminatePercentage, "Created Stored Procedure " + procedureName);
    }

    private static void CreateTable(ProcessUtilityComponents comp, string tableName, string tableCreationQuery)
    {
        if (DoesTableExist(tableName, comp) == true)
            return;
        comp.ExecuteNonQuery(tableCreationQuery);
        comp.ReportProgress(NonDeterminatePercentage, "Created table " + tableName);
    }

    private void RenameTable(ProcessUtilityComponents comp, string oldTableName,
        string newTableName, bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesTableExist(oldTableName, comp) == false)
            return;

        var deps = GetDependentObjectNames(comp, oldTableName);

        var dictViewsToRedefine = new Dictionary<string, string>();
        // Key = View Name, Value = View Definition

        var dictFunctionsToRedefine = new Dictionary<string, string>();
        // Key = Function Name, Value = Function Definition

        if (!skipDependentObjectFormulation)
        {
            foreach (var kvp in deps)
            {
                if (kvp.Value == "V")
                {
                    var viewName = kvp.Key;
                    var defn = GetViewDefinition(comp, viewName, instructionLogicalOrder);

                    var newDefn = defn.Replace(oldTableName, newTableName);

                    dictViewsToRedefine.Add(viewName, newDefn);
                }
                else if (kvp.Value == "TF")
                {
                    var functionName = kvp.Key;
                    var defn = GetFunctionDefinition(comp, functionName, instructionLogicalOrder);

                    var newDefn = defn.Replace(oldTableName, newTableName);

                    dictFunctionsToRedefine.Add(functionName, newDefn);
                }
            }
        }

        comp.ExecuteNonQuery("EXEC sp_rename '" + oldTableName + "', '" + newTableName + "'");

        if (!skipDependentObjectFormulation)
        {
            foreach (var kvp in dictViewsToRedefine)
            {
                var viewName = kvp.Key;
                var newDefn = kvp.Value;

                ChangeViewDefinition(comp, viewName, newDefn, false, instructionLogicalOrder);
            }

            foreach (var kvp in dictFunctionsToRedefine)
            {
                var functionName = kvp.Key;
                var newDefn = kvp.Value;

                ChangeFunctionDefinition(comp, functionName, newDefn, false, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Changed the name of table " + oldTableName + " to " + newTableName);
    }

    private static void DropTable(ProcessUtilityComponents comp, string tableName)
    {
        if (DoesTableExist(tableName, comp) == false)
            return;
        comp.ExecuteNonQuery("DROP TABLE " + tableName);
        comp.ReportProgress(NonDeterminatePercentage, "Dropped table " + tableName);
    }

    private static void CreateView(ProcessUtilityComponents comp, string viewName, string viewCreationQuery, int instructionLogicalOrder)
    {
        if (DoesViewExist(viewName, comp) == true)
            return;
        comp.ExecuteNonQuery("CREATE VIEW " + viewName + " AS " + viewCreationQuery);
        SaveViewDefinition(comp, viewName, viewCreationQuery, instructionLogicalOrder);
        comp.ReportProgress(NonDeterminatePercentage, "Created view " + viewName);
    }

    private static void SetPrimaryKeyOnTable(ProcessUtilityComponents comp, string tableName, string columns, int instructionLogicalOrder)
    {
        string columnsTrimmed = columns.Trim().Replace(" ", string.Empty);

        if (!DoesTableExist(tableName, comp)) throw new Exception($"Table {tableName} does not exist.");

        if (DoesPrimaryKeyExistOnTable(tableName, columnsTrimmed, comp) == true) return;

        if (DoesAnyPrimaryKeyExistOnTable(tableName, comp))
        {
            comp.ExecuteNonQuery($"IF OBJECT_ID('dbo.[PK_{tableName}]', 'PK') IS NOT NULL " +
                $"ALTER TABLE {tableName} DROP CONSTRAINT PK_{tableName}");

            var paramValues = new DbParameterContainer();
            paramValues.AddVarcharMax("@TableName", tableName);
            comp.ExecuteNonQuery($"DELETE FROM TablePrimaryKeys WHERE TableName = @TableName", paramValues);
        }

        comp.ExecuteNonQuery($"ALTER TABLE {tableName} ADD CONSTRAINT PK_{tableName} PRIMARY KEY ({columnsTrimmed})");
        SaveTablePrimaryKeyEntry(comp, tableName, $"PK_{tableName}", columnsTrimmed, instructionLogicalOrder);
        comp.ReportProgress(NonDeterminatePercentage, "Created table primary key on " + tableName);
    }

    private static void SetIndexOnTable(ProcessUtilityComponents comp, string tableName, string columns, int instructionLogicalOrder)
    {
        string columnsTrimmed = columns.Trim().Replace(" DESC", "--DESC")
                                .Trim().Replace(" ASC", "--ASC")
                                .Trim().Replace(" ", string.Empty)
                                .Trim().Replace("--ASC", " ASC")
                                .Trim().Replace("--DESC", " DESC");

        if (!DoesTableExist(tableName, comp)) throw new Exception($"Table {tableName} does not exist.");

        if (DoesIndexExistOnTable(tableName, columnsTrimmed, comp) == true) return;

        int idxNo = GetHighestIndexNoForTable(tableName, comp);
        idxNo++;

        comp.ExecuteNonQuery($"CREATE INDEX IDX_{idxNo} ON {tableName} ({columnsTrimmed})");
        SaveTableIndexEntry(comp, tableName, $"IDX_{idxNo}", idxNo, columnsTrimmed, instructionLogicalOrder);
        comp.ReportProgress(NonDeterminatePercentage, $"Set index on table {tableName} with columns {columnsTrimmed}");
    }

    private void ChangeViewDefinition(ProcessUtilityComponents comp,
        string viewName, string newQuery, bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesViewExist(viewName, comp) == false) throw new Exception($"View {viewName} does not exist.");

        comp.ExecuteNonQuery("ALTER VIEW " + viewName + " AS " + newQuery);
        SaveViewDefinition(comp, viewName, newQuery, instructionLogicalOrder);
        if (!skipDependentObjectFormulation) ReformulateAndSaveDependentObjectDefinitions(comp, viewName, instructionLogicalOrder);

        comp.ReportProgress(NonDeterminatePercentage, "Changed View Definition of view " + viewName);
    }

    private void RenameView(ProcessUtilityComponents comp, string oldViewName,
        string newViewName, bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesViewExist(oldViewName, comp) == false)
            return;

        var deps = GetDependentObjectNames(comp, oldViewName);

        var dictViewsToRedefine = new Dictionary<string, string>();
        // Key = View Name, Value = View Definition

        var dictFunctionsToRedefine = new Dictionary<string, string>();
        // Key = Function Name, Value = Function Definition

        if (!skipDependentObjectFormulation)
        {
            foreach (var kvp in deps)
            {
                if (kvp.Value == "V")
                {
                    var viewName = kvp.Key;
                    var defn = GetViewDefinition(comp, viewName, instructionLogicalOrder);

                    var newDefn = defn.Replace(oldViewName, newViewName);

                    dictViewsToRedefine.Add(viewName, newDefn);
                }
                else if (kvp.Value == "TF")
                {
                    var functionName = kvp.Key;
                    var defn = GetFunctionDefinition(comp, functionName, instructionLogicalOrder);

                    var newDefn = defn.Replace(oldViewName, newViewName);

                    dictFunctionsToRedefine.Add(functionName, newDefn);
                }
            }
        }

        comp.ExecuteNonQuery("EXEC sp_rename '" + oldViewName + "', '" + newViewName + "'");

        var paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@NewViewName", newViewName);
        paramValues.AddVarcharMax("@OldViewName", oldViewName);

        comp.ExecuteNonQuery($"UPDATE ViewDefinitions2 SET ViewName = @ViewName" +
            $" WHERE ViewName = @OldViewName");

        if (!skipDependentObjectFormulation)
        {
            foreach (var kvp in dictViewsToRedefine)
            {
                var viewName = kvp.Key;
                var newDefn = kvp.Value;

                ChangeViewDefinition(comp, viewName, newDefn, false, instructionLogicalOrder);
            }

            foreach (var kvp in dictFunctionsToRedefine)
            {
                var functionName = kvp.Key;
                var newDefn = kvp.Value;

                ChangeFunctionDefinition(comp, functionName, newDefn, false, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Changed the name of view " + oldViewName + " to " + newViewName);
    }

    private static void DropView(ProcessUtilityComponents comp, string viewName)
    {
        if (DoesViewExist(viewName, comp) == false)
            return;
        comp.ExecuteNonQuery("DROP VIEW " + viewName);
        comp.ReportProgress(NonDeterminatePercentage, "Dropped view " + viewName);
    }

    private static void CreateFunction(ProcessUtilityComponents comp, string functionName, string functionCreationQuery, int instructionLogicalOrder)
    {
        if (DoesFunctionExist(functionName, comp) == true)
            return;
        comp.ExecuteNonQuery("CREATE FUNCTION " + functionName + Strings.Space(1) + functionCreationQuery);
        SaveFunctionDefinition(comp, functionName, functionCreationQuery, instructionLogicalOrder);
        comp.ReportProgress(NonDeterminatePercentage, "Created function " + functionName);
    }

    private void ChangeFunctionDefinition(ProcessUtilityComponents comp,
        string functionName, string newDefinition,
        bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesFunctionExist(functionName, comp) == false) throw new Exception($"Function {functionName} does not exist.");

        comp.ExecuteNonQuery("ALTER FUNCTION " + functionName + Strings.Space(1) + newDefinition);
        SaveFunctionDefinition(comp, functionName, newDefinition, instructionLogicalOrder);
        if (!skipDependentObjectFormulation) ReformulateAndSaveDependentObjectDefinitions(comp, functionName, instructionLogicalOrder);

        comp.ReportProgress(NonDeterminatePercentage, "Changed Function Definition of function " + functionName);
    }

    private void RenameFunction(ProcessUtilityComponents comp,
        string oldFunctionName, string newFunctionName,
        bool skipDependentObjectFormulation, int instructionLogicalOrder)
    {
        if (DoesFunctionExist(oldFunctionName, comp) == false)
            return;

        var deps = GetDependentObjectNames(comp, oldFunctionName);

        var dictViewsToRedefine = new Dictionary<string, string>();
        // Key = View Name, Value = View Definition

        var dictFunctionsToRedefine = new Dictionary<string, string>();
        // Key = Function Name, Value = Function Definition

        if (!skipDependentObjectFormulation)
        {
            foreach (var kvp in deps)
            {
                if (kvp.Value == "V")
                {
                    var viewName = kvp.Key;
                    var defn = GetViewDefinition(comp, viewName, instructionLogicalOrder);

                    var newDefn = defn.Replace(oldFunctionName, newFunctionName);

                    dictViewsToRedefine.Add(viewName, newDefn);
                }
                else if (kvp.Value == "TF")
                {
                    var functionName = kvp.Key;
                    var defn = GetFunctionDefinition(comp, functionName, instructionLogicalOrder);

                    var newDefn = defn.Replace(oldFunctionName, newFunctionName);

                    dictFunctionsToRedefine.Add(functionName, newDefn);
                }
            }
        }

        comp.ExecuteNonQuery("EXEC sp_rename '" + oldFunctionName + "', '" + newFunctionName + "'");

        var paramValues = new DbParameterContainer();
        paramValues.AddVarcharMax("@NewFunctionName", newFunctionName);
        paramValues.AddVarcharMax("@OldFunctionName", oldFunctionName);

        comp.ExecuteNonQuery($"UPDATE FunctionDefinitions2 SET FunctionName = @NewFunctionName" +
            $" WHERE FunctionName = @OldFunctionName");

        if (!skipDependentObjectFormulation)
        {
            foreach (var kvp in dictViewsToRedefine)
            {
                var viewName = kvp.Key;
                var newDefn = kvp.Value;

                ChangeViewDefinition(comp, viewName, newDefn, false, instructionLogicalOrder);
            }

            foreach (var kvp in dictFunctionsToRedefine)
            {
                var functionName = kvp.Key;
                var newDefn = kvp.Value;

                ChangeFunctionDefinition(comp, functionName, newDefn, false, instructionLogicalOrder);
            }
        }

        comp.ReportProgress(NonDeterminatePercentage, "Changed the name of function " + oldFunctionName + " to " + newFunctionName);
    }

    private static void DropFunction(ProcessUtilityComponents comp, string functionName)
    {
        if (DoesFunctionExist(functionName, comp) == false)
            return;
        comp.ExecuteNonQuery("DROP FUNCTION " + functionName);
        comp.ReportProgress(NonDeterminatePercentage, "Dropped function " + functionName);
    }

    private void ExecuteCustomCode(ProcessUtilityComponents comp, string codeKey)
    {
        comp.ExecuteCustomCode(instructionDataAccessService, customCodeManager, codeKey);
        comp.ReportProgress(NonDeterminatePercentage, "Executed custom code '" + codeKey + "'");
    }

    private class ProcessUtilityComponents
    {
        private readonly DbConnection cnn = null;
        private readonly DbCommand cmd = null;
        private readonly DbDataAdapter dta = null;
        private readonly BackgroundWorker worker = null;

        public ProcessUtilityComponents(DbConnection cnn, DbCommand cmd, DbDataAdapter dta, BackgroundWorker worker)
        {
            this.cnn = cnn;
            this.cmd = cmd;
            this.dta = dta;
            this.worker = worker;
        }

        public static string GetString(object obj)
        {
            if (obj is null)
                return string.Empty;
            if (Information.IsDBNull(obj) == true)
                return string.Empty;
            return obj.ToString();
        }

        public static int GetInt3(object obj)
        {
            if (obj is null)
                return 0;
            if (Information.IsDBNull(obj) == true)
                return 0;
            return Convert.ToInt32(obj);
        }

        public void ReportProgress(int progressPercentage, string message)
        {
            if (worker is null)
                return;
            worker.ReportProgress(progressPercentage, message);
        }

        public object ExecuteScalar(string commandString)
        {
            cmd.CommandText = commandString;
            return cmd.ExecuteScalar();
        }

        public T ExecuteScalar<T>(string commandString, DbParameterContainer parameters)
        {
            cmd.CommandText = commandString;
            cmd.Parameters.Clear();
            parameters.AttachToCommand(cmd);

            dynamic objResult = null;

            using (var dtb = new DataTable())
            {
                dta.Fill(dtb);
                foreach (DataRow dr in dtb.Rows)
                {
                    objResult = dr[0];
                    //return (T)objResult;
                }
            }

            string targetTypeFullName = typeof(T).FullName;

            if (targetTypeFullName == typeof(string).FullName)
            {
                return (T)GetString(objResult);
            }
            else if (targetTypeFullName == typeof(int).FullName)
            {
                return (T)GetInt3(objResult);
            }
            else if (targetTypeFullName == typeof(long).FullName)
            {
                return (T)GetInt64(objResult);
            }

            return default;
        }

        public void ExecuteNonQuery(string commandString)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = commandString;
            cmd.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string commandString, DbParameterContainer paramValues)
        {
            cmd.CommandText = commandString;
            cmd.Parameters.Clear();
            paramValues.AttachToCommand(cmd);
            cmd.ExecuteNonQuery();
        }

        public DataTable GetSchemaTable(string tableName)
        {
            cmd.CommandText = $"SELECT * FROM {tableName} WHERE 1 = 0";
            using var dtb = new DataTable();
            dta.FillSchema(dtb, SchemaType.Mapped);
            return dtb;
        }

        public DataTable FetchDataTable(string query, DbParameterContainer parameters)
        {
            cmd.CommandText = query;
            if (parameters != null)
            {
                cmd.Parameters.Clear();
                parameters.AttachToCommand(cmd);
            }
            using var dtb = new DataTable();
            dta.Fill(dtb);
            return dtb;
        }

        public void ExecuteCustomCode(IDBCADataAccessService instructionDataAccessService, IDBMigrationCustomCodeManager customCodeManager, string codeKey)
        {
            if (customCodeManager is null)
                throw new Exception("Custom Code Manager not found.");
            customCodeManager.ExecuteCustomCode(instructionDataAccessService, codeKey, cnn, cmd.Transaction);
        }
    }
}