////using CCA.Util;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;

//public class Billdesk_Order : BaseObject, IPaymentGatewayOrder
//{
//    public static string EntityType { get => "Billdesk_Order"; }
//    public static string PrimaryKeys { get => "Ref"; }
//    public static string MasterTableName { get => "Billdesk_Orders"; }
//    public static string RepositoryName { get => MasterTableName; }
//    public static string SingularName { get => "Billdesk_Order"; }
//    public static string PluralName { get => "Billdesk_Orders"; }

//    public static void RegisterEntityTypeAspects()
//    {
//        //EntityTypes.RegisterEntityTypeAspects(EntityType, MasterTableName, GetDomainEntityListFormulator(), SingularName, PluralName, PrimaryKeys,
//        //    GetInMemoryTablesRequired());

//        FetchRequestHandlers.RegisterFetchHandler(Billdesk_Order_FetchRequest.FetchRequestType, HandleFetchRequest);
//    }

//    public static Billdesk_Order GetEntityFromDB(DbCommand cmd, long Ref, bool allowEdit)
//    {
//        var req = new Billdesk_Order_FetchRequest();
//        req.Billdesk_Order_Refs.Add(Ref);

//        var lst = GetEntitiesFromDB(cmd, req, allowEdit);
//        if (lst.Count > 0) return lst[0];

//        return null;
//    }

//    private static void HandleFetchRequest(TransportData td)
//    {
//        if (td.MainData.CollectionExists(Billdesk_Order_FetchRequest.FetchRequestType))
//        {
//            var reqColl = td.MainData.GetCollection(Billdesk_Order_FetchRequest.FetchRequestType);
//            reqColl.Entries.ForEach(obj =>
//            {
//                var req = Billdesk_Order_FetchRequest.FromJsonObject(obj);

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

//    public static List<Billdesk_Order> GetEntitiesFromDB(DbCommand cmd, Billdesk_Order_FetchRequest reqParams, bool allowEdit)
//    {
//        var result = new List<Billdesk_Order>();

//        var dau = SessionController.DAU;

//        var dc = new DataContainer();

//        var coord = CommandFormulationCoordinator.CreateInstance(MasterTableName)
//            .CoordinateLongValues("Ref", reqParams.Billdesk_Order_Refs)
//            .FormulateCommandStringAndParameters(cmd);

//        if (cmd.Parameters.Count <= 0) throw new DomainException($"No filters supplied for query.");

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

//    private Billdesk_Order(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
//        : base(data, allowEdit, parentContainer, allowInPlaceEditing)
//    {

//    }

//    public static Billdesk_Order CreateInstance(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
//    {
//        return new Billdesk_Order(data, parentContainer, allowEdit, allowInPlaceEditing);
//    }

//    public static Billdesk_Order CreateNewInstance()
//    {
//        var data = SessionController.DAU.GetBlankDataObjectFromTable(MasterTableName);
//        return CreateInstance(data, new DataContainer(), true);
//    }

//    public Billdesk_Order GetEditableVersion()
//    {
//        return new Billdesk_Order(BaseData, new DataContainer(), true);
//    }

//    public void AbsorbOrderResponse(DbCommand cmd, Billdesk_Response_Parameters response,
//        Billdesk_Response_Parameters statusInquiryResponse, bool isBackgroundProcess)
//    {
//        if (response.order_id != order_id) throw new DomainException("Request and Response Order Ids do not match.");

//        if (response.order_status.Trim() == "Success")
//        {
//            if (Utils.GetDecimal(response.txn_amount, 2) != amount) throw new DomainException("Request and Response Amounts do not match.");
//            if (response.currency != currency) throw new DomainException("Request and Response Currencies do not match.");
//            //if (response.additional_info3.Replace("-", string.Empty) != billing_name.Replace("-", string.Empty)) throw new DomainException("Request and Response Billing Names do not match.");
//            if (response.additional_info1 != additional_info1_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.additional_info2 != additional_info2_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.additional_info3 != additional_info3_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.additional_info4 != additional_info4_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.additional_info5 != additional_info5_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.additional_info6 != additional_info6_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.additional_info7 != additional_info7_projected) throw new DomainException("Request and Response Merchant Parameters do not match.");

