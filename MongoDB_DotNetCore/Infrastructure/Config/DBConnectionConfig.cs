using System;
using System.Collections.Generic;
using System.IO;

//name1space VJCore.Infrastructure.Config;

public class DBConnectionConfig
{
    private DBConnectionConfig() { }

    public string ServerName { get; set; } = @".\SQLExpress2017";
    public string DatabaseNameBase { get; set; } = string.Empty;
    public string UserId { get; set; } = "ElxServer";
    public string Password { get; set; } = "Elx123456";
    public string ServerHost { get; set; } = "127.0.0.1";
    public string DatabaseFolderPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + @"Data\";
    public bool IntegratedSecurity { get; set; } = true;
    public string DatabaseType { get; set; } = "Sql";
    // Can be Sql or NpgSql

    public string RTDBHost { get; set; } = "127.0.0.1";
    public int RTDBPort { get; set; } = 5432;

    private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"DBConnection.conf";

    private static DBConnectionConfig m_inMemoryInstance = null;

    public static DBConnectionConfig ReadFromFileOrCreateNew()
    {
        if (m_inMemoryInstance != null) return m_inMemoryInstance;

        m_inMemoryInstance = ReadFromFileOrCreateNewInternal();
        return m_inMemoryInstance;
    }
    private static DBConnectionConfig ReadFromFileOrCreateNewInternal()
    {
        DBConnectionConfig result = new();

        if (File.Exists(FilePath))
        {
            var lines = File.ReadAllLines(FilePath);
            result.ServerName = lines.Length >= 1 ? lines[0].Trim() : string.Empty;
            result.DatabaseNameBase = lines.Length >= 2 ? lines[1].Trim() : string.Empty;
            result.UserId = lines.Length >= 3 ? lines[2].Trim() : string.Empty;
            result.Password = lines.Length >= 4 ? lines[3].Trim() : string.Empty;
            result.ServerHost = lines.Length >= 5 ? lines[4].Trim() : string.Empty;
            result.DatabaseFolderPath = lines.Length >= 6 ? lines[5].Trim() : string.Empty;
            result.IntegratedSecurity = lines.Length >= 7 ? bool.Parse(lines[6].Trim()) : true;
            result.DatabaseType = lines.Length >= 8 ? lines[7].Trim() : "Sql";
            result.RTDBHost = lines.Length >= 9 ? lines[8].Trim() : "127.0.0.1";
            result.RTDBPort = lines.Length >= 10 ? int.Parse(lines[9].Trim()) : 5433;
        }

        return result;
    }

    public TransactionResult SaveToFile()
    {
        TransactionResult result = new();

        try
        {
            List<string> lines = new()
            {
            ServerName,
            DatabaseNameBase,
            UserId,
            Password,
            ServerHost,
            DatabaseFolderPath,
            IntegratedSecurity.ToString(),
            DatabaseType,
            RTDBHost,
            RTDBPort.ToString()
        };

            File.WriteAllLines(FilePath, lines.ToArray());

            result.Successful = true;

            m_inMemoryInstance = ReadFromFileOrCreateNewInternal();

        }
        catch (Exception ex)
        {
            result.Successful = false;
            result.Message = ex.Message;
        }

        return result;
    }
}