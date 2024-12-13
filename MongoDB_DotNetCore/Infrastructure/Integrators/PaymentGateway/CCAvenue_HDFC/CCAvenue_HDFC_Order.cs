////using CCA.Util;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;

//public class CCAvenue_HDFC_Order : BaseObject, IPaymentGatewayOrder
//{
//    public static string EntityType { get => "CCAvenue_HDFC_Order"; }
//    public static string PrimaryKeys { get => "Ref"; }
//    public static string MasterTableName { get => "CCAvenue_HDFC_Orders"; }
//    public static string RepositoryName { get => MasterTableName; }
//    public static string SingularName { get => "CCAvenue_HDFC_Order"; }
//    public static string PluralName { get => "CCAvenue_HDFC_Orders"; }

//    public static void RegisterEntityTypeAspects()
//    {
//        EntityTypes.RegisterEntityTypeAspects(EntityType, MasterTableName, GetDomainEntityListFormulator(), SingularName, PluralName, PrimaryKeys,
//            GetInMemoryTablesRequired());

//        FetchRequestHandlers.RegisterFetchHandler(CCAvenue_HDFC_Order_FetchRequest.FetchRequestType, HandleFetchRequest);
//    }

//    public static CCAvenue_HDFC_Order GetEntityFromDB(DbCommand cmd, long Ref, bool allowEdit)
//    {
//        var req = new CCAvenue_HDFC_Order_FetchRequest();
//        req.CCAvenue_HDFC_Order_Refs.Add(Ref);

//        var lst = GetEntitiesFromDB(cmd, req, allowEdit);
//        if (lst.Count > 0) return lst[0];

//        return null;
//    }

//    private static void HandleFetchRequest(TransportData td)
//    {
//        if (td.MainData.CollectionExists(CCAvenue_HDFC_Order_FetchRequest.FetchRequestType))
//        {
//            var reqColl = td.MainData.GetCollection(CCAvenue_HDFC_Order_FetchRequest.FetchRequestType);
//            reqColl.Entries.ForEach(obj =>
//            {
//                var req = CCAvenue_HDFC_Order_FetchRequest.FromJsonObject(obj);

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

//    public static List<CCAvenue_HDFC_Order> GetEntitiesFromDB(DbCommand cmd, CCAvenue_HDFC_Order_FetchRequest reqParams, bool allowEdit)
//    {
//        var result = new List<CCAvenue_HDFC_Order>();

//        var dau = SessionController.DAU;

//        var dc = new DataContainer();

//        var coord = CommandFormulationCoordinator.CreateInstance(MasterTableName)
//            .CoordinateLongValues("Ref", reqParams.CCAvenue_HDFC_Order_Refs)
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
//        //{
//        //    { MasterTableName, string.Empty }
//        //};
//    }

//    private CCAvenue_HDFC_Order(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
//        : base(data, allowEdit, parentContainer, allowInPlaceEditing)
//    {

//    }

//    public static CCAvenue_HDFC_Order CreateInstance(JObject data, DataContainer parentContainer, bool allowEdit = false, bool allowInPlaceEditing = false)
//    {
//        return new CCAvenue_HDFC_Order(data, parentContainer, allowEdit, allowInPlaceEditing);
//    }

//    public static CCAvenue_HDFC_Order CreateNewInstance()
//    {
//        var data = SessionController.DAU.GetBlankDataObjectFromTable(MasterTableName);
//        return CreateInstance(data, new DataContainer(), true);
//    }

//    public CCAvenue_HDFC_Order GetEditableVersion()
//    {
//        return new CCAvenue_HDFC_Order(BaseData, new DataContainer(), true);
//    }

//    public CCAvenue_HDFC_Request_Parameters GenerateRequestParameters()
//    {
//        return new CCAvenue_HDFC_Request_Parameters()
//        {
//            order_id = this.order_id,
//            merchant_id = this.merchant_id,
//            amount = this.amount.ToString("0.00"),
//            currency = this.currency,
//            redirect_url = this.redirect_url,
//            cancel_url = this.cancel_url,
//            billing_name = this.billing_name,
//            merchant_param1 = this.merchant_param1,
//            merchant_param2 = this.merchant_param2,
//            merchant_param3 = this.merchant_param3,
//            merchant_param4 = this.merchant_param4,
//            merchant_param5 = this.merchant_param5
//        };
//    }

//    public void AbsorbOrderResponse(DbCommand cmd, CCAvenue_HDFC_Response_Parameters response, 
//        CCAvenue_HDFC_Response_Parameters statusInquiryResponse, bool isBackgroundProcess)
//    {
//        if (response.order_id != order_id) throw new DomainException("Request and Response Order Ids do not match.");

