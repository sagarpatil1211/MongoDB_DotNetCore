//using Newtonsoft.Json.Linq;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data.Common;
//using System.Linq;

//public class SMSIntegrator_Msg91_v2 : BaseObject
//{
//    public static string EntityType { get => "SMSIntegrator_Msg91_v2"; }
//    public static string PrimaryKeys { get => "Ref"; }
//    public static string MasterTableName { get => "SMSIntegrator_Msg91_v2_Master"; }
//    public static string RepositoryName { get => MasterTableName; }
//    public static string SingularName { get => "Msg91 v2 SMS Integrator"; }
//    public static string PluralName { get => "Msg91 v2 SMS Integrators"; }

//    public static void RegisterEntityTypeAspects()
//    {
//        EntityTypes.RegisterEntityTypeAspects(EntityType, MasterTableName, GetDomainEntityListFormulator(), 
//            SingularName, PluralName, PrimaryKeys, GetInMemoryTablesRequired());

//        FetchRequestHandlers.RegisterFetchHandler(SMSIntegrator_Msg91_v2FetchRequest.FetchRequestType, 
//            HandleFetchRequest);

//        CustomProcessRequestHandlers.RegisterCustomProcessHandler(TestSMSIntegrationRequest.CustomProcessRequestType, 
//            HandleTestSMSIntegrationRequest);
//    }

//    private static Func<DbCommand, SMSIntegrator_Msg91_v2> NextActiveSMSSettingsGetter = null;

//    public static void RegisterNextActiveSMSSettingsGetter(Func<DbCommand, SMSIntegrator_Msg91_v2> targetMethod)
//    {
//        NextActiveSMSSettingsGetter = targetMethod;
//    }

//    public static SMSIntegrator_Msg91_v2 GetNextActiveSMSSettings(DbCommand cmd)
//    {
//        if (NextActiveSMSSettingsGetter == null) return null;
//        return NextActiveSMSSettingsGetter(cmd);
//    }

//    public static void SendMessagesBySelectingSMSSettings(DbCommand cmd, List<SMSTransmissionRequest> requests)
//    {
//        SMSIntegrator_Msg91_v2 settingsToUse = GetNextActiveSMSSettings(cmd);

//        foreach (SMSTransmissionRequest request in requests)
//        {
//            settingsToUse.TransmitSMS(request.Message, request.PhoneNumbers);
//        }
//    }

//    public static SMSIntegrator_Msg91_v2 GetEntityFromDB(DbCommand cmd, long Ref, bool allowEdit)
//    {
//        var req = new SMSIntegrator_Msg91_v2FetchRequest();
//        req.SMSIntegrator_Msg91_v2Refs.Add(Ref);

//        var lst = GetEntitiesFromDB(cmd, req, allowEdit);
//        if (lst.Count > 0) return lst[0];

//        return null;
//    }

//    private static void HandleFetchRequest(TransportData td)
//    {
//        if (td.MainData.CollectionExists(SMSIntegrator_Msg91_v2FetchRequest.FetchRequestType))
//        {
//            var reqColl = td.MainData.GetCollection(SMSIntegrator_Msg91_v2FetchRequest.FetchRequestType);
//            reqColl.Entries.ForEach(obj =>
//            {
//                var req = SMSIntegrator_Msg91_v2FetchRequest.FromJsonObject(obj);

//                var cmd = SessionController.DAU.CreateCommand();

//                var lst = GetEntitiesFromDB(cmd, req, false);

//                foreach (var entity in lst)
//                {
//                    entity.ParentDataContainer.CopyEntireContainerTo(td.MainData);
//                    break; // The parent container of the first entity will contain all the data
//                }
//            });
//        }
//    }

//    private static void HandleTestSMSIntegrationRequest(TransportData td)
//    {
//        if (td.MainData.CollectionExists(TestSMSIntegrationRequest.CustomProcessRequestType))
//        {
//            var reqColl = td.MainData.GetCollection(TestSMSIntegrationRequest.CustomProcessRequestType);
//            reqColl.Entries.ForEach(obj =>
//            {
//                var req = TestSMSIntegrationRequest.FromJsonObject(obj);

