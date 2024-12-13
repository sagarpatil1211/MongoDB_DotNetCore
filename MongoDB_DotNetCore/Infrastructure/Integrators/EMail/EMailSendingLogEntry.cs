using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using MimeKit.Cryptography;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
//using VJ1Core.Infrastructure.DbConnectionObjects;
//using VJ1Core.Infrastructure.Main;

public class EMailSendingLogEntry : BaseObject
{
    public string TransDate
    {
        get => GetStringPropertyValue("TransDate");
        set => SetStringPropertyValue("TransDate", value);
    }

    public string SenderEMailId
    {
        get => GetStringPropertyValue("SenderEMailId");
        set => SetStringPropertyValue("SenderEMailId", value);
    }

    public int SentCount
    {
        get => GetIntegerPropertyValue("SentCount");
        set => SetIntegerPropertyValue("SentCount", value);
    }

    #region "Infrastructure"
    public EMailSendingLogEntry GetEditableVersion()
    {
        return new EMailSendingLogEntry(new JObject(BaseData), new DataContainer(), true);
    }

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
                result.Add(new EMailSendingLogEntry(data, td.MainData, allowEdit, allowInPlaceEditing));
            }

            return result;
        };
    }

    protected override void SaveValidator(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
    {
        
    }

    public static EMailSendingLogEntry CreateNewInstance()
    {
        var data = SessionController.DAU.GetBlankDataObjectFromTable(MasterTableName);
        return CreateInstance(data, new DataContainer(), true);
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

    public static string EntityType { get => "EMailSendingLogEntry"; }
    public static string PrimaryKeys { get => "TransDate, SenderEMailId"; }
    public static string MasterTableName { get => "EMailSendingLog"; }
    public static string RepositoryName { get => MasterTableName; }
    public static string SingularName { get => "EMail Sending Log Entry"; }
    public static string PluralName { get => "EMail Sending Log Entries"; }

    public static void RegisterEntityTypeAspects()
    {
        EntityTypes.RegisterEntityTypeAspects(EntityType, MasterTableName, 
            GetDomainEntityListFormulator(), SingularName, PluralName, PrimaryKeys,
            CreateInstance, CreateNewInstance,
            GetInMemoryTablesRequired());

        EntityConstraintsManager.RegisterUniqueConstraint(EntityType, "1",
            "Duplicate EMail Sending Log Entries for the same date and email id are not permitted.", "TransDate", "EMailId");

        FetchRequestHandlers.RegisterFetchHandler(EMailSendingLogEntryFetchRequest.FetchRequestType, HandleFetchRequest);
    }

    public static EMailSendingLogEntry GetEntityFromDB(DbCommand cmd, string transDate, string emailId, bool allowEdit)
    {
        var req = new EMailSendingLogEntryFetchRequest();
        req.TransDates.Add(transDate);
        req.EMailIds.Add(emailId);

        var lst = GetEntitiesFromDB(cmd, req, allowEdit);
        if (lst.Count > 0) return lst[0];

        return null;
    }

    private static void HandleFetchRequest(TransportData td)
    {
        if (td.MainData.CollectionExists(EMailSendingLogEntryFetchRequest.FetchRequestType))
        {
            var reqColl = td.MainData.GetCollection(EMailSendingLogEntryFetchRequest.FetchRequestType);
            reqColl.Entries.ForEach(obj =>
            {
                var req = EMailSendingLogEntryFetchRequest.FromJsonObject(obj);

                var cmd = SessionController.DAU.CreateCommand();

                var lst = GetEntitiesFromDB(cmd, req, false);

                EMailSettingsFetchRequest reqEMS = new EMailSettingsFetchRequest();

                foreach (var entity in lst)
                {
                    entity.MergeIntoTransportData(td);
                }
            });
        }
    }

    public static List<EMailSendingLogEntry> GetEntitiesFromDB(DbCommand cmd, EMailSendingLogEntryFetchRequest reqParams, bool allowEdit)
    {
        var result = new List<EMailSendingLogEntry>();

        var dau = SessionController.DAU;

        var dc = new DataContainer();

        var coord = CommandFormulationCoordinator.CreateInstance(MasterTableName);
        if (reqParams.TransDates.Count > 0) coord.CoordinateVarcharValues("TransDate", 256, reqParams.TransDates);
        if (reqParams.FromDate.Trim().Length > 0) coord.AddFilterString("TransDate >= @FromDate").AddVarcharParameter("@FromDate", 256, reqParams.FromDate);
        if (reqParams.ToDate.Trim().Length > 0) coord.AddFilterString("TransDate <= @ToDate").AddVarcharParameter("@ToDate", 256, reqParams.ToDate);
        if (reqParams.EMailIds.Count > 0) coord.CoordinateVarcharValues("EMailId", 1024, reqParams.EMailIds);

        coord.FormulateCommandStringAndParameters(cmd);

        if (cmd.Parameters.Count <= 0) throw new DomainException($"No filters supplied for query.");

        dau.FillByQueryAndTableName(cmd, dc, MasterTableName);

        //List<string> lstFilterStrings = new List<string>();

        //if (reqParams.TransDates.Count > 0) lstFilterStrings.Add($"TransDate IN ({reqParams.TransDatesString})");
        //if (reqParams.FromDate.Trim().Length > 0) lstFilterStrings.Add($"TransDate >= {reqParams.FromDate}");
        //if (reqParams.ToDate.Trim().Length > 0) lstFilterStrings.Add($"TransDate <= {reqParams.ToDate}");
        //if (reqParams.EMailIds.Count > 0) lstFilterStrings.Add($"EMailId IN ({reqParams.EMailIdsString})");

        //string strCommand = Utils.FormulateQueryStringWithFilters($"SELECT * FROM {MasterTableName}", lstFilterStrings);

        //dau.FillByQuery(cmd, dc, MasterTableName, strCommand);

        if (dc.CollectionExists(MasterTableName))
        {
            var coll = dc.GetCollection(MasterTableName);
            coll.Entries.ForEach(e =>
            {
                var entity = CreateInstance(e, dc, allowEdit);
                result.Add(entity);
            });
        }

        return result;
    }

    private static Dictionary<string, string> GetInMemoryTablesRequired()
    {
        return new Dictionary<string, string>();
    }

    private EMailSendingLogEntry(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
        : base(data, allowEdit, parentContainer, allowInPlaceEditing)
    {

    }

    public static EMailSendingLogEntry CreateInstance(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
    {
        return new EMailSendingLogEntry(data, parentContainer, allowEdit, allowInPlaceEditing);
    }
    #endregion
}