//        if (response.order_status.Trim() == "Success")
//        {
//            if (Utils.GetDecimal(response.mer_amount, 2) != amount) throw new DomainException("Request and Response Amounts do not match.");
//            if (response.currency != currency) throw new DomainException("Request and Response Currencies do not match.");
//            //if (response.billing_name.Replace("-", string.Empty) != billing_name.Replace("-", string.Empty)) throw new DomainException("Request and Response Billing Names do not match.");
//            if (response.merchant_param1 != merchant_param1) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.merchant_param2 != merchant_param2) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.merchant_param3 != merchant_param3) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.merchant_param4 != merchant_param4) throw new DomainException("Request and Response Merchant Parameters do not match.");
//            if (response.merchant_param5 != merchant_param5) throw new DomainException("Request and Response Merchant Parameters do not match.");

//            if (Utils.GetDecimal(statusInquiryResponse.mer_amount, 2) != amount) throw new DomainException("Request and Response Amounts do not match.");
//            //if (statusInquiryResponse.billing_name.Replace("-", string.Empty) != billing_name) throw new DomainException("Request and Response Billing Names do not match.");
//            if (statusInquiryResponse.currency != currency) throw new DomainException("Request and Response Currencies do not match.");
//            if (statusInquiryResponse.order_id != order_id) throw new DomainException("Request and Response Order Ids do not match.");
//            if (statusInquiryResponse.order_status != response.order_status) throw new DomainException("Request and Response Order Statuses do not match.");
//            if (statusInquiryResponse.bank_ref_no != response.bank_ref_no) throw new DomainException("Request and Response Bank Ref Nos. do not match.");
//        }

//        if (isBackgroundProcess)
//        {
//            if (response.trans_date.Trim().Length == 0)
//            {
//                ResponseDateTime = DTU.GetCurrentDateTime();
//            }
//            else
//            {
//                ResponseDateTime = response.trans_date.Replace(" ", "-").Replace(":", "-").Replace(".", "-");
//            }

//            Rechecked = true;
//        }
//        else
//        {
//            var checkCount = Utils.GetInt32(DataAccessUtils.GetScalarValueByQuery(cmd, $"SELECT COUNT(Ref) FROM {MasterTableName}" +
//                $" WHERE Ref = {order_id} AND (LEN(tracking_id) > 0 OR LEN(bank_ref_no) > 0)"));

//            if (checkCount > 0) throw new DomainException("This Order has already been processed.");

//            ResponseDateTime = DTU.GetCurrentDateTime();
//            trans_date = response.trans_date;
//        }

//        tracking_id = response.tracking_id;
//        bank_ref_no = response.bank_ref_no;
//        order_status = response.order_status;
//        failure_message = response.failure_message;
//        payment_mode = response.payment_mode;
//        card_name = response.card_name;
//        status_code = response.status_code;
//        status_message = response.status_message;
//        vault = response.vault;
//        mer_amount = Utils.GetDecimal(response.mer_amount, 2);
//        eci_value = response.eci_value;
//        retry = response.retry;
//        response_code = response.response_code;
//        billing_notes = response.billing_notes;
//        bin_country = response.bin_country;
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

//    public string redirect_url
//    {
//        get => GetStringPropertyValue("redirect_url");
//        set => SetStringPropertyValue("redirect_url", value);
//    }

//    public string cancel_url
//    {
//        get => GetStringPropertyValue("cancel_url");
//        set => SetStringPropertyValue("cancel_url", value);
//    }

//    public string billing_name
//    {
//        get => GetStringPropertyValue("billing_name");
//        set => SetStringPropertyValue("billing_name", value);
//    }

//    public string merchant_param1
//    {
//        get => GetStringPropertyValue("merchant_param1");
//        set => SetStringPropertyValue("merchant_param1", value);
//    }

//    public string merchant_param2
//    {
//        get => GetStringPropertyValue("merchant_param2");
//        set => SetStringPropertyValue("merchant_param2", value);
//    }

//    public string merchant_param3
//    {
//        get => GetStringPropertyValue("merchant_param3");
//        set => SetStringPropertyValue("merchant_param3", value);
//    }

//    public string merchant_param4
//    {
//        get => GetStringPropertyValue("merchant_param4");
//        set => SetStringPropertyValue("merchant_param4", value);
//    }

//    public string merchant_param5
//    {
//        get => GetStringPropertyValue("merchant_param5");
//        set => SetStringPropertyValue("merchant_param5", value);
//    }

//    public string tracking_id
//    {
//        get => GetStringPropertyValue("tracking_id");
//        set => SetStringPropertyValue("tracking_id", value);
//    }