//                SMSIntegrator_Msg91_v2 smsi = CreateNewInstance();
//                smsi.Sender = req.Sender;
//                smsi.APIKey = req.APIKey;
//                smsi.Country = req.Country;
//                smsi.EnableSMSTransmission = true;
//                smsi.Route = req.Route;

//                smsi.TransmitSMS(req.Message, req.PhoneNumbers);
//            });
//        }
//    }

//    public static List<SMSIntegrator_Msg91_v2> GetEntitiesFromDB(DbCommand cmd, SMSIntegrator_Msg91_v2FetchRequest reqParams, 
//        bool allowEdit)
//    {
//        var result = new List<SMSIntegrator_Msg91_v2>();

//        var dau = SessionController.DAU;

//        var dc = new DataContainer();

//        var coord = CommandFormulationCoordinator.CreateInstance(MasterTableName)
//            .CoordinateLongValues("Ref", reqParams.SMSIntegrator_Msg91_v2Refs)
//            .FormulateCommandStringAndParameters(cmd);

//        dau.FillByQueryAndTableName(cmd, dc, MasterTableName);

//        if (dc.CollectionExists(MasterTableName))
//        {
//            var coll = dc.GetCollection(MasterTableName);
//            coll.Entries.ForEach(e =>
//            {
//                var entity = CreateInstance(e, dc, allowEdit);
//                result.Add(entity);
//            });
//        }

//        return result;
//    }

//    private static Dictionary<string, string> GetInMemoryTablesRequired()
//    {
//        return new Dictionary<string, string>();
//    }

//    private SMSIntegrator_Msg91_v2(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
//        : base(data, allowEdit, parentContainer, allowInPlaceEditing)
//    {

//    }

//    public static SMSIntegrator_Msg91_v2 CreateInstance(JObject data, DataContainer parentContainer, 
//        bool allowEdit = false, bool allowInPlaceEditing = false)
//    {
//        return new SMSIntegrator_Msg91_v2(data, parentContainer, allowEdit, allowInPlaceEditing);
//    }

//    public static SMSIntegrator_Msg91_v2 CreateNewInstance()
//    {
//        var data = SessionController.DAU.GetBlankDataObjectFromTable(MasterTableName);
//        return CreateInstance(data, new DataContainer(), true, false);
//    }

//    public SMSIntegrator_Msg91_v2 GetEditableVersion()
//    {
//        return new SMSIntegrator_Msg91_v2(BaseData, new DataContainer(), true);
//    }

//    public long Ref
//    {
//        get => GetLongPropertyValue("Ref");
//        set => SetLongPropertyValue("Ref", value);
//    }

//    public string Sender
//    {
//        get => GetStringPropertyValue("Sender");
//        set => SetStringPropertyValue("Sender", value);
//    }

//    public string Route
//    {
//        get => GetStringPropertyValue("Route");
//        set => SetStringPropertyValue("Route", value);
//    }

//    public string Country
//    {
//        get => GetStringPropertyValue("Country");
//        set => SetStringPropertyValue("Country", value);
//    }

//    public string APIKey
//    {
//        get => GetStringPropertyValue("APIKey");
//        set => SetStringPropertyValue("APIKey", value);
//    }

//    public bool EnableSMSTransmission
//    {
//        get => GetBooleanPropertyValue("EnableSMSTransmission");
//        set => SetBooleanPropertyValue("EnableSMSTransmission", value);
//    }

//    public static Func<TransportData, Func<JObject, bool>, Func<JObject, string>, bool, bool, List<BaseObject>> GetDomainEntityListFormulator()
//    {
//        return (td, filterFunction, sortFunction, allowEdit, allowInPlaceEditing) =>
//        {
//            var coll = td.MainData.GetCollection(MasterTableName);

