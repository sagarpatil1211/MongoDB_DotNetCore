using System;
using System.Collections.Generic;
using System.Text;
//using VJ1Core.Infrastructure.Main;

public class Topics
{
    public static readonly string ClientToServerRequest = "ClientToServerRequest";
    public static readonly string ServerToClientResponse = "ServerToClientResponse";
    
    private static readonly string m_CacheDataTopic = "CacheData";
    private static readonly string m_CacheDataChangeLevelTopic = "CacheDataChangeLevel";

    public static string CacheDataTopic(string masterTableName)
    {
        return $"{SessionController.CustomerLicenseNo}/{m_CacheDataTopic}/{masterTableName}";
    }

    public static string CacheDataChangeLevelTopic(string masterTableName)
    {
        return $"{SessionController.CustomerLicenseNo}/{m_CacheDataChangeLevelTopic}/{masterTableName}";
    }
}
