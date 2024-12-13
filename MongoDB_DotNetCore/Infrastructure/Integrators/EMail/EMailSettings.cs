using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Agreement;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
//using VJ1Core.Infrastructure.DbConnectionObjects;
//using VJ1Core.Infrastructure.Main;

public class EMailSettings : BaseObject
{
    public long Ref
    {
        get => GetLongPropertyValue("Ref");
        set => SetLongPropertyValue("Ref", value);
    }

    public string SenderFriendlyName
    {
        get => GetStringPropertyValue("SenderFriendlyName");
        set => SetStringPropertyValue("SenderFriendlyName", value);
    }

    public string EMailID
    {
        get => GetStringPropertyValue("EMailID");
        set => SetStringPropertyValue("EMailID", value);
    }

    public string SMTPHost
    {
        get => GetStringPropertyValue("SMTPHost");
        set => SetStringPropertyValue("SMTPHost", value);
    }

    public int SMTPPort
    {
        get => GetIntegerPropertyValue("SMTPPort");
        set => SetIntegerPropertyValue("SMTPPort", value);
    }

    public bool SSLRequired
    {
        get => GetBooleanPropertyValue("SSLRequired");
        set => SetBooleanPropertyValue("SSLRequired", value);
    }

    public string UserName
    {
        get => GetStringPropertyValue("UserName");
        set => SetStringPropertyValue("UserName", value);
    }

    public string Password
    {
        get => GetStringPropertyValue("Password");
        set => SetStringPropertyValue("Password", value);
    }

    public bool EnableEMailTransmission
    {
        get => GetBooleanPropertyValue("EnableEMailTransmission");
        set => SetBooleanPropertyValue("EnableEMailTransmission", value);
    }

    public int DailySendingQuota
    {
        get => GetIntegerPropertyValue("DailySendingQuota");
        set => SetIntegerPropertyValue("DailySendingQuota", value);
    }

    public EMailSSLConnectionOptions SSLConnectionOption
    {
        get => (EMailSSLConnectionOptions)GetIntegerPropertyValue("SSLConnectionOption");
        set => SetIntegerPropertyValue("SSLConnectionOption", Convert.ToInt32(value));
    }

    #region "Infrastructure"
    public static Func<TransportData, Func<JObject, bool>, Func<JObject, string>, bool, bool, List<BaseObject>> GetDomainEntityListFormulator()
    {
        return (td, filterFunction, sortFunction, allowEdit, allowInPlaceEditing) =>
        {
            var coll = td.MainData.GetCollection(MasterTableName);

            var result = new List<BaseObject>();

            var collTemp = coll.Entries as IEnumerable<JObject>;
            if (filterFunction != null) collTemp = collTemp.Where(filterFunction);
            if (sortFunction != null) collTemp = collTemp.OrderBy(sortFunction);

            foreach (JObject data in collTemp)
            {
                result.Add(new EMailSettings(data, td.MainData, allowEdit, allowInPlaceEditing));
            }

            return result;
        };
    }

    protected override void SaveValidator(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        
    }

    public override void PerformPreSaveValidationProcess(DbCommand cmd, TransportData td, ValidationResultEntryCollection validationResultEntries)
    {

    }

    public override void PerformPreSaveProcess(DbCommand cmd, TransportData td, ValidationResultEntryCollection validationResultEntries)
    {

    }

    public override void InvokePostSaveProcess(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec)
    {

    }

    protected override List<BaseObject> DependentEntityGenerator(DbCommand cmd, TransportData td, ValidationResultEntryCollection validationResultEntries)
    {
        return new List<BaseObject>();
    }

    private static Func<DbCommand, EMailSettings> NextActiveEMailSettingsGetter = null;

    public static void RegisterNextActiveEMailSettingsGetter(Func<DbCommand, EMailSettings> targetMethod)
    {
        NextActiveEMailSettingsGetter = targetMethod;
    }

    public static EMailSettings GetNextActiveEMailSettings(DbCommand cmd)
    {
        if (NextActiveEMailSettingsGetter == null) return null;
        return NextActiveEMailSettingsGetter(cmd);
    }

    public static string EntityType { get => "EMailSettings"; }

    public static string PrimaryKeys { get => "Ref"; }
    public static string MasterTableName { get => "EMailSettingsMaster"; }
    public static string RepositoryName { get => MasterTableName; }
    public static string SingularName { get => "EMail Settings"; }
    public static string PluralName { get => "EMail Settings"; }