//            var result = new List<BaseObject>();

//            var collTemp = coll.Entries as IEnumerable<JObject>;
//            if (filterFunction != null) collTemp = collTemp.Where(filterFunction);
//            if (sortFunction != null) collTemp = collTemp.OrderBy(sortFunction);

//            foreach (JObject data in collTemp)
//            {
//                result.Add(new SMSIntegrator_Msg91_v2(data, td.MainData, allowEdit, allowInPlaceEditing));
//            }

//            return result;
//        };
//    }

//    protected override void SaveValidator(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
//    {
//        if (Sender.Trim().Length == 0) vrec.AddToList(string.Empty, "Sender ID cannot be blank.");
//        if (Route.Trim().Length == 0) vrec.AddToList(string.Empty, "Route cannot be blank.");
//        if (Country.Trim().Length == 0) vrec.AddToList(string.Empty, "Country cannot be blank.");
//        if (APIKey.Trim().Length == 0) vrec.AddToList(string.Empty, "API Key cannot be blank.");
//    }

//    public override void PerformPreSaveValidationProcess(DbCommand cmd, TransportData td, 
//        ValidationResultEntryCollection validationResultEntries)
//    {

//    }

//    public override void PerformPreSaveProcess(DbCommand cmd, TransportData td, 
//        ValidationResultEntryCollection validationResultEntries)
//    {

//    }

//    public override void InvokePostSaveProcess(DbCommand cmd, TransportData tdContext, 
//        ValidationResultEntryCollection vrec)
//    {

//    }

//    protected override List<BaseObject> DependentEntityGenerator(DbCommand cmd, TransportData td, 
//        ValidationResultEntryCollection validationResultEntries)
//    {
//        return new List<BaseObject>();
//    }

//    public void TransmitSMS(string message, string phoneNumbers)
//    {
//        if (!EnableSMSTransmission) return;

//        if (message.Length == 0) throw new DomainException("SMS Message cannot be blank.");
//        string messageToSend = Uri.EscapeDataString(message);

//        if (phoneNumbers.Length == 0) throw new DomainException("Phone Number(s) not specified.");

//        List<string> lstPhoneNumbers = phoneNumbers.Split(',', StringSplitOptions.RemoveEmptyEntries)
//                .Where(n => n.Trim().Length > 0).ToList();

//        List<List<string>> lstChunks = new List<List<string>>();

//        int maxPhoneNumbersPerChunk = 10;

//        for (int i = 0; i < lstPhoneNumbers.Count(); i += maxPhoneNumbersPerChunk)
//        {
//            lstChunks.Add(lstPhoneNumbers.Skip(i).Take(maxPhoneNumbersPerChunk).ToList());
//        }

//        List<string> lstURLs = new List<string>();

//        foreach(List<string> chunk in lstChunks)
//        {
//            string phoneNumbersInChunk = string.Empty;

//            foreach(string pNo in chunk)
//            {
//                if (phoneNumbersInChunk.Length > 0) phoneNumbersInChunk += ",";
//                phoneNumbersInChunk += pNo;
//            }

//            lstURLs.Add($"api.msg91.com/api/v2/sendsms?authkey={APIKey}&mobiles={phoneNumbersInChunk}&country={Country}&message={messageToSend}&sender={Sender}&route={Route}");
//        }

//        BackgroundWorker bw = new BackgroundWorker();
//        bw.DoWork += (s, ev) =>
//        {
//            List<string> urls = ev.Argument as List<string>;

//            foreach (var urlString in lstURLs)
//            {
//                try
//                {
//                    var client = new RestClient($"https://{urlString}");
//                    var request = new RestRequest(Method.Get.ToString());
//                    client.Execute(request);
//                }
//                catch (Exception)
//                {
//                    // Just swallow it for now
//                }
//            }
//        };

//        bw.RunWorkerAsync(lstURLs);
//    }
//}
