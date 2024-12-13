using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Collections.Concurrent;
using Npgsql;
using VJCore.Infrastructure.Extensions;
//using VJ1Core.Infrastructure.Config;
//using VJ1Core.Infrastructure.DatabaseMigration;
//using VJ1Core.Infrastructure.Main;

public class DataAccessUtils
{
    private DbConnection cnn = null;
    private DbTransaction trs = null;

    public bool IsConnectionOpen { get; private set; } = false;
    public bool IsInTransaction { get; private set; } = false;

    private string m_explicitDBName = string.Empty;

    private string DatabaseName => m_explicitDBName.Trim().Length > 0 ? m_explicitDBName : $"{SessionController.ProjectName}{SessionController.CustomerLicenseNo}";

    internal static DBConnectionConfig DatabaseConnectionConfig => DBConnectionConfig.ReadFromFileOrCreateNew();

    public DataAccessUtils()
    {
        
    }

    public DataAccessUtils(string explicitDBName)
    {
        m_explicitDBName = explicitDBName;
    }

    private static string BuildConnectionString(string dbName)
    {
        string connectionString = string.Empty;

        switch (DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                SqlConnectionStringBuilder sscBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = DatabaseConnectionConfig.ServerName,
                    InitialCatalog = dbName,
                    IntegratedSecurity = DatabaseConnectionConfig.IntegratedSecurity,
                    PersistSecurityInfo = false,
                    MultipleActiveResultSets = true
                };

                if (!sscBuilder.IntegratedSecurity)
                {
                    sscBuilder.UserID = DatabaseConnectionConfig.UserId;
                    sscBuilder.Password = DatabaseConnectionConfig.Password;
                }

                connectionString = sscBuilder.ConnectionString;
                break;

            case "NpgSql":
                NpgsqlConnectionStringBuilder npgscsBuilder = new NpgsqlConnectionStringBuilder
                {
                    Database = dbName,
                    PersistSecurityInfo = false
                };

                npgscsBuilder.Username = DatabaseConnectionConfig.UserId;
                npgscsBuilder.Password = DatabaseConnectionConfig.Password;
                
                connectionString = npgscsBuilder.ConnectionString;
                break;
        }

