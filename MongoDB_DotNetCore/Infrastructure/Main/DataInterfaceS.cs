using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using VJCore.Infrastructure.Extensions;

public class DataInterfaceS
{
    private readonly DataAccessUtils DAU = SessionController.DAU;

    private Dictionary<string, string> InMemoryDataQueries = null;

    public DataInterfaceS()
    {
        FormulateInMemoryDataQueries();
        FillInMemoryData(DAU.CreateCommand());

        RegisterHandlers();
    }

    public void TransmitCacheData()
    {
        
    }

    private void FormulateInMemoryDataQueries()
    {
        InMemoryDataQueries = EntityTypes.GetInMemoryTablesRequired();
    }

    private string GetInMemoryDataQuery(string key)
    {
        if (!InMemoryDataQueries.ContainsKey(key)) return string.Empty;

        string result = InMemoryDataQueries[key];

        if (result.Trim().Length == 0)
        {
            result = $"select * from {key}";
        }

        return result;
    }

    private void FillInMemoryData(DbCommand cmd)
    {
        foreach(KeyValuePair<string, string> kvp in InMemoryDataQueries)
        {
            UpdateInMemoryData(cmd, kvp.Key);
        }
    }

    public void RegisterHandlers()
    {
        
    }

    private static readonly List<Func<TransportData, bool>> m_requestUserSessionValidationIgnorabilityValidators = new List<Func<TransportData, bool>>();

    public static void RegisterRequestUserSessionValidationIgnorabilityValidator(Func<TransportData, bool> validator)
    {
        m_requestUserSessionValidationIgnorabilityValidators.Add(validator);
    }

    private static bool ShouldIgnoreUserSessionValidation(TransportData td)
    {
        bool result = false;

        foreach(Func<TransportData, bool> validator in m_requestUserSessionValidationIgnorabilityValidators)
        {
            if (validator(td) == true) return true;
        }

        return result;
    }