//            if (Utils.GetDecimal(statusInquiryResponse.txn_amount, 2) != amount) throw new DomainException("Request and Response Amounts do not match.");
//            //if (statusInquiryResponse.additional_info3.Replace("-", string.Empty) != billing_name) throw new DomainException("Request and Response Billing Names do not match.");
//            if (statusInquiryResponse.currency != currency) throw new DomainException("Request and Response Currencies do not match.");
//            if (statusInquiryResponse.order_id != order_id) throw new DomainException("Request and Response Order Ids do not match.");
//            if (statusInquiryResponse.order_status != response.order_status) throw new DomainException("Request and Response Order Statuses do not match.");
//            if (statusInquiryResponse.bank_reference != response.bank_reference) throw new DomainException("Request and Response Bank Ref Nos. do not match.");
//        }

//        if (isBackgroundProcess)
//        {
//            if (response.txn_date.Trim().Length == 0)
//            {
//                ResponseDateTime = DTU.GetCurrentDateTime();
//            }
//            else
//            {
//                ResponseDateTime = response.txn_date.Replace(" ", "-").Replace(":", "-").Replace(".", "-");
//            }

//            Rechecked = true;
//        }
//        else
//        {
//            var checkCount = Utils.GetInt32(DataAccessUtils.GetScalarValueByQuery(cmd, $"SELECT COUNT(Ref) FROM {MasterTableName}" +
//                $" WHERE Ref = {order_id} AND (LEN(txn_reference) > 0 OR LEN(bank_reference) > 0)"));

//            if (checkCount > 0) throw new DomainException("This Order has already been processed.");

//            ResponseDateTime = DTU.GetCurrentDateTime();
//            trans_date = response.txn_date;
//        }

//        txn_reference = response.txn_reference;
//        bank_reference = response.bank_reference;
//        order_status = response.order_status;
//        error_description = response.error_description;
//        auth_status = response.auth_status;
//        auth_message = response.auth_status;
//        txn_amount = Utils.GetDecimal(response.txn_amount, 2);
//        response_checksum = response.response_checksum;
//    }

//    public long Ref
//    {
//        get => GetLongPropertyValue("Ref");
//        set => SetLongPropertyValue("Ref", value);
//    }

//    public long CampusRef
//    {
//        get => GetLongPropertyValue("CampusRef");
//        set => SetLongPropertyValue("CampusRef", value);
//    }

//    public string OrderGenerationDateTime
//    {
//        get => GetStringPropertyValue("OrderGenerationDateTime");
//        set => SetStringPropertyValue("OrderGenerationDateTime", value);
//    }

//    public string ResponseDateTime
//    {
//        get => GetStringPropertyValue("ResponseDateTime");
//        set => SetStringPropertyValue("ResponseDateTime", value);
//    }

//    public string order_id
//    {
//        get => Ref.ToString();
//    }

//    public string merchant_id
//    {
//        get => GetStringPropertyValue("merchant_id");
//        set => SetStringPropertyValue("merchant_id", value);
//    }

//    public decimal amount
//    {
//        get => GetDecimalPropertyValue("amount", 2);
//        set => SetDecimalPropertyValue("amount", value);
//    }

//    public string currency
//    {
//        get => GetStringPropertyValue("currency");
//        set => SetStringPropertyValue("currency", value);
//    }

//    public string callback_url
//    {
//        get => GetStringPropertyValue("callback_url");
//        set => SetStringPropertyValue("callback_url", value);
//    }

//    public string customer_email_id
//    {
//        get => additional_info1;
//        set => additional_info1 = value;
//    }

//    public string customer_mobile_no
//    {
//        get => additional_info2;
//        set => additional_info2 = value;
//    }

//    public string billing_name
//    {
//        get => additional_info3;
//        set => additional_info3 = value;
//    }

