using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
//using VJ1Core.Domain.User_Management;
//using VJ1Core.Infrastructure.Config;

//name1space VJCore.Infrastructure;

public sealed class SessionController
{
    public static string ControllerName { get; private set; } = string.Empty;
    public static int CustomerLicenseNo { get; private set; } = 0;
    public static string CustomerCode { get; private set; } = string.Empty;
    public static string CustomerName { get; private set; } = string.Empty;
    public static bool IsInDevelopmentMode { get; private set; } = false;
    public static string ProjectName { get; private set; } = string.Empty;
    public static bool CombinedCacheDataBroadcast { get; private set; } = false;

    private static void InitializeDomains()
    {
        var assemblies = AssemblyManagement.GetAllAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                var ifaces = t.GetInterfaces();

                if (ifaces.Any(iface => iface.Name.Contains("IDomainInitializer")))
                {
                    var initializer = (IDomainInitializer)Activator.CreateInstance(t);
                    initializer.InitializeDomain();
                }
            }
        }
    }

    private static void InvokeDomainSpecificPostInitializationHandlers()
    {
        var assemblies = AssemblyManagement.GetAllAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                var ifaces = t.GetInterfaces();

                if (ifaces.Any(iface => iface.Name.Contains("IDomainSpecificPostInitializationHandler")))
                {
                    var handler = (IDomainSpecificPostInitializationHandler)Activator.CreateInstance(t);
                    handler.InvokeProcesses();
                }
            }
        }
    }

    public static void PostDomainSpecificCacheDataToMessageHub()
    {
        var assemblies = AssemblyManagement.GetAllAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                var ifaces = t.GetInterfaces();

                if (ifaces.Any(iface => iface.Name.Contains("IDomainSpecificPostInitializationHandler")))
                {
                    var handler = (IDomainSpecificPostInitializationHandler)Activator.CreateInstance(t);
                    handler.PostCacheDataToMessageHub();
                }
            }
        }
    }

    private static void InitializeModules()
    {
        string appPath = AppDomain.CurrentDomain.BaseDirectory;

        var filePaths = Directory.GetFiles(appPath);

        foreach (string filePath in filePaths)
        {
            string fName = Path.GetFileName(filePath);

            if (fName.StartsWith(CustomerCode) && fName.Contains("_Module"))
            {
                var assy = Assembly.LoadFrom(filePath);

                foreach (Type t in assy.GetTypes())
                {
                    var ifaces = t.GetInterfaces();

                    if (ifaces.Any(iface => iface.Name.Contains("IModuleInterface")))
                    {
                        var initializer = (IModuleInterface)Activator.CreateInstance(t);
                        initializer.InitializeModule();
                    }
                }
            }
        }
    }

    private static string GetStringValueInLicenseInfoLine(string line)
    {
        var parts = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2) return parts[1];
        return string.Empty;
    }

    public static void ReadAndSetUplodedFilesFolder()
    {
        var fPath = $"{AppDomain.CurrentDomain.BaseDirectory}UploadedFilesFolderConfig.cnf";

        if (!File.Exists(fPath)) throw new DomainException("Uploaded Files Folder Configuration file not found. Cannot start the system.");

        var lines = File.ReadAllLines(fPath);

        if (lines.Length != 1) throw new DomainException("License Information file is not properly formed.");

        UploadedFilesFolder = lines[0].Trim();
    }

    public static void ReadControllerNameAndCustomerLicenseNo()
    {
        var fPath = $"{AppDomain.CurrentDomain.BaseDirectory}LicenseInfo.txt";

        if (!File.Exists(fPath)) throw new DomainException("License Information file not found. Cannot start the system.");

        var lines = File.ReadAllLines(fPath);

        if (lines.Length != 8) throw new DomainException("License Information file is not properly formed.");

        foreach (var line in lines)
        {
            if (line.StartsWith("ControllerName:")) ControllerName = GetStringValueInLicenseInfoLine(line);
            if (line.StartsWith("CustomerLicenseNo:")) CustomerLicenseNo = int.Parse(GetStringValueInLicenseInfoLine(line));
            if (line.StartsWith("CustomerCode:")) CustomerCode = GetStringValueInLicenseInfoLine(line);
            if (line.StartsWith("CustomerName:")) CustomerName = GetStringValueInLicenseInfoLine(line);
        }

        if (ControllerName.Trim().Length == 0) throw new DomainException("Controller Name not set in the License Configuration file.");
        if (CustomerLicenseNo == 0) throw new DomainException("Customer License Number not set in the License Configuration file.");
        if (CustomerCode.Trim().Length == 0) throw new DomainException("Customer Code not set in the License Configuration file.");
        if (CustomerName.Trim().Length == 0) throw new DomainException("Customer Name not set in the License Configuration file.");

        var assemblies = AssemblyManagement.GetAllAssemblies();

        foreach (Assembly assy in assemblies)
        {
            foreach (Type t in assy.GetTypes())
            {
                var ifaces = t.GetInterfaces();

                if (ifaces.Any(iface => iface.Name.Contains("IProjectDescriptor")))
                {
                    var descriptor = (IProjectDescriptor)Activator.CreateInstance(t);
                    ProjectName = descriptor.ProjectName;
                    CombinedCacheDataBroadcast = descriptor.CombinedCacheDataBroadcast;
                }
            }
        }
    }

    public static DataAccessUtils DAU { get; private set; } = null;

    public static DataInterfaceS DIS { get; private set; } = null;
    //public static SystemUserOperationsManager SUOM { get; private set; } = null;

    public static string UploadedFilesFolder { get; private set; } = null;

    private static ActorSystem m_actorSystem = null;

    public static ActorSystem ActorSystem
    {
        get
        {
            return m_actorSystem;
        }
        private set
        {
            m_actorSystem = value;
        }
    }

    public static void InstantiateActorSystem()
    {
        m_actorSystem ??= ActorSystem.Create("CentralActorSystem");
    }

    public static void StopActorSystem()
    {
        m_actorSystem?.Terminate().GetAwaiter().GetResult();
    }

    private static MessageHubConnection m_hubConnection = null;

    public static MessageHubConnection HubConnection
    {
        get
        {
            return m_hubConnection;
        }
        private set
        {
            m_hubConnection = value;
        }
    }

    private static void InstantiateMessageHubConnection()
    {
        m_hubConnection ??= new MessageHubConnection(ServerHostingConfig.MessageHubUrl, MessageHub.HubName);
    }

    private static void ConnectHubConnection()
    {
        m_hubConnection?.Connect();
    }

    private static void DisconnectHubConnection()
    {
        m_hubConnection?.Disconnect();
    }

    private static int m_processLockThreadId = 0;

    private static object ProcessLock { get; } = new object();

    public static void ObtainProcessLock()
    {
        Monitor.Enter(ProcessLock);
        m_processLockThreadId = Environment.CurrentManagedThreadId;

        EnableSQLCommandPerformanceLogging = File.Exists(SQLCommandPerformanceLoggingEnablerFlagFilePath);
        EnableHTTPRequestPerformanceLogging = File.Exists(HTTPRequestPerformanceLoggingEnablerFlagFilePath);
    }

    public static void ReleaseProcessLock()
    {
        int releasingThreadId = Environment.CurrentManagedThreadId;

        try
        {
            if (m_processLockThreadId != releasingThreadId) throw new DomainException("Process Lock Thread Ids do not match.");
        }
        catch (Exception ex)
        {
            List<string> errors = new()
        {
            string.Empty,
            $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}\n",
            $"Locking Thread Id : {m_processLockThreadId}\n",
            $"Releasing Thread Id : {releasingThreadId}\n",
            $"Call Stack : {ex.StackTrace}\n"
        };

            File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}LockReleaseErrors.txt", errors);
        }
        finally
        {
            Monitor.Exit(ProcessLock);
        }
    }

    private static bool m_initialized = false;

    public static Action<DbCommand> PostInitializationProcess = null;

    public static bool EnableSQLCommandPerformanceLogging = false;
    public static bool EnableHTTPRequestPerformanceLogging = false;

    public static string SQLCommandPerformanceLogFilesDirectory = string.Empty;
    public static string HTTPRequestPerformanceLogFilesDirectory = string.Empty;

    public static string SQLCommandPerformanceLogFilePath
    {
        get
        {
            DateTime dtNow = DateTime.Now;
            string fileName = $"SQLCPL_{dtNow.Year:0000}_{dtNow.Month:00}_{dtNow.Day:00}_{dtNow.Hour:00}_{dtNow.Minute:00}.txt";
            return Path.Combine(SQLCommandPerformanceLogFilesDirectory, fileName);
        }
    }

    public static string HTTPRequestPerformanceLogFilePath
    {
        get
        {
            DateTime dtNow = DateTime.Now;
            string fileName = $"HTTPRPL_{dtNow.Year:0000}_{dtNow.Month:00}_{dtNow.Day:00}_{dtNow.Hour:00}_{dtNow.Minute:00}.txt";
            return Path.Combine(HTTPRequestPerformanceLogFilesDirectory, fileName);
        }
    }

    private static string SQLCommandPerformanceLoggingEnablerFlagFilePath = string.Empty;
    private static string HTTPRequestPerformanceLoggingEnablerFlagFilePath = string.Empty;

    public static void Initialize(bool isInDevelopmentMode)
    {
        if (m_initialized) return;

        IsInDevelopmentMode = isInDevelopmentMode;

        ServerHostingConfig.ReadFromFile();

        ReadControllerNameAndCustomerLicenseNo();
        ReadAndSetUplodedFilesFolder();

        DAU = new DataAccessUtils();
        DAU.InstantiateDBConnection();

        InitializeDomains();

        EntityTypes.EnsurePersistedEnumStringsSynchronizationInDB();

        DIS = new DataInterfaceS();

        //SystemUserOperationsManager.GenerateAndStoreSystemUserIfNonExistent();

        InstantiateMessageHubConnection();

        InstantiateActorSystem();

        string m_SQLCommandPerformanceLogFilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQLCommandPerformanceLogs");

        if (!Directory.Exists(m_SQLCommandPerformanceLogFilesDirectory))
        {
            Directory.CreateDirectory(m_SQLCommandPerformanceLogFilesDirectory);
        }

        SQLCommandPerformanceLogFilesDirectory = m_SQLCommandPerformanceLogFilesDirectory;
        SQLCommandPerformanceLoggingEnablerFlagFilePath = Path.Combine(SQLCommandPerformanceLogFilesDirectory, "EnableSQLCommandPerformanceLogging.txt");

        string m_HTTPRequestPerformanceLogFilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HTTPRequestPerformanceLogs");

        if (!Directory.Exists(m_HTTPRequestPerformanceLogFilesDirectory))
        {
            Directory.CreateDirectory(m_HTTPRequestPerformanceLogFilesDirectory);
        }

        HTTPRequestPerformanceLogFilesDirectory = m_HTTPRequestPerformanceLogFilesDirectory;
        HTTPRequestPerformanceLoggingEnablerFlagFilePath = Path.Combine(HTTPRequestPerformanceLogFilesDirectory, "EnableHTTPRequestPerformanceLogging.txt");
    }

    public static void PerformPostStartupProcesses()
    {
        //ConnectHubConnection();

        //TimeServerMonitor.InstantiateTimeServerMonitor();

        InvokeDomainSpecificPostInitializationHandlers();
        InitializeModules();

        DIS.TransmitCacheData();

        PostDomainSpecificCacheDataToMessageHub();

        //HubConnection.PublishString($"{CustomerCode}/ProductName", ControllerName, true);

        m_initialized = true;
    }

    public static bool IsInitialized()
    {
        return m_initialized;
    }

    public static void Shutdown()
    {
        if (!m_initialized) return;

        StopActorSystem();
        DisconnectHubConnection();
    }
}