//    public string bank_ref_no
//    {
//        get => GetStringPropertyValue("bank_ref_no");
//        set => SetStringPropertyValue("bank_ref_no", value);
//    }

//    public string order_status
//    {
//        get => GetStringPropertyValue("order_status");
//        set => SetStringPropertyValue("order_status", value);
//    }

//    public string failure_message
//    {
//        get => GetStringPropertyValue("failure_message");
//        set => SetStringPropertyValue("failure_message", value);
//    }

//    public string payment_mode
//    {
//        get => GetStringPropertyValue("payment_mode");
//        set => SetStringPropertyValue("payment_mode", value);
//    }

//    public string card_name
//    {
//        get => GetStringPropertyValue("card_name");
//        set => SetStringPropertyValue("card_name", value);
//    }

//    public string status_code
//    {
//        get => GetStringPropertyValue("status_code");
//        set => SetStringPropertyValue("status_code", value);
//    }

//    public string status_message
//    {
//        get => GetStringPropertyValue("status_message");
//        set => SetStringPropertyValue("status_message", value);
//    }

//    public string vault
//    {
//        get => GetStringPropertyValue("vault");
//        set => SetStringPropertyValue("vault", value);
//    }

//    public decimal mer_amount
//    {
//        get => GetDecimalPropertyValue("mer_amount", 2);
//        set => SetDecimalPropertyValue("mer_amount", value);
//    }

//    public string eci_value
//    {
//        get => GetStringPropertyValue("eci_value");
//        set => SetStringPropertyValue("eci_value", value);
//    }

//    public string retry
//    {
//        get => GetStringPropertyValue("retry");
//        set => SetStringPropertyValue("retry", value);
//    }

//    public string response_code
//    {
//        get => GetStringPropertyValue("response_code");
//        set => SetStringPropertyValue("response_code", value);
//    }

//    public string billing_notes
//    {
//        get => GetStringPropertyValue("billing_notes");
//        set => SetStringPropertyValue("billing_notes", value);
//    }

//    public string trans_date
//    {
//        get => GetStringPropertyValue("trans_date");
//        set
//        {
//            SetStringPropertyValue("trans_date", value);
//            this.TransDateTimeInSystemFormat = CCAvenue_HDFC_Response_Parameters.ConvertResponseTransDateToTransDateTimeInSystemFormat(value);
//        }
//    }

//    public string bin_country
//    {
//        get => GetStringPropertyValue("bin_country");
//        set => SetStringPropertyValue("bin_country", value);
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

//    public static string FormulatePostActionURLString(CCAvenue_HDFC_Order ordr, CCAvenue_HDFC_PG_Credentials cred)
//    {
//        CCACrypto ccaCrypto = new CCACrypto();

//        var ccaRequest = $"merchant_id={cred.MerchantId}&" +
//            $"order_id={ordr.order_id}&" +
//            $"currency={ordr.currency}&" +
//            $"amount={ordr.amount.ToString("0.00")}&" +
//            $"redirect_url={ordr.redirect_url}&" +
//            $"cancel_url={ordr.cancel_url}&" +
//            $"billing_name={ordr.billing_name}&";

//        string strEncRequest = ccaCrypto.Encrypt(ccaRequest, cred.WorkingKey);
//        var requestString = $"encRequest={strEncRequest}&access_code={cred.AccessCode}";

//        var result = $"{cred.PostActionURL}&{requestString}";

//        return result;
//    }

//    public static CCAvenue_HDFC_Response_Parameters FormulateResponseParameters(long campusRef, string resp)
//    {
//        CCAvenue_HDFC_PG_Credentials cred = CCAvenue_HDFC_PG_Credentials.ReadCredentials(campusRef);

//        CCAvenue_HDFC_Response_Parameters result = new CCAvenue_HDFC_Response_Parameters();

//        CCACrypto ccaCrypto = new CCACrypto();
//        string encResponse = ccaCrypto.Decrypt(resp, cred.WorkingKey);

//        string[] segments = encResponse.Split('&');
        
//        foreach (string seg in segments)
//        {
//            string[] parts = seg.Split('=');
//            if (parts.Length > 0)
//            {
//                string key = parts[0].Trim();
//                string value = parts[1].Trim();