//    public string additional_info1
//    {
//        get => GetStringPropertyValue("additional_info1");
//        set => SetStringPropertyValue("additional_info1", value);
//    }

//    public string additional_info2
//    {
//        get => GetStringPropertyValue("additional_info2");
//        set => SetStringPropertyValue("additional_info2", value);
//    }

//    public string additional_info3
//    {
//        get => GetStringPropertyValue("additional_info3");
//        set => SetStringPropertyValue("additional_info3", value);
//    }

//    public string additional_info4
//    {
//        get => GetStringPropertyValue("additional_info4");
//        set => SetStringPropertyValue("additional_info4", value);
//    }

//    public string additional_info5
//    {
//        get => GetStringPropertyValue("additional_info5");
//        set => SetStringPropertyValue("additional_info5", value);
//    }

//    public string additional_info6
//    {
//        get => GetStringPropertyValue("additional_info6");
//        set => SetStringPropertyValue("additional_info6", value);
//    }

//    public string additional_info7
//    {
//        get => GetStringPropertyValue("additional_info7");
//        set => SetStringPropertyValue("additional_info7", value);
//    }

//    public string additional_info1_projected => additional_info1.Trim().Length == 0 ? "NA" : additional_info1;
//    public string additional_info2_projected => additional_info2.Trim().Length == 0 ? "NA" : additional_info2;
//    public string additional_info3_projected => additional_info3.Trim().Length == 0 ? "NA" : additional_info3;
//    public string additional_info4_projected => additional_info4.Trim().Length == 0 ? "NA" : additional_info4;
//    public string additional_info5_projected => additional_info5.Trim().Length == 0 ? "NA" : additional_info5;
//    public string additional_info6_projected => additional_info6.Trim().Length == 0 ? "NA" : additional_info6;
//    public string additional_info7_projected => additional_info7.Trim().Length == 0 ? "NA" : additional_info7;

//    public string txn_reference
//    {
//        get => GetStringPropertyValue("txn_reference");
//        set => SetStringPropertyValue("txn_reference", value);
//    }

//    public string bank_reference
//    {
//        get => GetStringPropertyValue("bank_reference");
//        set => SetStringPropertyValue("bank_reference", value);
//    }

//    public string bank_ref_no
//    {
//        get => bank_reference;
//        set => bank_reference = value;
//    }

//    public string order_status
//    {
//        get => GetStringPropertyValue("order_status");
//        set => SetStringPropertyValue("order_status", value);
//    }

//    public string error_status
//    {
//        get => GetStringPropertyValue("error_status");
//        set => SetStringPropertyValue("error_status", value);
//    }

//    public string error_description
//    {
//        get => GetStringPropertyValue("error_description");
//        set => SetStringPropertyValue("error_description", value);
//    }

//    public string auth_status
//    {
//        get => GetStringPropertyValue("auth_status");
//        set => SetStringPropertyValue("auth_status", value);
//    }

//    public string auth_message
//    {
//        get => GetStringPropertyValue("auth_message");
//        set => SetStringPropertyValue("auth_message", value);
//    }

//    public decimal txn_amount
//    {
//        get => GetDecimalPropertyValue("txn_amount", 2);
//        set => SetDecimalPropertyValue("txn_amount", value);
//    }

//    public string response_checksum
//    {
//        get => GetStringPropertyValue("response_checksum");
//        set => SetStringPropertyValue("response_checksum", value);
//    }

//    public string trans_date
//    {
//        get => GetStringPropertyValue("trans_date");
//        set
//        {
//            SetStringPropertyValue("trans_date", value);
//            this.TransDateTimeInSystemFormat = Billdesk_Response_Parameters.ConvertResponseTransDateToTransDateTimeInSystemFormat(value);
//        }
//    }

//    public string TransDateTimeInSystemFormat
//    {
//        get => GetStringPropertyValue("TransDateTimeInSystemFormat");
//        set => SetStringPropertyValue("TransDateTimeInSystemFormat", value);
//    }