        return connectionString;
    }

    public static DbConnection CreateCheckingConnection()
    {
        Console.WriteLine("Creating Checking Connection ...");

        DbConnection result = null;

        switch(DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                string sqlConnectionString = BuildConnectionString("master");
                //result = new SqlDataConnection(sqlConnectionString);
                result = new SqlConnection(sqlConnectionString);
                break;

            case "NpgSql":
                string npgSqlConnectionString = BuildConnectionString("postgres");
                //result = new NpgsqlDataConnection(npgSqlConnectionString);
                result = new NpgsqlConnection(npgSqlConnectionString);
                break;
        }
       
        return result;
    }

    public static DbConnection GenerateDomainConnection(string dbName, bool showGeneratingStatus)
    {
        if (showGeneratingStatus) Console.WriteLine("Generating Domain Connection ...");

        DbConnection result = null;

        switch (DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                string sqlConnectionString = BuildConnectionString(dbName);
                //result = new SqlDataConnection(sqlConnectionString);
                result = new SqlConnection(sqlConnectionString);
                break;

            case "NpgSql":
                string npgSqlConnectionString = BuildConnectionString(dbName);
                //result = new NpgsqlDataConnection(npgSqlConnectionString);
                result = new NpgsqlConnection(npgSqlConnectionString);
                break;
        }

        return result;
    }

    public static void PerformDatabaseProvisioning(string dbName)
    {
        DatabaseMigrationManager.PerformDatabaseProvisioning(dbName, DatabaseConnectionConfig.DatabaseFolderPath);
    }

    public static DbConnection ProvisionAndCreateNewDBConnection(string dbName)
    {
        PerformDatabaseProvisioning(dbName);

        DbConnection result = GenerateDomainConnection(dbName, true);
        return result;
    }

    private static ConcurrentDictionary<string, List<string>> ComputedColumnNames { get; } = new ConcurrentDictionary<string, List<string>>();
    // Key = Table Name
    // Value = List of Computed Column Names

    private static ConcurrentDictionary<string, string> InsertCommandStrings { get; } = new ConcurrentDictionary<string, string>();
    // Key = Table Name
    // Value = Insert Command String

    private static Dictionary<string, DataTable> TableRawStructures { get; } = new Dictionary<string, DataTable>();
    private static Dictionary<string, DataTable> ViewRawStructures { get; } = new Dictionary<string, DataTable>();

    private static Dictionary<string, TemplateDbParameterContainer> TemplateDbParameterContainers { get; } = new Dictionary<string, TemplateDbParameterContainer>();

    private static DataContainer TableStructures { get; } = new DataContainer();
    private static DataContainer ViewStructures { get; } = new DataContainer();

    public JObject GetBlankDataObjectFromTable(string tableName)
    {
        if (TableStructures.CollectionExists(tableName))
        {
            var dtb = TableRawStructures[tableName];
            var result = ClassService.InitializeDataObject(dtb);

            return result;
        }

        return null;
    }

    private static TemplateDbParameterContainer GetTemplateDbParameterContainer(string objectName)
    {
        if (TemplateDbParameterContainers.ContainsKey(objectName))
        {
            return TemplateDbParameterContainers[objectName];
        }

        return null;
    }

    public static TemplateDbParameterContainer GetTemplateDbParameterContainer(DbCommand cmd, string objectName)
    {
        var cont = GetTemplateDbParameterContainer(objectName);

        if (cont == null)
        {
            cont = FormulateTemplateDbParameterContainerforObject(cmd, objectName);
            TemplateDbParameterContainers.Add(objectName, cont);
        }
        
        return cont;
    }

    public static DataTable GetTableRawStructure(string tableName)
    {
        if (TableRawStructures.ContainsKey(tableName)) return TableRawStructures[tableName];
        return null;
    }

    public static DataTable GetViewRawStructure(string viewName)
    {
        if (ViewRawStructures.ContainsKey(viewName)) return ViewRawStructures[viewName];
        return null;
    }

    public void InstantiateDBConnectionWithoutMigration()
    {
        cnn = ProvisionAndCreateNewDBConnection(DatabaseName);
    }

    private static TemplateDbParameterContainer FormulateTemplateDbParameterContainerforObject(DbCommand cmd, string objectName)
    {
        TemplateDbParameterContainer result = new TemplateDbParameterContainer(objectName);

        using var dta = CreateDataAdapter(cmd);
        using var dtb = new DataTable();

        switch (DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                cmd.CommandText = $"SELECT c.name as column_name, t.name AS column_type, c.max_length," +
                    $" c.precision, c.scale, c.is_nullable" +
                    $" FROM sys.columns c" +
                    $" JOIN sys.types t" +
                    $" ON c.user_type_id = t.user_type_id" +
                    $" WHERE c.object_id = Object_id('{objectName}')";
                
                dta.Fill2(dtb);

                foreach (DataRow dr in dtb.Rows)
                {
                    string columnName = Utils.GetString(dr["column_name"]);
                    string columnType = Utils.GetString(dr["column_type"]);
                    int size = Utils.GetInt32(dr["max_length"]);
                    byte precision = Utils.GetByte(dr["precision"]);
                    byte scale = Utils.GetByte(dr["scale"]);

                    switch (columnType)
                    {
                        case "varchar":
                            result.AddVarchar($"@{columnName}", size); break;
                        case "nvarchar":
                            result.AddNVarchar($"@{columnName}", size); break;
                        case "int":
                            result.AddInteger($"@{columnName}"); break;
                        case "bigint":
                            result.AddLong($"@{columnName}"); break;
                        case "decimal":
                            result.AddDecimal($"@{columnName}", precision, scale); break;
                    }
                }

                break;

            case "NpgSql":
                cmd.CommandText = $"select column_name, data_type, character_maximum_length, " +
                    $"numeric_precision, numeric_scale" +
                    $" from information_schema.columns " +
                    $" where table_name = '{objectName}'";

                dta.Fill2(dtb);

                foreach (DataRow dr in dtb.Rows)
                {
                    string columnName = Utils.GetString(dr["column_name"]);
                    string columnType = Utils.GetString(dr["data_type"]);
                    int size = Utils.GetInt32(dr["character_maximum_length"]);
                    byte precision = Utils.GetByte(dr["numeric_precision"]);
                    byte scale = Utils.GetByte(dr["numeric_scale"]);

                    switch (columnType)
                    {
                        case "character varying":
                            result.AddVarchar($"@{columnName}", size); break;
                        case "nvarchar":
                            result.AddNVarchar($"@{columnName}", size); break;
                        case "integer":
                            result.AddInteger($"@{columnName}"); break;
                        case "bigint":
                            result.AddLong($"@{columnName}"); break;
                        case "numeric":
                            result.AddDecimal($"@{columnName}", precision, scale); break;
                    }
                }

                break;
        }

        return result;
    }

    private void CacheTemplateDbParameterContainers(DbCommand cmd)
    {
        TemplateDbParameterContainers.Clear();

        foreach (string tableName in GetTableNamesInDB(cmd))
        {
            if (!TemplateDbParameterContainers.ContainsKey(tableName))
            {
                var cont = FormulateTemplateDbParameterContainerforObject(cmd, tableName);
                TemplateDbParameterContainers.Add(tableName, cont);
            }
        }

        foreach (string viewName in GetViewNamesInDB(cmd))
        {
            if (!TemplateDbParameterContainers.ContainsKey(viewName))
            {
                var cont = FormulateTemplateDbParameterContainerforObject(cmd, viewName);
                TemplateDbParameterContainers.Add(viewName, cont);
            }
        }
    }

    //private class TimeTaken
    //{
    //    public string ObjectName { get; set; } = string.Empty;
    //    public double TimeInMS { get; set; } = 0;
    //}

    public void InstantiateDBConnection()
    {
        DatabaseMigrationManager.PerformDatabaseProvisioningAndMigration(DatabaseName, DatabaseConnectionConfig.DatabaseFolderPath);

        cnn = GenerateDomainConnection(DatabaseName, true);

        var cToken = OpenConnection();

        try
        {
            DbCommand cmd = cnn.CreateCommand();

            CacheComputedColumnNames(cmd);
            CacheTemplateDbParameterContainers(cmd);

            //List<TimeTaken> lstTimesTaken = new List<TimeTaken>();

            foreach (string tableName in GetTableNamesInDB(cmd))
            {
                //DateTime dtBefore = DateTime.Now;
                FillTableSchema(CreateCommand(), TableStructures, tableName, "T");
                //DateTime dtAfter = DateTime.Now;

                //lstTimesTaken.Add(new TimeTaken { ObjectName = tableName, TimeInMS = dtAfter.Subtract(dtBefore).TotalMilliseconds });
            }

            foreach (string viewName in GetViewNamesInDB(cmd))
            {
                try
                {
                    //DateTime dtBefore = DateTime.Now;
                    FillTableSchema(CreateCommand(), ViewStructures, viewName, "V");
                    //DateTime dtAfter = DateTime.Now;

                    //lstTimesTaken.Add(new TimeTaken { ObjectName = viewName, TimeInMS = dtAfter.Subtract(dtBefore).TotalMilliseconds });
                }
                catch
                {
                    // SWALLOW FOR NOW
                }
            }

            //lstTimesTaken = lstTimesTaken.OrderByDescending(t => t.TimeInMS).ToList();

            //File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TimesTaken.txt"), 
            //    lstTimesTaken.Select(t => JsonConvert.SerializeObject(t)).ToList());
        }
        finally
        {
            CloseConnection(cToken);
        }
    }

    public static bool DoesTableExistInDB(DbCommand cmd, string tableName)
    {
        cmd.CommandText = $"select count(name) from sysobjects where xType = 'U' and name = '{tableName}'";
        return Utils.GetInt32(cmd.ExecuteScalar2()) > 0;
    }

    public static bool DoesViewExistInDB(DbCommand cmd, string viewName)
    {
        cmd.CommandText = $"select count(name) from sysobjects where xType = 'V' and name = '{viewName}'";
        return Utils.GetInt32(cmd.ExecuteScalar2()) > 0;
    }

    public static List<string> GetTableNamesInDB(DbCommand cmd)
    {
        List<string> result = new List<string>();

        using (DbDataAdapter dta = CreateDataAdapter(cmd))
        {
            cmd.CommandText = "select name from sysobjects where xType = 'U'";
            using (DataTable dtb = new DataTable())
            {
                dta.Fill2(dtb);
                result = Utils.FormulateStringList(dtb, "name", string.Empty, "name");
            }
        }

        return result;
    }

    public static List<string> GetViewNamesInDB(DbCommand cmd)
    {
        List<string> result = new List<string>();

        using (DbDataAdapter dta = CreateDataAdapter(cmd))
        {
            cmd.CommandText = "select name from sysobjects where xType = 'V'";
            using (DataTable dtb = new DataTable())
            {
                dta.Fill2(dtb);
                result = Utils.FormulateStringList(dtb, "name", string.Empty, "name");
            }
        }

        return result;
    }

    public DbCommand CreateCommand()
    {
        var result = cnn.CreateCommand();
        result.Transaction = trs;

        return result;
    }

    public static DbDataAdapter CreateDataAdapter(DbCommand cmd)
    {
        DbDataAdapter result = null;

        dynamic cmdInternal = cmd;

        switch (DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                result = new SqlDataAdapter((SqlCommand)cmdInternal);
                //result = new SqlDbDataAdapter(cmd);
                //result = DbProviderFactories.GetFactory("Sql").CreateDataAdapter();
                //result.SelectCommand = cmd;
                break;
            case "NpgSql":
                result = new NpgsqlDataAdapter((NpgsqlCommand)cmdInternal);
                //result = new NpgsqlDbDataAdapter(cmd);
                //result = DbProviderFactories.GetFactory("NpgSql").CreateDataAdapter();
                //result.SelectCommand = cmd;
                break;
        }

        return result;
    }

    private UInt64 m_connectionToken = 0;

    private UInt64 m_connectionOwnerToken = 0;

    public UInt64 OpenConnection()
    {
        m_connectionToken++;

        if (IsConnectionOpen) return m_connectionToken;
        cnn.Open();
        IsConnectionOpen = true;
        m_connectionOwnerToken = m_connectionToken;

        return m_connectionToken;
    }

    public void CloseConnection(UInt64 connectionToken)
    {
        if (!IsConnectionOpen) return;
        if (connectionToken != m_connectionOwnerToken) return;

        IsInTransaction = false;

        try
        {
            if (trs != null)
            {
                trs.Rollback();
                trs = null;

                throw new DomainException(string.Empty);
            }
        }
        catch (Exception ex)
        {
            string errorFileContent = ex.StackTrace;

            string ts = DTU.GetCurrentDateTime();

            string errorFileName = $"{AppDomain.CurrentDomain.BaseDirectory}ConnectionClosedWithActiveTransaction-{ts}.txt";

            File.WriteAllText(errorFileName, errorFileContent);
        }

        cnn.Close();
        IsConnectionOpen = false;
        m_connectionOwnerToken = 0;
    }

    public void BeginTransaction()
    {
        if (IsInTransaction) return;
        if (trs != null) throw new DomainException("Cannot begin a new transaction before committing or rolling back the previous transaction.");
        trs = cnn.BeginTransaction();
        IsInTransaction = true;
    }

    public UInt64 OpenConnectionAndBeginTransaction()
    {
        UInt64 result = OpenConnection();
        BeginTransaction();

        return result;
    }

    public void CommitTransaction(UInt64 connectionToken)
    {
        if (!IsInTransaction) return;
        if (connectionToken != m_connectionOwnerToken) return;

        if (trs != null)
        {
            trs.Commit();
            trs = null;
            IsInTransaction = false;
        }
    }

    public void CommitTransactionAndCloseConnection(UInt64 connectionToken)
    {
        CommitTransaction(connectionToken);
        CloseConnection(connectionToken);
    }

    public void RollbackTransaction(UInt64 connectionToken)
    {
        if (!IsInTransaction) return;
        if (connectionToken != m_connectionOwnerToken) return;

        if (trs != null)
        {
            trs.Rollback();
            trs = null;
            IsInTransaction = false;
        }
    }

    public void RollbackTransactionAndCloseConnection(UInt64 connectionToken)
    {
        RollbackTransaction(connectionToken);
        CloseConnection(connectionToken);
    }

    private static bool IsComputedColumn(string tableName, string columnName)
    {
        var containsTableName = ComputedColumnNames.TryGetValue(tableName, out var lstComputedColumnNames);
        if (!containsTableName) return false;

        return lstComputedColumnNames.Contains(columnName);
    }

    public static void FormulateInsertCommandFromTable(string tableName, DbCommand cmd, DataTable dtb, DataRow dr = null)
    {
        string strCommand = string.Empty;

        if (!InsertCommandStrings.TryGetValue(tableName, out strCommand))
        {
            string strColumnNames = string.Empty;
            string strColumnValues = string.Empty;

            {
                StringBuilder sbColumnNames = new StringBuilder();
                StringBuilder sbColumnValues = new StringBuilder();

                foreach (DataColumn dc in dtb.Columns)
                {
                    if (!IsComputedColumn(tableName, dc.ColumnName))
                    {
                        if (sbColumnNames.Length > 0) sbColumnNames.Append(",");
                        sbColumnNames.Append(dc.ColumnName);

                        if (sbColumnValues.Length > 0) sbColumnValues.Append(",");
                        sbColumnValues.Append($"@{dc.ColumnName}");
                    }
                }

                strColumnNames = sbColumnNames.ToString();
                strColumnValues = sbColumnValues.ToString();
            }

            strCommand = $"INSERT INTO {tableName} ({strColumnNames}) VALUES ({strColumnValues})";

            InsertCommandStrings.TryAdd(tableName, strCommand);
        }

        cmd.CommandText = strCommand;

        if (dr != null)
        {
            FormulateParametersFromDataRow(cmd, tableName, dtb, dr);
        }
    }

    public static void FormulateParametersFromDataRow(DbCommand cmd, string tableName, DataTable dtb, DataRow dr)
    {
        cmd.Parameters.Clear();

        foreach (DataColumn dc in dtb.Columns)
        {
            if (!IsComputedColumn(tableName, dc.ColumnName))
            {
                string columnName = dc.ColumnName;

                var paramValue = dr[columnName];

                SqlParameter param = new SqlParameter($"@{columnName}", paramValue);

                _ = cmd.Parameters.Add(param);
            }
        }
    }

    public static void FormulateInsertCommandFromTable(string tableName, SQLiteCommand cmd, DataTable dtb, DataRow dr = null)
    {
        string strCommand = string.Empty;

        if (!InsertCommandStrings.TryGetValue(tableName, out strCommand))
        {
            string strColumnNames = string.Empty;
            string strColumnValues = string.Empty;

            {
                StringBuilder sbColumnNames = new StringBuilder();
                StringBuilder sbColumnValues = new StringBuilder();

                foreach (DataColumn dc in dtb.Columns)
                {
                    if (!IsComputedColumn(tableName, dc.ColumnName))
                    {
                        if (sbColumnNames.Length > 0) sbColumnNames.Append(",");
                        sbColumnNames.Append(dc.ColumnName);

                        if (sbColumnValues.Length > 0) sbColumnValues.Append(",");
                        sbColumnValues.Append($"@{dc.ColumnName}");
                    }
                }

                strColumnNames = sbColumnNames.ToString();
                strColumnValues = sbColumnValues.ToString();
            }

            strCommand = $"INSERT INTO {tableName} ({strColumnNames}) VALUES ({strColumnValues})";

            InsertCommandStrings.TryAdd(tableName, strCommand);
        }

        cmd.CommandText = strCommand;

        if (dr != null)
        {
            FormulateParametersFromDataRow(cmd, tableName, dtb, dr);
        }
    }

    public static void FormulateParametersFromDataRow(SQLiteCommand cmd, string tableName, DataTable dtb, DataRow dr)
    {
        cmd.Parameters.Clear();

        foreach (DataColumn dc in dtb.Columns)
        {
            if (!IsComputedColumn(tableName, dc.ColumnName))
            {
                string columnName = dc.ColumnName;

                var paramValue = dr[columnName];

                SQLiteParameter param = new SQLiteParameter($"@{columnName}", paramValue);

                _ = cmd.Parameters.Add(param);
            }
        }
    }

    public static void FormulateInsertCommandFromData(string tableName, DbCommand cmd, JObject data = null)
    {
        TableRawStructures.TryGetValue(tableName, out DataTable dtb);

        //string strCommand = string.Empty;

        if (!InsertCommandStrings.TryGetValue(tableName, out string strCommand))
        {
            string strColumnNames = string.Empty;
            {
                StringBuilder sbColumnNames = new StringBuilder(dtb.Columns.Count);

                foreach (DataColumn dc in dtb.Columns)
                {
                    if (!IsComputedColumn(tableName, dc.ColumnName))
                    {
                        if (sbColumnNames.Length > 0) sbColumnNames.Append(",");
                        sbColumnNames.Append(dc.ColumnName);
                    }
                }

                strColumnNames = sbColumnNames.ToString();
            }

            string strColumnValues = string.Empty;
            {
                StringBuilder sbColumnValues = new StringBuilder(dtb.Columns.Count);

                foreach (DataColumn dc in dtb.Columns)
                {
                    if (!IsComputedColumn(tableName, dc.ColumnName))
                    {
                        if (sbColumnValues.Length > 0) sbColumnValues.Append(",");
                        sbColumnValues.Append($"@{dc.ColumnName}");
                    }
                }

                strColumnValues = sbColumnValues.ToString();
            }

            strCommand = $"INSERT INTO {tableName} ({strColumnNames}) VALUES ({strColumnValues})";

            InsertCommandStrings.TryAdd(tableName, strCommand);
        }

        cmd.CommandText = strCommand;

        if (data != null)
        {
            FormulateParametersFromData(cmd, tableName, dtb, data);
        }
    }

    public static void FormulateParametersFromData(DbCommand cmd, string tableName, 
        DataTable structure, JObject data)
    {
        cmd.Parameters.Clear();

        foreach (DataColumn dc in structure.Columns)
        {
            if (!IsComputedColumn(tableName, dc.ColumnName))
            {
                string columnName = dc.ColumnName;

                object paramValue = null;

                if (data.ContainsKey(columnName))
                {
                    paramValue = data[columnName].ToObject(typeof(object));
                }
                else
                {
                    if (dc.DataType.FullName == typeof(string).FullName)
                    {
                        paramValue = string.Empty;
                    }
                    else
                    {
                        paramValue = 0;
                    }
                }

                SqlParameter param = new SqlParameter($"@{columnName}", paramValue);

                _ = cmd.Parameters.Add(param);
            }
        }
    }

    //public static void FormulateUpdateCommandFromData(DbCommand cmd, string tableName,
    //    List<string> columnNamesToUpdate, string filterCondition, Dictionary<string, object> dictParams)
    //{
    //    var paramValues = DbParameterContainer.CreateInstance();
    //    var contTemplate = GetTemplateDbParameterContainer(tableName);

    //    foreach (var kvp in dictParams)
    //    {
    //        string paramName = kvp.Key;
    //        object pValue = kvp.Value;

    //        var pInfo = contTemplate.GetDbParameterInfo(paramName);

    //        switch (pInfo.DbType)
    //        {
    //            case DbTypeManager.DbType_Varchar:
    //                paramValues.AddVarchar(paramName, pInfo.Size, Convert.ToString(pValue));
    //                break;
    //            case DbTypeManager.DbType_NVarchar:
    //                paramValues.AddNVarchar(paramName, pInfo.Size, Convert.ToString(pValue));
    //                break;
    //            case DbTypeManager.DbType_Long:
    //                paramValues.AddLong(paramName, Convert.ToInt64(pValue));
    //                break;
    //            case DbTypeManager.DbType_Int:
    //                paramValues.AddInteger(paramName, Convert.ToInt32(pValue));
    //                break;
    //            case DbTypeManager.DbType_Decimal:
    //                paramValues.AddDecimal(paramName, pInfo.Precision, pInfo.Scale, Convert.ToDecimal(pValue));
    //                break;
    //            case DbTypeManager.DbType_Bool:
    //                paramValues.AddBoolean(paramName, Convert.ToBoolean(pValue));
    //                break;
    //        }
    //    }

    //    FormulateUpdateCommandFromData(cmd, tableName, columnNamesToUpdate, filterCondition, paramValues);
    //}

    public static void FormulateUpdateCommandFromData(DbCommand cmd, string tableName,
        List<string> columnNamesToUpdate, string filterCondition, DbParameterContainer paramValues)
    {
        StringBuilder sbColumnValues = new StringBuilder();

        foreach (var prop in columnNamesToUpdate)
        {
            if (!IsComputedColumn(tableName, prop))
            {
                if (sbColumnValues.Length > 0) sbColumnValues.Append(",");
                sbColumnValues.Append($"{prop} = @{prop}");
            }
        }

        if (sbColumnValues.Length > 0)
        {
            string strColumnValues = sbColumnValues.ToString();

            string strCommand = $"UPDATE {tableName} SET {strColumnValues} WHERE {filterCondition}";

            cmd.CommandText = strCommand;
            paramValues.AttachToCommand(cmd);
        }
    }

    //public static void FormulateUpdateCommandParametersFromData(DbCommand cmd, string tableName, JObject data)
    //{
    //    cmd.Parameters.Clear();

    //    foreach (var prop in data.Properties())
    //    {
    //        if (!IsComputedColumn(tableName, prop.Name))
    //        {
    //            object paramValue = data[prop.Name].ToObject(typeof(object));
    //            DbParameter param = new SqlParameter($"@{prop.Name}", paramValue);
    //            _ = cmd.Parameters.Add(param);
    //        }
    //    }
    //}

    public static JObject JObjectFromDataRow(DataRow dr)
    {
        JObject result = new JObject();

        foreach (DataColumn dc in dr.Table.Columns)
        {
            result.Add(dc.ColumnName, JToken.FromObject(dr[dc.ColumnName]));
        }

        return result;
    }

    public static void TransferDataTableColumnDefinitionsToDataCollection(DataTable dtb, ref DataCollection dc)
    {
        dc.ColumnDefinitions.RemoveAll();

        foreach (DataColumn col in dtb.Columns)
        {
            dc.ColumnDefinitions.Add(col.ColumnName, string.Empty);
        }
    }

    public static void TransferDataTableDataToDataCollection(DataTable dtb, ref DataCollection dc)
    {
        foreach (DataRow dr in dtb.Rows)
        {
            dc.Entries.Add(JObjectFromDataRow(dr));
        }
    }

    public static void TransferDataTableRowsToDataContainer(string tableName, DataRow[] rows, ref DataContainer dc)
    {
        var coll = dc.GetOrCreateCollection(tableName);

        foreach (DataRow dr in rows)
        {
            coll.Entries.Add(JObjectFromDataRow(dr));
        }
    }

    private void CacheComputedColumnNames(DbCommand cmd)
    {
        ComputedColumnNames.Clear();

        cmd.CommandText = "select schema_name(o.schema_id) as schema_name," +
            " object_name(c.object_id) as table_name, column_id, c.name as column_name," +
            " type_name(user_type_id) as data_type, definition" +
            " from sys.computed_columns c join sys.objects o on o.object_id = c.object_id" +
            " order by schema_name, table_name, column_id";

        using var dta = CreateDataAdapter(cmd);
        using var dtb = new DataTable();

        dta.Fill2(dtb);

        foreach (DataRow dr in dtb.Rows)
        {
            string tableName = Utils.GetString(dr["table_name"]);
            string computedColumnName = Utils.GetString(dr["column_name"]);

            if (!ComputedColumnNames.ContainsKey(tableName))
            {
                ComputedColumnNames.TryAdd(tableName, new List<string>());
            }

            ComputedColumnNames.TryGetValue(tableName, out var lstComputedColumnNames);
            if (lstComputedColumnNames != null) lstComputedColumnNames.Add(computedColumnName);
        }
    }

    private void FillTableSchema(DbCommand cmd, DataContainer container, string tableName, string objectType)
    {
        cmd.CommandText = $"select * from {tableName}";

        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.FillSchema(dtb, SchemaType.Mapped);

        if (objectType == "T")
        {
            TableRawStructures.Add(tableName, dtb);
        }
        else if (objectType == "V")
        {
            ViewRawStructures.Add(tableName, dtb);
        }

        var dc = container.GetOrCreateCollection(tableName);
        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
    }

    public void FillByQuery(DbCommand cmd, DataSet ds, string tableName, string qry, 
        DbParameterContainer parameters = null)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            cmd.Parameters.Clear();
            parameters.AttachToCommand(cmd);
        }

        var dta = CreateDataAdapter(cmd);

        dta.Fill2(ds, tableName);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();

            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }
    }

    public void FillByQuery(DbCommand cmd, DataSet ds, string tableName, string qry, Dictionary<string, object> parameters = null)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            cmd.Parameters.Clear();

            foreach (var kvp in parameters)
            {
                cmd.AddParameterWithValue(kvp.Key, kvp.Value);
            }
        }

        var dta = CreateDataAdapter(cmd);

        dta.Fill2(ds, tableName);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();

            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }
    }

    public void FillByQuery(DbCommand cmd, DataTable dtb, string qry, DbParameterContainer parameters = null)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            cmd.Parameters.Clear();
            parameters.AttachToCommand(cmd);
        }

        var dta = CreateDataAdapter(cmd);

        dta.Fill2(dtb);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();
            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }
    }

    public void FillByQuery(DbCommand cmd, DataTable dtb)
    {
        using var dta = CreateDataAdapter(cmd);
        dta.Fill2(dtb);
    }

    public void FillByQuery(DbCommand cmd, DataContainer container, string tableName, string qry)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.Fill2(dtb);

        var dc = container.GetOrCreateCollection(tableName);
        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
        TransferDataTableDataToDataCollection(dtb, ref dc);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();
            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }
    }

    public void FillByQueryAndTableName(DbCommand cmd, DataContainer container, string tableName)
    {
        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.Fill2(dtb);

        var dc = container.GetOrCreateCollection(tableName);
        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
        TransferDataTableDataToDataCollection(dtb, ref dc);
    }

    public void FillByQuery(DbCommand cmd, DataContainer container, string tableName, string qry, DbParameterContainer parameters)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            cmd.Parameters.Clear();
            parameters.AttachToCommand(cmd);
        }

        FillByQueryAndTableName(cmd, container, tableName);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();
            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }
    }

    public void FillByQuery(DbCommand cmd, DataContainer container, string tableName, string qry, Dictionary<string, object> parameters)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                cmd.AddParameterWithValue(kvp.Key, kvp.Value);
            }
        }

        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.Fill2(dtb);

        var dc = container.GetOrCreateCollection(tableName);
        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
        TransferDataTableDataToDataCollection(dtb, ref dc);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();
            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }
    }


    public void FillTableSchema(DbCommand cmd, DataCollection dc)
    {
        cmd.CommandText = $"select * from {dc.Name}";

        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.FillSchema(dtb, SchemaType.Mapped);

        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
    }

    public void FillEntireTable(DbCommand cmd, DataCollection dc)
    {
        cmd.CommandText = $"select * from {dc.Name}";

        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.Fill2(dtb);

        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
        TransferDataTableDataToDataCollection(dtb, ref dc);
    }

    public static void FillByCommand(DbCommand cmd, DataCollection dc)
    {
        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();

        //DateTime dtStart = DateTime.Now;

        dta.Fill2(dtb);

        //DateTime dtEnd = DateTime.Now;

        //List<string> lstLog = new List<string>
        //{
        //    $"{dtEnd.Subtract(dtStart).TotalMilliseconds},{cmd.CommandText}"
        //};

        //string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dtafilllog.csv");

        //File.AppendAllLines(logFilePath, lstLog);

        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
        TransferDataTableDataToDataCollection(dtb, ref dc);
    }

    public static void FillByQuery(DbCommand cmd, DataCollection dc, string qry)
    {
        cmd.CommandText = qry;
        FillByCommand(cmd, dc);
    }

    public void FillByQuery(DbCommand cmd, DataCollection dc, string qry, DbParameterContainer parameters)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            parameters.AttachToCommand(cmd);
        }

        var dta = CreateDataAdapter(cmd);

        DataTable dtb = new DataTable();
        dta.Fill2(dtb);

        dc.Entries.Clear();

        TransferDataTableColumnDefinitionsToDataCollection(dtb, ref dc);
        TransferDataTableDataToDataCollection(dtb, ref dc);

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();
            foreach(var p in cmdParams) cmd.Parameters.Add(p);
        }
    }

    public static object GetScalarValueByQuery(DbCommand cmd, string qry)
    {
        cmd.CommandText = qry;
        return cmd.ExecuteScalar2();
    }

    public static object GetScalarValueByQuery(DbCommand cmd, string qry, DbParameterContainer parameters)
    {
        var cmdParams = cmd.Parameters;

        cmd.Parameters.Clear();
        cmd.CommandText = qry;

        if (parameters != null)
        {
            parameters.AttachToCommand(cmd);
        }

        var result = cmd.ExecuteScalar2();

        if (cmdParams != null)
        {
            cmd.Parameters.Clear();
            foreach (var p in cmdParams) cmd.Parameters.Add(p);
        }

        return result;
    }

    public static int GetVarcharFieldSize(string dbObjectName, string columnName)
    {
        var cont = GetTemplateDbParameterContainer(dbObjectName);
        if (cont != null)
        {
            var p = cont.GetVarcharParameter(columnName);
            if (p != null) return p.Size;
        }

        return 256;
    }

    public static List<long> GetLongListFromDB(DbCommand cmd)
    {
        using DataTable dtb = new DataTable();
        using DbDataAdapter dta = CreateDataAdapter(cmd);
        dta.Fill2(dtb);

        return Utils.FormulateLongList(dtb, dtb.Columns[0].ColumnName);
    }

    public static List<int> GetIntegerListFromDB(DbCommand cmd)
    {
        using DataTable dtb = new DataTable();
        using DbDataAdapter dta = CreateDataAdapter(cmd);
        dta.Fill2(dtb);

        return Utils.FormulateIntegerList(dtb, dtb.Columns[0].ColumnName);
    }
}