    public PayloadPacket AcceptRequest(PayloadPacket incomingPkt, IFormFileCollection files)
    {
        TransactionResult tr = new TransactionResult();

        try
        {
            TransportData td = TransportData.FromPayloadPacket(incomingPkt);

            if (files != null) td.SetFileCollection(files);

            if (ShouldIgnoreUserSessionValidation(td))
            {
                // USER LOGIN SESSION VALIDATION IS IGNORED ONLY FOR CERTAIN SCENARIOS
                // EG. ACADEMIC BATCH FETCH REQUESTS BECAUSE THEY ARE ONLY REQUIRED 
                // DURING USER NAME FINDING PROCESS OF THE STUDENT ADMISSION APP LOGIN PROCESS.
            }
            else
            {
                //SystemUserOperationsManager.CheckUserSessionValidityForPerformingOperations(incomingPkt);
                //SessionController.SUOM.UpdateSystemUserLastActiveDateTime(incomingPkt);
            }

            PerformProcess(td);

            tr.Successful = true;

            if (td.InMemoryDataRepositoryNamesToBeFetched.Count > 0)
            {
                InMemoryData.GetInstance().UpdatedTableNames = td.InMemoryDataRepositoryNamesToBeFetched;

                List<PayloadPacket> lstPkt = new List<PayloadPacket>();

                foreach (string masterTableName in InMemoryData.GetInstance().UpdatedTableNames)
                {
                    var cont = new DataContainer();

                    if (InMemoryData.GetInstance().Tables.Contains(masterTableName))
                    {
                        var dtb = InMemoryData.GetInstance().Tables[masterTableName];
                        var coll = cont.GetOrCreateCollection(masterTableName);

                        DataAccessUtils.TransferDataTableColumnDefinitionsToDataCollection(dtb, ref coll);
                        DataAccessUtils.TransferDataTableDataToDataCollection(dtb, ref coll);
                    }

                    PayloadPacket pktInMemoryData = PayloadPacket.CreateNewInstance();
                    pktInMemoryData.PartNo = 1;
                    pktInMemoryData.TotalPartCount = 1;
                    pktInMemoryData.Sender = 0;
                    pktInMemoryData.TargetMethod = string.Empty;
                    pktInMemoryData.Topic = Topics.CacheDataTopic(masterTableName);
                    pktInMemoryData.PayloadDescriptor = "CacheData";
                    pktInMemoryData.Payload = cont.Serialize();

                    lstPkt.Add(pktInMemoryData);
                }
            }

            td.InMemoryDataRepositoryNamesToBeFetched.Clear();

            tr.Tag = td.ConvertToJsonObject();
            tr.TagType = td.GetType().FullName;
        }
        catch (Exception ex)
        {
            tr.AbsorbException(ex);
            //tr.AbsorbExceptionWithStackTrace(ex);
        }

        PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr, incomingPkt);
        return pktResult;
    }

    public TransactionResult PerformSaveProcessForInternalCalls(DbCommand cmd, TransportData td,
        List<Tuple<string, DbParameterContainer>> preProcessQueriesAndParams = null,
        Dictionary<string, string> alternativeTableNames = null)
    {
        TransactionResult tr = new TransactionResult();

        try
        {
            PerformSaveProcess(cmd, td, preProcessQueriesAndParams, alternativeTableNames);

            tr.Successful = true;

            //if (td.InMemoryDataRepositoryNamesToBeFetched.Count > 0)
            //{
            //    List<PayloadPacket> lstPkt = new List<PayloadPacket>();
                
            //    foreach (string masterTableName in td.InMemoryDataRepositoryNamesToBeFetched)
            //    {
            //        var cont = new DataContainer();

            //        if (InMemoryData.GetInstance().Tables.Contains(masterTableName))
            //        {
            //            var dtb = InMemoryData.GetInstance().Tables[masterTableName];
            //            var coll = cont.GetOrCreateCollection(masterTableName);

            //            DataAccessUtils.TransferDataTableColumnDefinitionsToDataCollection(dtb, ref coll);
            //            DataAccessUtils.TransferDataTableDataToDataCollection(dtb, ref coll);
            //        }

            //        PayloadPacket pktInMemoryData = PayloadPacket.CreateNewInstance();
            //        pktInMemoryData.PartNo = 1;
            //        pktInMemoryData.TotalPartCount = 1;
            //        //pktInMemoryData.RequestId = 0;
            //        pktInMemoryData.Sender = 0;
            //        pktInMemoryData.TargetMethod = string.Empty;
            //        pktInMemoryData.Topic = Topics.CacheDataTopic(masterTableName);
            //        pktInMemoryData.PayloadDescriptor = "CacheData";
            //        pktInMemoryData.Payload = cont.ConvertToJsonObject().ToString();

            //        lstPkt.Add(pktInMemoryData);
            //    }

            //    ODB.Enqueue(lstPkt);
            //}

            td.InMemoryDataRepositoryNamesToBeFetched.Clear();
        }
        catch (Exception ex)
        {
            tr.AbsorbException(ex);
        }

        return tr;
    }

    private void PerformProcess(TransportData td, List<Tuple<string, DbParameterContainer>> preProcessQueriesAndParams = null,
        Dictionary<string, string> alternativeTableNames = null)
    {
        if (td.RequestType == RequestTypes.Save.ToString())
        {
            PerformSaveProcess(null, td, preProcessQueriesAndParams, alternativeTableNames);
        }
        else if (td.RequestType == RequestTypes.Deletion.ToString())
        {
            PerformDeletionProcess(td, preProcessQueriesAndParams, alternativeTableNames);
        }
        else if (td.RequestType == RequestTypes.CustomProcess.ToString())
        {
            PerformCustomProcess(td, preProcessQueriesAndParams, alternativeTableNames);
        }
        else if (td.RequestType == RequestTypes.Fetch.ToString())
        {
            PerformFetch(td, preProcessQueriesAndParams, alternativeTableNames);
        }
        else if (td.RequestType == RequestTypes.GenerateDocument.ToString())
        {
            AcceptDocumentGenerationRequest(td);
        }
    }

    private void PerformSaveProcess(DbCommand cmd, TransportData td, List<Tuple<string, DbParameterContainer>> preProcessQueriesAndParams = null,
        Dictionary<string, string> alternativeTableNames = null)
    {
        ValidationResultEntryCollection vrec = new ValidationResultEntryCollection();

        bool inlineCall = (cmd != null);

        ulong cToken = 0L;

        if (!inlineCall)
        {
            cToken = DAU.OpenConnectionAndBeginTransaction();
        }

        try
        {
            if (!inlineCall) cmd = DAU.CreateCommand();

            if (preProcessQueriesAndParams != null)
            {
                foreach (var t in preProcessQueriesAndParams)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = t.Item1;
                    if (t.Item2 != null)
                    {
                        t.Item2.AttachToCommand(cmd);
                    }
                    cmd.ExecuteNonQuery2();
                }
            }

            List<string> lstEntityTypes_Pre_DependentGeneration = EntityTypes.GetEntityTypesInTransportData(td);

            foreach (string entityType in lstEntityTypes_Pre_DependentGeneration)
            {
                List<BaseObject> lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                lstDomainEntities.ForEach(e =>
                {
                    e.SetComputedPropertyValues(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });

                lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                lstDomainEntities.ForEach(e =>
                {
                    if (!e.DenormalizationProcessDone)
                    {
                        e.PerformDenormalizationProcesses(cmd, td, vrec);
                        if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                        e.DenormalizationProcessDone = true;
                        e.MergeIntoTransportData(td);
                    }
                });

                lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                lstDomainEntities.ForEach(e =>
                {
                    e.GenerateAndMergeDependentEntities(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });
            }

            List<string> lstEntityTypes_Pre_Save = EntityTypes.GetEntityTypesInTransportData(td);

            foreach (string entityType in lstEntityTypes_Pre_Save)
            {
                List<BaseObject> lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                lstDomainEntities.ForEach(e =>
                {
                    e.PerformPreSaveValidationProcess(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });

                lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                lstDomainEntities.ForEach(e =>
                {
                    e.CheckSaveValidity(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });

                lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                lstDomainEntities.ForEach(e =>
                {
                    e.PerformPreSaveProcess(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });

                lstDomainEntities.ForEach(e =>
                {
                    e.DeleteChildEntitiesFromDB(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });
            }

            List<string> lstTableNamesToUpdate = new List<string>();

            List<string> lstEntityTypes_Save = EntityTypes.GetEntityTypesInTransportData(td);
            // Done purposely so that new types of entities that may have been generated
            // during the pre-save processors are considered.

            foreach (string entityType in lstEntityTypes_Save)
            {
                string tableName = EntityTypes.GetDatabaseTableNameFromEntityType(entityType);
                
                if (alternativeTableNames != null)
                {
                    if (alternativeTableNames.ContainsKey(tableName)) tableName = alternativeTableNames[tableName];
                }

                lstTableNamesToUpdate.Add(tableName);

                List<BaseObject> lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                PerformFinalSaveProcess(cmd, tableName, lstDomainEntities);
            }

            foreach (string tableName in lstTableNamesToUpdate)
            {
                if (InMemoryDataQueries.ContainsKey(tableName))
                {
                    cmd.CommandText = "SELECT ChangeLevel FROM CacheDataChangeLevels WHERE MasterTableName = @MasterTableName";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharParameter("@MasterTableName", 1024, tableName);
                    var changeLevel = Utils.GetInt64(cmd.ExecuteScalar2());

                    changeLevel++;

                    cmd.CommandText = "DELETE FROM CacheDataChangeLevels WHERE MasterTableName = @MasterTableName";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharParameter("@MasterTableName", "CacheDataChangeLevels", tableName);
                    cmd.ExecuteNonQuery2();

                    cmd.CommandText = "INSERT INTO CacheDataChangeLevels (MasterTableName, ChangeLevel) VALUES (@MasterTableName, @ChangeLevel)";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharMaxParameter("@MasterTableName", tableName);
                    cmd.AddLongParameter("@ChangeLevel", changeLevel);
                    cmd.ExecuteNonQuery2();
                }

                if (InMemoryData.GetInstance().Tables.Contains(tableName))
                {
                    td.InMemoryDataRepositoryNamesToBeFetched.Add(tableName);
                }
            }

            List<string> lstEntityTypes_Pre_Commit = EntityTypes.GetEntityTypesInTransportData(td);

            foreach (string entityType in lstEntityTypes_Pre_Commit)
            {
                List<BaseObject> lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);
                lstDomainEntities.ForEach(e => {
                    e.InvokePreCommitProcess(cmd, td, vrec);
                    if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
                    e.MergeIntoTransportData(td);
                });
            }

            if (!inlineCall) DAU.CommitTransaction(cToken);

            List<string> lstEntityTypes_Post_Save = EntityTypes.GetEntityTypesInTransportData(td);

            foreach (string entityType in lstEntityTypes_Post_Save)
            {
                List<BaseObject> lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);
                lstDomainEntities.ForEach(e => {
                    e.InvokePostSaveProcess(cmd, td, vrec);
                    e.MergeIntoTransportData(td);
                });

                if (vrec.Count > 0) throw new DomainException(vrec.FormulateMessageFromList());
            }

            foreach (string tableName in lstTableNamesToUpdate)
            {
                UpdateInMemoryData(cmd, tableName);

                if (InMemoryData.GetInstance().Tables.Contains(tableName))
                {
                    td.InMemoryDataRepositoryNamesToBeFetched.Add(tableName);
                }
            }
        }
        catch(Exception)
        {
            if (!inlineCall) DAU.RollbackTransaction(cToken);
            throw;
        }
        finally
        {
            if (!inlineCall) DAU.CloseConnection(cToken);
        }
    }

    public static void PerformFinalSaveProcess(DbCommand cmd, string tableName, List<BaseObject> lstDomainEntities)
    {
        foreach (var domainEntity in lstDomainEntities)
        {
            var primaryKeyValueMap = domainEntity.GetPrimaryKeyValueMap();
            var conditionsAndParams = DomainUtils.FormulateQueryConditionStringAndParametersFromValueMap(cmd, tableName, primaryKeyValueMap);

            string strCondition = conditionsAndParams.Item1;
            var paramsMap = conditionsAndParams.Item2;

            string strDeleteQuery = $"DELETE FROM {tableName}";
            if (strCondition.Trim().Length > 0) strDeleteQuery += $" WHERE {strCondition}";

            cmd.CommandText = strDeleteQuery;
            cmd.Parameters.Clear();
            paramsMap.AttachToCommand(cmd);
            cmd.ExecuteNonQuery2();

            var strPrimaryKeyValues = domainEntity.GetPrimaryKeyValuesString();

            cmd.CommandText = "delete from entityregister where primarykeyvalues = @PrimaryKeyValues";
            cmd.Parameters.Clear();
            cmd.AddVarcharParameter("@PrimaryKeyValues", 512, strPrimaryKeyValues);
            cmd.ExecuteNonQuery2();

            cmd.CommandText = "insert into entityregister (primarykeyvalues, tablename) values (@PrimaryKeyValues, @TableName)";
            cmd.Parameters.Clear();
            cmd.AddVarcharMaxParameter("@PrimaryKeyValues", strPrimaryKeyValues);
            cmd.AddVarcharMaxParameter("@TableName", tableName);
            cmd.ExecuteNonQuery2();

            var d = domainEntity.BaseData;

            DataAccessUtils.FormulateInsertCommandFromData(tableName, cmd, d);
            cmd.ExecuteNonQuery2();
        }
    }

    private void PerformDeletionProcess(TransportData td, List<Tuple<string, DbParameterContainer>> preProcessQueriesAndParams = null,
        Dictionary<string, string> alternativeTableNames = null)
    {
        ValidationResultEntryCollection vrec = new ValidationResultEntryCollection();

        var cToken = DAU.OpenConnectionAndBeginTransaction();

        try
        {
            var cmd = DAU.CreateCommand();

            if (preProcessQueriesAndParams != null)
            {
                foreach (var kvp in preProcessQueriesAndParams)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = kvp.Item1;
                    if (kvp.Item2 != null)
                    {
                        kvp.Item2.AttachToCommand(cmd);
                    }
                    cmd.ExecuteNonQuery2();
                }
            }

            List<string> lstTableNamesToUpdate = new List<string>();

            List<string> lstEntityTypes_Deletion = EntityTypes.GetEntityTypesInTransportData(td);

            foreach (string entityType in lstEntityTypes_Deletion)
            {
                List<BaseObject> lstDomainEntities = EntityTypes.FormulateDomainEntityListFromTransportData(entityType, td, null, null, true, true);

                foreach (var domainEntity in lstDomainEntities)
                {
                    string tableName = EntityTypes.GetDatabaseTableNameFromEntityType(entityType);

                    lstTableNamesToUpdate.Add(tableName);

                    TransactionResult trDeletion = domainEntity.CheckDeletionValidity(cmd);
                    if (!trDeletion.Successful) throw new DomainException(trDeletion.Message);

                    var primaryKeyValueMap = domainEntity.GetPrimaryKeyValueMap();
                    var conditionsAndParams = DomainUtils.FormulateQueryConditionStringAndParametersFromValueMap(cmd, tableName, primaryKeyValueMap);

                    string strCondition = conditionsAndParams.Item1;
                    var paramsMap = conditionsAndParams.Item2;

                    string strDeleteQuery = $"DELETE FROM {tableName}";
                    if (strCondition.Trim().Length > 0) strDeleteQuery += $" WHERE {strCondition}";

                    cmd.CommandText = strDeleteQuery;
                    cmd.Parameters.Clear();
                    paramsMap.AttachToCommand(cmd);
                    cmd.ExecuteNonQuery2();

                    var strPrimaryKeyValues = domainEntity.GetPrimaryKeyValuesString();

                    cmd.CommandText = "delete from entityregister where primarykeyvalues = @PrimaryKeyValues";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharParameter("@PrimaryKeyValues", 512, strPrimaryKeyValues);
                    cmd.ExecuteNonQuery2();
                }
            }

            foreach (string tableName in lstTableNamesToUpdate)
            {
                if (InMemoryDataQueries.ContainsKey(tableName))
                {
                    cmd.CommandText = "SELECT ChangeLevel FROM CacheDataChangeLevels WHERE MasterTableName = @MasterTableName";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharParameter("@MasterTableName", 1024, tableName);
                    var changeLevel = Utils.GetInt64(cmd.ExecuteScalar2());

                    changeLevel++;

                    cmd.CommandText = "DELETE FROM CacheDataChangeLevels WHERE MasterTableName = @MasterTableName";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharParameter("@MasterTableName", "CacheDataChangeLevels", tableName);
                    cmd.ExecuteNonQuery2();

                    cmd.CommandText = "INSERT INTO CacheDataChangeLevels (MasterTableName, ChangeLevel) VALUES (@MasterTableName, @ChangeLevel)";
                    cmd.Parameters.Clear();
                    cmd.AddVarcharMaxParameter("@MasterTableName", tableName);
                    cmd.AddLongParameter("@ChangeLevel", changeLevel);
                    cmd.ExecuteNonQuery2();
                }

                if (InMemoryData.GetInstance().Tables.Contains(tableName))
                {
                    td.InMemoryDataRepositoryNamesToBeFetched.Add(tableName);
                }
            }

            DAU.CommitTransaction(cToken);

            foreach (string tableName in lstTableNamesToUpdate)
            {
                UpdateInMemoryData(cmd, tableName);

                if (InMemoryData.GetInstance().Tables.Contains(tableName))
                {
                    td.InMemoryDataRepositoryNamesToBeFetched.Add(tableName);
                }
            }
        }
        catch (Exception)
        {
            DAU.RollbackTransaction(cToken);
            throw;
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }
    }


    private void PerformCustomProcess(TransportData td, 
        List<Tuple<string, DbParameterContainer>> preProcessQueriesAndParams = null,
        Dictionary<string, string> alternativeTableNames = null)
    {
        var cToken = DAU.OpenConnectionAndBeginTransaction();

        try
        {
            td.MainData.GetKeys().ForEach(key =>
            {
                CustomProcessRequestHandlers.GetCustomProcessHandler(key)?.Invoke(td);
            });

            DAU.CommitTransaction(cToken);

            foreach(Action<TransportData> action in td.CustomProcessPostCommitActions)
            {
                action(td);
            }
        }
        catch (Exception)
        {
            DAU.RollbackTransaction(cToken);
            throw;
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }
    }


    private void PerformFetch(TransportData td, List<Tuple<string, DbParameterContainer>> preProcessQueriesAndParams = null,
        Dictionary<string, string> alternativeTableNames = null)
    {
        var cToken = DAU.OpenConnection();

        try
        {
            td.MainData.GetKeys().ForEach(key =>
            {
                FetchRequestHandlers.GetFetchHandler(key)?.Invoke(td);
            });
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }
    }

    private void AcceptDocumentGenerationRequest(TransportData td)
    {
        //string strCode = DocumentGenerationRequestManager.AddDocumentGenerationRequestAndReturnCode(td);
        //var coll = td.MainData.GetOrCreateCollection(DocumentGenerationRequestManager.CodeCollectionName);

        //JObject objResult = new JObject { { "Code", strCode } };

        //coll.Entries.Add(objResult);
    }

    public DocumentStreamContainer PerformDocumentGeneration(TransportData td)
    {
        var cToken = DAU.OpenConnection();

        DocumentStreamContainer result = null;

        try
        {
            foreach(string key in td.MainData.GetKeys())
            {
                result = DocumentGenerationRequestHandlers.GetDocumentGenerationRequestHandler(key)?.Invoke(td);
                break;
            }
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }

        return result;
    }

    private void UpdateInMemoryData(DbCommand cmd, string key)
    {
        if (InMemoryDataQueries.ContainsKey(key))
        {
            UpdateDatatable(cmd, InMemoryData.GetInstance(), key, GetInMemoryDataQuery(key));
        }
    }

    private void UpdateDatatable(DbCommand cmd, DataSet ds, string tableName, string qry)
    {
        if (ds.Tables.Contains(tableName)) ds.Tables[tableName].Clear();
        DAU.FillByQuery(cmd, ds, tableName, qry, new Dictionary<string, object>());
    }

    public PayloadPacket GetCacheDataChangeLevel(PayloadPacket incomingPkt)
    {
        TransactionResult tr = new TransactionResult();

        var cToken = DAU.OpenConnectionAndBeginTransaction();

        var resp = new CacheDataChangeLevelResponse();

        var req = CacheDataChangeLevelFetchRequest.Deserialize(incomingPkt.Payload.ToString());

        try
        {
            var masterTableNames = req.FormulateTableNamesString();

            var coll = new DataCollection();

            string strFilter = string.Empty;
            if (masterTableNames.Length > 0) strFilter = $" WHERE MasterTableName IN ({masterTableNames})";

            var cmd = DAU.CreateCommand();

            DataAccessUtils.FillByQuery(cmd, coll, $"SELECT * FROM CacheDataChangeLevels{strFilter}");

            foreach (JObject obj in coll.Entries)
            {
                resp.ChangeLevels.Add(Utils.GetString(obj["MasterTableName"]).Trim(), Utils.GetInt64(obj["ChangeLevel"]));
            }

            foreach(string mtName in req.MasterTableNames)
            {
                if (!resp.ChangeLevels.ContainsKey(mtName)) resp.ChangeLevels.Add(mtName, 0);
            }

            tr.Successful = true;
            tr.Tag = resp.Serialize();

            DAU.CommitTransaction(cToken);
        }
        catch (Exception ex)
        {
            DAU.RollbackTransaction(cToken);
            tr.AbsorbException(ex);
            throw;
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }

        PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr);
        return pktResult;
    }
}