//    public string SettlementDateTime
//    {
//        get => GetStringPropertyValue("SettlementDateTime");
//        set => SetStringPropertyValue("SettlementDateTime", value);
//    }

//    public bool Rechecked
//    {
//        get => GetBooleanPropertyValue("Rechecked");
//        set => SetBooleanPropertyValue("Rechecked", value);
//    }

//    public long FinancialReceiptGroupRef
//    {
//        get => GetLongPropertyValue("FinancialReceiptGroupRef");
//        set => SetLongPropertyValue("FinancialReceiptGroupRef", value);
//    }

//    public static Billdesk_Response_Parameters FormulateResponseParameters(long campusRef, string resp)
//    {
//        Billdesk_PG_Credentials cred = Billdesk_PG_Credentials.ReadCredentials(campusRef);

//        Billdesk_Response_Parameters result = new Billdesk_Response_Parameters();

//        var responseParts = resp.Split("|", StringSplitOptions.RemoveEmptyEntries);

//        if (responseParts.Length != 26)
//        {
//            throw new DomainException("Invalid response format.");
//        }

//        string merchantId = responseParts[0];
//        string orderId = responseParts[1];
//        string txn_reference = responseParts[2];
//        string bank_reference = responseParts[3];
//        string txn_amount = responseParts[4];
//        string currency = responseParts[8];
//        string txn_date = responseParts[13];
//        string auth_status = responseParts[14];
//        string additional_info1 = responseParts[16];
//        string additional_info2 = responseParts[17];
//        string additional_info3 = responseParts[18];
//        string additional_info4 = responseParts[19];
//        string additional_info5 = responseParts[20];
//        string additional_info6 = responseParts[21];
//        string additional_info7 = responseParts[22];
//        string error_status = responseParts[23];
//        string error_description = responseParts[24];
//        string response_checksum = responseParts[25];

//        string response_without_checksum = string.Join("|", responseParts[0..25]);

//        string checksum_for_validation = Billdesk_Integrator.GetHMACSHA256(response_without_checksum, cred.Key);

//        if (checksum_for_validation != response_checksum) throw new DomainException("Checksum Mismatch!");

//        result.additional_info1 = additional_info1;
//        result.additional_info2 = additional_info2;
//        result.additional_info3 = additional_info3;
//        result.additional_info4 = additional_info4;
//        result.additional_info5 = additional_info5;
//        result.additional_info6 = additional_info6;
//        result.additional_info7 = additional_info7;
//        result.auth_status = auth_status;
//        result.bank_reference = bank_reference;
//        result.currency = currency;
//        result.error_description = error_description;
//        result.error_status = error_status;
//        result.merchant_id = merchantId;
//        result.order_id = orderId;
//        result.response_checksum = response_checksum;
//        result.txn_amount = txn_amount;
//        result.txn_date = txn_date;
//        result.txn_reference = txn_reference;

//        return result;
//    }

//    public static string FormulateStatusInquiryString(long campusRef, Billdesk_Response_Parameters resp_params)
//    {
//        //CCACrypto ccaCrypto = new CCACrypto();

//        Billdesk_PG_Credentials cred = Billdesk_PG_Credentials.ReadCredentials(campusRef);

//        JObject reqObject = new JObject
//        {
//            { "bank_reference", resp_params.bank_reference }
//        };

//        var objString = JsonConvert.SerializeObject(reqObject);

//       // string enc_objString = ccaCrypto.Encrypt(objString, cred.SecurityId);

//        var ccaRequest = $"enc_request={enc_objString}&access_code={cred.SecurityId}&command=orderStatusTracker&request_type=JSON&response_type=JSON&version=1.1";

//        //var result = $"{cred.StatusInquiryURL}&{ccaRequest}";

//        //return result;

//        return ccaRequest;
//    }

//    public static Billdesk_Response_Parameters FormulateResponseParametersFromStatusInquiry(long campusRef, string resp)
//    {
//        Billdesk_PG_Credentials cred = Billdesk_PG_Credentials.ReadCredentials(campusRef);

//        Billdesk_Response_Parameters result = new Billdesk_Response_Parameters();

