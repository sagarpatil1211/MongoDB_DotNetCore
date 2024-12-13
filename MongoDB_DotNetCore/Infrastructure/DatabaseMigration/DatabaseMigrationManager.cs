using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using VJCore.Infrastructure.Extensions;
using Npgsql;
//using VJ1Core.Infrastructure.Config;
//using VJ1Core.Infrastructure.DatabaseMigration.SQLiteConnectivity;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class DatabaseMigrationManager
{
    private static DBConnectionConfig DatabaseConnectionConfig => DBConnectionConfig.ReadFromFileOrCreateNew();

    public static int GetInt32(object input)
    {
        if (input == null || input == DBNull.Value || !int.TryParse(input.ToString(), out _)) return 0;
        return Convert.ToInt32(input);
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

        switch (DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                string sqlConnectionString = BuildConnectionString("master");
                result = new SqlConnection(sqlConnectionString);
                //result = new SqlDataConnection(sqlConnectionString);
                break;

            case "NpgSql":
                string npgSqlConnectionString = BuildConnectionString("postgres");
                result = new NpgsqlConnection(npgSqlConnectionString);
                //result = new NpgsqlDataConnection(npgSqlConnectionString);
                break;
        }

        return result;
    }

    public static DbConnection GenerateDomainConnection(string dbName)
    {
        Console.WriteLine("Generating Domain Connection ...");

        DbConnection result = null;

        switch (DatabaseConnectionConfig.DatabaseType)
        {
            case "Sql":
                string sqlConnectionString = BuildConnectionString(dbName);
                result = new SqlConnection(sqlConnectionString);
                //result = new SqlDataConnection(sqlConnectionString);
                break;

            case "NpgSql":
                string npgSqlConnectionString = BuildConnectionString(dbName);
                result = new NpgsqlConnection(npgSqlConnectionString);
                //result = new NpgsqlDataConnection(npgSqlConnectionString);
                break;
        }

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
                break;
            case "NpgSql":
                result = new NpgsqlDataAdapter((NpgsqlCommand)cmdInternal);
                //result = new NpgsqlDbDataAdapter(cmd);
                break;
        }

        return result;
    }

    public static void PerformDatabaseProvisioning(string dbName, string folderPath)
    {
        var cnnCheck = CreateCheckingConnection();

        cnnCheck.Open();

        try
        {
            using DbCommand cmdCheck = cnnCheck.CreateCommand();
            cmdCheck.CommandText = $"select count(name) from sysdatabases where name = @DatabaseName";
            cmdCheck.AddVarcharParameter("@DatabaseName", 256, dbName);

            if (GetInt32(cmdCheck.ExecuteScalar()) == 0)
            {
                cmdCheck.Parameters.Clear();

                List<string> lstCreationCommands = GenerateDatabaseCreationCommands(dbName, folderPath);

                foreach (string command in lstCreationCommands)
                {
                    cmdCheck.CommandText = command;
                    cmdCheck.ExecuteNonQuery();
                }

                cnnCheck.ChangeDatabase(dbName);

                List<string> lstPostCreationCommands = GenerateDatabasePostCreationCommands(folderPath);

                foreach (string command in lstPostCreationCommands)
                {
                    cmdCheck.CommandText = command;
                    cmdCheck.ExecuteNonQuery();
                }
            }
        }
        finally
        {
            cnnCheck.Close();
        }
    }

    public static void PerformDatabaseMigration(string dbName)
    {
        SqlServerDatabaseChangeInstructionManager migrationManager = new SqlServerDatabaseChangeInstructionManager(BuildConnectionString(dbName),
            new SQLiteDataAccessService(string.Empty, (msg, title) => { }), msg => { }, (progressPercentage, msg) => { }, msg => { }, msg => { });
        migrationManager.ExecuteChanges(false);
    }

    public static void PerformDatabaseProvisioningAndMigration(string dbName, string folderPath)
    {
        PerformDatabaseProvisioning(dbName, folderPath);
        PerformDatabaseMigration(dbName);
    }

    private static List<string> GenerateDatabaseCreationCommands(string dbName, string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = $"{folderPath}{dbName}.MDF";
        bool databaseFileExists = File.Exists(filePath);

        List<string> result = new List<string>
    {
        $"CREATE DATABASE [{dbName}]",
        $"ALTER DATABASE [{dbName}] SET COMPATIBILITY_LEVEL = 140",
        $"ALTER DATABASE [{dbName}] SET ANSI_NULL_DEFAULT OFF",
        $"ALTER DATABASE [{dbName}] SET ANSI_NULLS OFF",
        $"ALTER DATABASE [{dbName}] SET ANSI_PADDING OFF",
        $"ALTER DATABASE [{dbName}] SET ANSI_WARNINGS OFF",
        $"ALTER DATABASE [{dbName}] SET ARITHABORT OFF",
        $"ALTER DATABASE [{dbName}] SET AUTO_CLOSE OFF",
        $"ALTER DATABASE [{dbName}] SET AUTO_SHRINK OFF",
        $"ALTER DATABASE [{dbName}] SET AUTO_CREATE_STATISTICS OFF",
        $"ALTER DATABASE [{dbName}] SET AUTO_UPDATE_STATISTICS OFF",
        $"ALTER DATABASE [{dbName}] SET CURSOR_CLOSE_ON_COMMIT OFF",
        $"ALTER DATABASE [{dbName}] SET CURSOR_DEFAULT  GLOBAL",
        $"ALTER DATABASE [{dbName}] SET CONCAT_NULL_YIELDS_NULL OFF",
        $"ALTER DATABASE [{dbName}] SET NUMERIC_ROUNDABORT OFF",
        $"ALTER DATABASE [{dbName}] SET QUOTED_IDENTIFIER OFF",
        $"ALTER DATABASE [{dbName}] SET RECURSIVE_TRIGGERS OFF",
        $"ALTER DATABASE [{dbName}] SET DISABLE_BROKER",
        $"ALTER DATABASE [{dbName}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF",
        $"ALTER DATABASE [{dbName}] SET DATE_CORRELATION_OPTIMIZATION OFF",
        $"ALTER DATABASE [{dbName}] SET PARAMETERIZATION SIMPLE",
        $"ALTER DATABASE [{dbName}] SET READ_COMMITTED_SNAPSHOT OFF",
        $"ALTER DATABASE [{dbName}] SET READ_WRITE",
        $"ALTER DATABASE [{dbName}] SET RECOVERY SIMPLE",
        $"ALTER DATABASE [{dbName}] SET MULTI_USER",
        $"ALTER DATABASE [{dbName}] SET PAGE_VERIFY CHECKSUM",
        $"ALTER DATABASE [{dbName}] SET TARGET_RECOVERY_TIME = 60 SECONDS",
        $"ALTER DATABASE [{dbName}] SET DELAYED_DURABILITY = DISABLED"
    };

        return result;
    }

    private static List<string> GenerateDatabasePostCreationCommands(string dbName)
    {
        var result = new List<string>
    {
        $"ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = Off;",
        $"ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = Primary;",
        $"ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;",
        $"ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;",
        $"ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = On;",
        $"ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = Primary;",
        $"ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = Off;",
        $"ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = Primary;",
        $"IF NOT EXISTS(SELECT name FROM sys.filegroups WHERE is_default= 1 AND name = N'PRIMARY') ALTER DATABASE [{dbName}] MODIFY FILEGROUP[PRIMARY] DEFAULT"
    };

        return result;
    }
}