    public static void RegisterEntityTypeAspects()
    {
        EntityTypes.RegisterEntityTypeAspects(EntityType, MasterTableName, 
            GetDomainEntityListFormulator(), SingularName, PluralName, PrimaryKeys,
            CreateInstance, CreateNewInstance,
            GetInMemoryTablesRequired());

        FetchRequestHandlers.RegisterFetchHandler(EMailSettingsFetchRequest.FetchRequestType, HandleFetchRequest);

        CustomProcessRequestHandlers.RegisterCustomProcessHandler(TestEMailIntegrationRequest.CustomProcessRequestType, HandleTestEMailIntegrationRequest);
    }

    public static EMailSettings GetEntityFromDB(DbCommand cmd, long Ref, bool allowEdit)
    {
        var req = new EMailSettingsFetchRequest();
        req.EMailSettingsRefs.Add(Ref);

        var lst = GetEntitiesFromDB(cmd, req, allowEdit);
        if (lst.Count > 0) return lst[0];

        return null;
    }

    private static void HandleTestEMailIntegrationRequest(TransportData td)
    {
        if (td.MainData.CollectionExists(TestEMailIntegrationRequest.CustomProcessRequestType))
        {
            var reqColl = td.MainData.GetCollection(TestEMailIntegrationRequest.CustomProcessRequestType);
            reqColl.Entries.ForEach(obj =>
            {
                var req = TestEMailIntegrationRequest.FromJsonObject(obj);

                EMailSettings ems = CreateNewInstance();
                ems.EMailID = req.EMailID;
                ems.EnableEMailTransmission = true;
                ems.Password = req.Password;
                ems.SenderFriendlyName = req.SenderFriendlyName;
                ems.SMTPHost = req.SMTPHost;
                ems.SMTPPort = req.SMTPPort;
                ems.SSLRequired = req.SSLRequired;
                ems.SSLConnectionOption = req.SSLConnectionOption;
                ems.UserName = req.UserName;

                EMailTransmissionRequest etr = new EMailTransmissionRequest();

                etr.Receivers.Add(req.SendToEMailId, req.SendToEMailId);
                etr.Subject = $"Test EMail";

                etr.TextBody = "Test EMail";

                EMailTransmitter.SendMailSync(ems, etr);
            });
        }
    }

    private static void HandleFetchRequest(TransportData td)
    {
        if (td.MainData.CollectionExists(EMailSettingsFetchRequest.FetchRequestType))
        {
            var reqColl = td.MainData.GetCollection(EMailSettingsFetchRequest.FetchRequestType);
            reqColl.Entries.ForEach(obj =>
            {
                var req = EMailSettingsFetchRequest.FromJsonObject(obj);

                var cmd = SessionController.DAU.CreateCommand();

                var lst = GetEntitiesFromDB(cmd, req, false);

                foreach (var entity in lst)
                {
                    entity.ParentDataContainer.CopyEntireContainerTo(td.MainData);
                    break; // The parent container of the first entity will contain all the data
                }
            });
        }
    }

    public static List<EMailSettings> GetEntitiesFromDB(DbCommand cmd, EMailSettingsFetchRequest reqParams, bool allowEdit)
    {
        var result = new List<EMailSettings>();

        var dau = SessionController.DAU;

        var dc = new DataContainer();

        var coord = CommandFormulationCoordinator.CreateInstance(MasterTableName)
            .CoordinateLongValues("Ref", reqParams.EMailSettingsRefs)
            .FormulateCommandStringAndParameters(cmd);

        dau.FillByQueryAndTableName(cmd, dc, MasterTableName);

        //List<string> lstFilterStrings = new List<string>();

        //if (reqParams.EMailSettingsRefs.Count > 0) lstFilterStrings.Add($"Ref IN ({reqParams.EMailSettingsRefsString})");

        //string strCommand = Utils.FormulateQueryStringWithFilters($"SELECT * FROM {MasterTableName}", lstFilterStrings);

        //dau.FillByQuery(cmd, dc, MasterTableName, strCommand);

        if (dc.CollectionExists(MasterTableName))
        {
            var coll = dc.GetCollection(MasterTableName);
            coll.Entries.ForEach(e =>
            {
                var entity = EMailSettings.CreateInstance(e, dc, allowEdit);
                result.Add(entity);
            });
        }

        return result;
    }

    private static Dictionary<string, string> GetInMemoryTablesRequired()
    {
        return new Dictionary<string, string>();
    }

    private EMailSettings(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
        : base(data, allowEdit, parentContainer, allowInPlaceEditing)
    {

    }

    public static EMailSettings CreateInstance(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
    {
        return new EMailSettings(data, parentContainer, allowEdit, allowInPlaceEditing);
    }

    public static EMailSettings CreateNewInstance()
    {
        var data = SessionController.DAU.GetBlankDataObjectFromTable(MasterTableName);
        return CreateInstance(data, new DataContainer(), true);
    }

    public EMailSettings GetEditableVersion()
    {
        return new EMailSettings(BaseData, new DataContainer(), true);
    }
    #endregion
}