//        var responseFormatErrorMessage = "Status Response is not in the proper format.";

//        //if (resp.Contains(" ")) throw new DomainException(responseFormatErrorMessage);

//        string[] segments = resp.Split('&');

//        if (segments.Length < 2) throw new DomainException(responseFormatErrorMessage);

//        string statusString = segments[0].Trim();
//        var statusStringParts = statusString.Split('=');
//        if (statusStringParts.Length != 2) throw new DomainException(responseFormatErrorMessage);

//        var statusValue = statusStringParts[1];

//        if (statusValue != "0" && statusValue != "1") throw new DomainException(responseFormatErrorMessage);

//        if (statusValue == "1")
//        {
//            if (segments.Length != 3) throw new DomainException(responseFormatErrorMessage);
//        }
//        else if (statusValue == "0")
//        {
//            if (segments.Length != 2) throw new DomainException(responseFormatErrorMessage);
//        }

//        if (statusValue == "1")
//        {
//            var errorCode = segments[2];
//            throw new DomainException($"Error while validating. Error Code : {errorCode}");
//        }
//        else
//        {
//            var encryptedResponse = segments[1].Trim();

//            var encryptedResponseParts = encryptedResponse.Split('=');

//            if (encryptedResponseParts.Length != 2) throw new DomainException(responseFormatErrorMessage);

//            CCACrypto ccaCrypto = new CCACrypto();
//            string decryptedResponseString = ccaCrypto.Decrypt(encryptedResponseParts[1], cred.Key);

//            var responseObject = JObject.Parse(decryptedResponseString);

//            if (responseObject.ContainsKey("status"))
//            {
//                if (Utils.GetString(responseObject["status"]) == "1")
//                {
//                    throw new DomainException(Utils.GetString(responseObject["error_desc"]));
//                }
//            }

//            result.txn_amount = Utils.GetString(responseObject["txn_amount"]);
//            result.txn_date = Utils.GetString(responseObject["txn_date"]);
//            result.currency = Utils.GetString(responseObject["order_currncy"]);
//            result.order_id = Utils.GetString(responseObject["order_no"]);
//            //result.order_status = Utils.GetString(responseObject["order_status"]);
//            result.bank_reference = Utils.GetString(responseObject["bank_reference"]);
//            //result.card_name = Utils.GetString(responseObject["order_card_name"]);
//        }

//        return result;
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
//                result.Add(new Billdesk_Order(data, td.MainData, allowEdit, allowInPlaceEditing));
//            }

//            return result;
//        };
//    }

//    protected override void SaveValidator(DbCommand cmd, TransportData td, ValidationResultEntryCollection vrec)
//    {
//        if (merchant_id.Trim().Length == 0) vrec.AddToList(string.Empty, "Merchant Id cannot be blank.");
//        if (order_id.Trim().Length == 0) vrec.AddToList(string.Empty, "Order Id cannot be blank.");
//        if (amount == 0.0M) vrec.AddToList(string.Empty, "Amount cannot be zero.");
//        if (currency.Trim().Length == 0) vrec.AddToList(string.Empty, "Currency cannot be blank.");
//        if (callback_url.Trim().Length == 0) vrec.AddToList(string.Empty, "Redirect URL cannot be blank.");
//        if (billing_name.Trim().Length == 0) vrec.AddToList(string.Empty, "Billing Name cannot be blank.");
//    }

//    public static string GetOrderStatus(string orderId)
//    {
//        var DAU = SessionController.DAU;

//        var cToken = DAU.OpenConnection();

//        string result = string.Empty;

//        try
//        {
//            var cmd = DAU.CreateCommand();
//            result = Utils.GetString(DataAccessUtils.GetScalarValueByQuery(cmd, $"SELECT order_status FROM {MasterTableName} WHERE Ref = {orderId}"));
//        }
//        catch (Exception)
//        {
//            result = string.Empty;
//        }
//        finally
//        {
//            DAU.CloseConnection(cToken);
//        }

//        return result;
//    }
//}
