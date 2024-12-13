using System;
using System.IO;

//name1space VJCore.Infrastructure.Config;

public class ServerHostingConfig
{
    public static string BaseUrl { get; private set; } = string.Empty;
    public static string TimeServerUrl { get; private set; } = string.Empty;
    public static string MessageHubUrl { get; private set; } = string.Empty;
    public static string ClientLoginURL { get; private set; } = string.Empty;

    public static void ReadFromFile()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerHostingConfig.cnfg");
        if (!File.Exists(filePath)) throw new DomainException("Server hosting configuration file not found.");

        var lines = File.ReadAllLines(filePath);
        if (lines.Length < 1) throw new DomainException("Server hosting configuration file is not properly formed.");

        BaseUrl = lines[0];

        if (lines.Length > 1)
        {
            TimeServerUrl = lines[1];
        }
        else
        {
            TimeServerUrl = BaseUrl;
        }

        if (lines.Length > 2)
        {
            MessageHubUrl = lines[2];
        }
        else
        {
            MessageHubUrl = BaseUrl;
        }

        if (lines.Length > 3)
        {
            ClientLoginURL = lines[3];
        }
        else
        {
            ClientLoginURL = string.Empty;
        }
    }
}