//                if (key == "order_id") result.order_id = value;
//                if (key == "tracking_id") result.tracking_id = value;
//                if (key == "bank_ref_no") result.bank_ref_no = value;
//                if (key == "order_status") result.order_status = value;
//                if (key == "failure_message") result.failure_message = value;
//                if (key == "payment_mode") result.payment_mode = value;
//                if (key == "card_name") result.card_name = value;
//                if (key == "status_code") result.status_code = value;
//                if (key == "status_message") result.status_message = value;
//                if (key == "currency") result.currency = value;
//                if (key == "amount") result.amount = value;
//                if (key == "billing_name") result.billing_name = value;
//                if (key == "merchant_param1") result.merchant_param1 = value;
//                if (key == "merchant_param2") result.merchant_param2 = value;
//                if (key == "merchant_param3") result.merchant_param3 = value;
//                if (key == "merchant_param4") result.merchant_param4 = value;
//                if (key == "merchant_param5") result.merchant_param5 = value;
//                if (key == "vault") result.vault = value;
//                if (key == "mer_amount") result.mer_amount = value;
//                if (key == "eci_value") result.eci_value = value;
//                if (key == "retry") result.retry = value;
//                if (key == "response_code") result.response_code = value;
//                if (key == "billing_notes") result.billing_notes = value;
//                if (key == "trans_date") result.trans_date = value;
//                if (key == "bin_country") result.bin_country = value;
//            }
//        }

//        return result;
//    }

//    public static string FormulateStatusInquiryString(long campusRef, CCAvenue_HDFC_Response_Parameters resp_params)
//    {
//        CCACrypto ccaCrypto = new CCACrypto();

//        CCAvenue_HDFC_PG_Credentials cred = CCAvenue_HDFC_PG_Credentials.ReadCredentials(campusRef);

//        JObject reqObject = new JObject
//        {
//            { "reference_no", resp_params.bank_ref_no }
//        };

//        var objString = JsonConvert.SerializeObject(reqObject);

//        string enc_objString = ccaCrypto.Encrypt(objString, cred.AccessCode);

//        var ccaRequest = $"enc_request={enc_objString}&access_code={cred.AccessCode}&command=orderStatusTracker&request_type=JSON&response_type=JSON&version=1.1";

//        //var result = $"{cred.StatusInquiryURL}&{ccaRequest}";

//        //return result;

//        return ccaRequest;
//    }

//    public static CCAvenue_HDFC_Response_Parameters FormulateResponseParametersFromStatusInquiry(long campusRef, string resp)
//    {
//        CCAvenue_HDFC_PG_Credentials cred = CCAvenue_HDFC_PG_Credentials.ReadCredentials(campusRef);

//        CCAvenue_HDFC_Response_Parameters result = new CCAvenue_HDFC_Response_Parameters();

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
//            string decryptedResponseString = ccaCrypto.Decrypt(encryptedResponseParts[1], cred.WorkingKey);

//            var responseObject = JObject.Parse(decryptedResponseString);

//            if (responseObject.ContainsKey("status"))
//            {
//                if (Utils.GetString(responseObject["status"]) == "1")
//                {
//                    throw new DomainException(Utils.GetString(responseObject["error_desc"]));
//                }
//            }

//            result.amount = Utils.GetString(responseObject["order_amt"]);
//            result.billing_name = Utils.GetString(responseObject["order_bill_name"]);
//            result.trans_date = Utils.GetString(responseObject["order_date_time"]);
//            result.currency = Utils.GetString(responseObject["order_currncy"]);
//            result.order_id = Utils.GetString(responseObject["order_no"]);
//            result.order_status = Utils.GetString(responseObject["order_status"]);
//            result.bank_ref_no = Utils.GetString(responseObject["order_bank_ref_no"]);
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
//                result.Add(new CCAvenue_HDFC_Order(data, td.MainData, allowEdit, allowInPlaceEditing));
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
//        if (redirect_url.Trim().Length == 0) vrec.AddToList(string.Empty, "Redirect URL cannot be blank.");
//        if (cancel_url.Trim().Length == 0) vrec.AddToList(string.Empty, "Cancel URL cannot be blank.");
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

//    public override void PerformPreSaveValidationProcess(DbCommand cmd, TransportData td, ValidationResultEntryCollection validationResultEntries)
//    {
//        //System.Diagnostics.Debug.WriteLine($"Committing Order Id {order_id} to DB!");
//    }

//    public override void PerformPreSaveProcess(DbCommand cmd, TransportData td, ValidationResultEntryCollection validationResultEntries)
//    {

//    }

//    public override void InvokePostSaveProcess(DbCommand cmd, TransportData tdContext, ValidationResultEntryCollection vrec)
//    {
//        //System.Diagnostics.Debug.WriteLine($"Committed Order Id {order_id} to DB!");
//    }

//    protected override List<BaseObject> DependentEntityGenerator(DbCommand cmd, TransportData td, ValidationResultEntryCollection validationResultEntries)
//    {
//        return new List<BaseObject>();
//    }
//}
