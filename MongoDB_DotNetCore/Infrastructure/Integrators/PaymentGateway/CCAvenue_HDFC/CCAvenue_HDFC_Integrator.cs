//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using RestSharp;
////using CCA.Util;
//using System.Linq;
//using Syncfusion.XlsIO;
//using System.IO;
//using Microsoft.AspNetCore.Http;
//using System.Data.Common;
////using DBObjectExtensions;

//public class CCA_HDFC_OrderNominalInfo
//{
//    public string OrderId { get; set; } = string.Empty;
//    public string TrackingId { get; set; } = string.Empty;

//    private string m_status = string.Empty;

//    public string Status
//    {
//        get
//        {
//            if (m_status.Trim().ToUpper() == "SHIPPED")
//            {
//                return "Success";
//            }
//            else
//            {
//                return m_status;
//            }
//        }
//        set
//        {
//            m_status = value;
//        }
//    }

//    public string ResponseDateTime { get; set; } = string.Empty;
//}


//public class CCAvenue_HDFC_Integrator
//{
//    public static void GetOrderStatus(DbCommand cmd, string orderId, out CCAvenue_HDFC_Response_Parameters result)
//    {
//        CCACrypto ccaCrypto = new CCACrypto();

//        long orderRef = long.Parse(orderId);

//        cmd.CommandText = $"SELECT CampusRef FROM {CCAvenue_HDFC_Order.MasterTableName} WHERE Ref = @OrderRef";
//        cmd.Parameters.Clear();
//        cmd.AddLongParameter("@OrderRef", orderRef);

//        long campusRef = Utils.GetInt64(cmd.ExecuteScalar2());

//        CCAvenue_HDFC_PG_Credentials cred = CCAvenue_HDFC_PG_Credentials.ReadCredentials(campusRef);

//        JObject reqObject = new JObject { { "order_no", orderId } };

//        var objString = JsonConvert.SerializeObject(reqObject);

//        string enc_objString = ccaCrypto.Encrypt(objString, cred.WorkingKey);

//        string ccaRequest = $"enc_request={enc_objString}&access_code={cred.AccessCode}&command=orderStatusTracker&request_type=JSON&response_type=JSON&version=1.1";

//        var client = new RestClient($"{cred.StatusInquiryURL}?{ccaRequest}");
//        var reqStatusInquiry = new RestRequest(Method.Post.ToString());

//        RestResponse response = client.Post(reqStatusInquiry);

//        string resp = response.Content;

//        result = new CCAvenue_HDFC_Response_Parameters();
//        result.order_id = orderId;

//        string[] segments = resp.Split('&');

//        string responseFormatErrorMessage = "Error in Response Format from PG";

//        //if (segments.Length < 2) return; // throw new DomainException(responseFormatErrorMessage);
//        if (segments.Length < 2) throw new DomainException(responseFormatErrorMessage);

//        string statusString = segments[0].Trim();
//        var statusStringParts = statusString.Split('=');
//        //if (statusStringParts.Length != 2) return; // throw new DomainException(responseFormatErrorMessage);
//        if (statusStringParts.Length != 2) throw new DomainException(responseFormatErrorMessage);

//        var statusValue = statusStringParts[1];

//        //if (statusValue != "0" && statusValue != "1") return; // throw new DomainException(responseFormatErrorMessage);
//        if (statusValue != "0" && statusValue != "1") throw new DomainException(responseFormatErrorMessage);

//        if (statusValue == "1")
//        {
//            //if (segments.Length != 3) return; // throw new DomainException(responseFormatErrorMessage);
//            if (segments.Length != 3) throw new DomainException(responseFormatErrorMessage);
//        }
//        else if (statusValue == "0")
//        {
//            //if (segments.Length != 2) return; // throw new DomainException(responseFormatErrorMessage);
//            if (segments.Length != 2) throw new DomainException(responseFormatErrorMessage);
//        }

//        if (statusValue == "1")
//        {
//            var errorCode = segments[2];
//            result.failure_message = errorCode;
//            result.order_status = "Failure";
//        }
//        else
//        {
//            var encryptedResponse = segments[1].Trim();

//            var encryptedResponseParts = encryptedResponse.Split('=');

//            //if (encryptedResponseParts.Length != 2) return; // throw new DomainException(responseFormatErrorMessage);
//            if (encryptedResponseParts.Length != 2) throw new DomainException(responseFormatErrorMessage);

//            string decryptedResponseString = ccaCrypto.Decrypt(encryptedResponseParts[1], cred.WorkingKey);

//            var responseObject = JObject.Parse(decryptedResponseString);

//            if (responseObject.ContainsKey("status"))
//            {
//                if (Utils.GetString(responseObject["status"]) == "1")
//                {
//                    result.failure_message = Utils.GetString(responseObject["error_desc"]);
//                    result.order_status = "Failure";
//                    //return;
//                    throw new DomainException(result.failure_message);
//                }
//            }

//            result.amount = Utils.GetString(responseObject["order_amt"]);
//            result.billing_name = Utils.GetString(responseObject["order_bill_name"]);
//            result.trans_date = Utils.GetString(responseObject["order_date_time"]);
//            result.currency = Utils.GetString(responseObject["order_currncy"]);
//            result.order_id = Utils.GetString(responseObject["order_no"]);

//            var order_status_string = Utils.GetString(responseObject["order_status"]);

//            result.order_status = order_status_string == "Shipped" ? "Success" : order_status_string;

//            result.tracking_id = Utils.GetString(responseObject["reference_no"]);
//            result.bank_ref_no = Utils.GetString(responseObject["order_bank_ref_no"]);
//            result.mer_amount = Utils.GetString(responseObject["order_gross_amt"]);
//        }
//    }

//    public static TransactionResult AcceptSingleMPRFileUploadAndAbsorbReport(IFormFileCollection files,
//        PayloadPacket incomingPkt, Action singleMPRReportProcessor)
//    {
//        TransactionResult tr = new TransactionResult();

//        var DAU = SessionController.DAU;

//        var cToken = DAU.OpenConnectionAndBeginTransaction();

//        try
//        {
//            SessionController.SUOM.CheckUserSessionValidityForPerformingOperations(incomingPkt);

//            SessionController.SUOM.UpdateSystemUserLastActiveDateTime(incomingPkt);

//            if (incomingPkt.PayloadDescriptor != "SingleMPRReport") throw new DomainException("Upload metadata is not in the correct format.");

//            foreach (var file in files)
//            {
//                if (file == null || file.Length == 0)
//                    throw new DomainException($"Only files with extension 'xlsx' are allowed.");

//                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

//                if (ext != ".xlsx")
//                {
//                    throw new DomainException($"Only files with extension 'xlsx' are allowed.");
//                }

//                var folderName = Path.Combine("SystemUploads", "Single_MPR_Reports");
//                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//                if (!Directory.Exists(folderPath))
//                {
//                    Directory.CreateDirectory(folderPath);
//                }

//                var fileName = $"Single_MPR_Report.xlsx";
//                var filePath = Path.Combine(folderPath, fileName);

//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    file.CopyTo(fileStream);
//                }

//                singleMPRReportProcessor?.Invoke();
//            }

//            DAU.CommitTransaction(cToken);
//            tr.Successful = true;
//        }
//        catch (Exception ex)
//        {
//            DAU.RollbackTransaction(cToken);
//            tr.AbsorbException(ex);
//        }
//        finally
//        {
//            DAU.CloseConnection(cToken);
//        }

//        return tr;
//    }

//    public static TransactionResult AcceptMDRFileUploadAndAbsorbReport(IFormFileCollection files, 
//        PayloadPacket incomingPkt, Action mdrReportProcessor)
//    {
//        TransactionResult tr = new TransactionResult();

//        var DAU = SessionController.DAU;

//        var cToken = DAU.OpenConnectionAndBeginTransaction();

//        try
//        {
//            SessionController.SUOM.CheckUserSessionValidityForPerformingOperations(incomingPkt);

//            SessionController.SUOM.UpdateSystemUserLastActiveDateTime(incomingPkt);

//            if (incomingPkt.PayloadDescriptor != "MDRFile") throw new DomainException("Upload metadata is not in the correct format.");

//            foreach (var file in files)
//            {
//                if (file == null || file.Length == 0)
//                    throw new DomainException($"Only files with extension 'xls' are allowed.");

//                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

//                if (ext != ".xls")
//                {
//                    throw new DomainException($"Only files with extension 'xls' are allowed.");
//                }

//                var folderName = Path.Combine("SystemUploads", "MDRReports");
//                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//                if (!Directory.Exists(folderPath))
//                {
//                    Directory.CreateDirectory(folderPath);
//                }

//                var fileName = $"MDR_Report.xls";
//                var filePath = Path.Combine(folderPath, fileName);

//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    file.CopyTo(fileStream);
//                }

//                mdrReportProcessor?.Invoke();
//            }

//            DAU.CommitTransaction(cToken);
//            tr.Successful = true;
//        }
//        catch (Exception ex)
//        {
//            DAU.RollbackTransaction(cToken);
//            tr.AbsorbException(ex);
//        }
//        finally
//        {
//            DAU.CloseConnection(cToken);
//        }

//        return tr;
//    }

//    public static List<Tuple<string, string, decimal>> GetSettlementDatesAndAmountsFromSingleMPRFile()
//    // Return Value : 0: TrackingId, 1: SettlementDateTime, 2: SettlementAmount
//    {
//        List<Tuple<string, string, decimal>> result = new List<Tuple<string, string, decimal>>();

//        var eng = new ExcelEngine();

//        IApplication application = eng.Excel;

//        application.DefaultVersion = ExcelVersion.Excel2007;

//        var folderName = Path.Combine("SystemUploads", "Single_MPR_Reports");
//        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        var fileName = $"Single_MPR_Report.xlsx";
//        var singleMPRReportFilePath = Path.Combine(folderPath, fileName);

//        if (!File.Exists(singleMPRReportFilePath)) return result;

//        using FileStream fStream = new FileStream(singleMPRReportFilePath, FileMode.Open);

//        try
//        {
//            IWorkbook workbook = application.Workbooks.Open(fStream, ExcelParseOptions.Default, false, "266753");

//            if (workbook.Worksheets.Count == 0) return result;

//            IWorksheet worksheet = workbook.Worksheets[0];

//            int rowCountInWorksheet = worksheet.Rows.Count();

//            for (int r = 0; r < rowCountInWorksheet; r++)
//            {
//                int rIndex = r + 1;

//                string trackingId = string.Empty;
//                string settlementDate = string.Empty;
//                decimal settlementAmount = 0.0M;

//                string recFmt = worksheet.Range[$"R{rIndex}"].DisplayText.Trim();
//                if (recFmt.StartsWith("'")) recFmt = recFmt[1..];

//                if (recFmt == "BAT" || recFmt == "CR" || recFmt == "SALE")
//                {
//                    trackingId = worksheet.Range[$"E{rIndex}"].DisplayText[1..].Trim();

//                    DateTime dtSettlement = DateTime.Parse(worksheet.Range[$"F{rIndex}"].DisplayText[1..].Trim());

//                    settlementDate = DTU.ConvertToString(dtSettlement);
//                    settlementAmount = Utils.GetDecimal(worksheet.Range[$"I{rIndex}"].DisplayText[1..], 2);
//                }

//                if (trackingId.Trim().Length > 0 && settlementDate.Trim().Length > 0
//                    && settlementAmount > 0.0M)
//                {
//                    result.Add(Tuple.Create(trackingId, settlementDate, settlementAmount));
//                }
//            }
//        }
//        finally
//        {
//            fStream.Close();
//        }

//        return result;
//    }

//    public static List<Tuple<string, string, decimal>> GetSettlementDatesAndAmountsFromMDRReportFile()
//    // Return Value : 0: TrackingId, 1: SettlementDateTime, 2: SettlementAmount
//    {
//        List<Tuple<string, string, decimal>> result = new List<Tuple<string, string, decimal>>();

//        var eng = new ExcelEngine();

//        IApplication application = eng.Excel;

//        application.DefaultVersion = ExcelVersion.Excel2007;

//        var folderName = Path.Combine("SystemUploads", "MDRReports");
//        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        var fileName = $"MDR_Report.xls";
//        var mdrReportFilePath = Path.Combine(folderPath, fileName);

//        if (!File.Exists(mdrReportFilePath)) return result;

//        using FileStream fStream = new FileStream(mdrReportFilePath, FileMode.Open);

//        try
//        {
//            IWorkbook workbook = application.Workbooks.Open(fStream);

//            if (workbook.Worksheets.Count == 0) return result;

//            IWorksheet worksheet = workbook.Worksheets[0];

//            int upiTransactionsRowIndex = int.MaxValue;

//            int rowCountInWorksheet = worksheet.Rows.Count();

//            for (int r = 0; r < rowCountInWorksheet; r++)
//            {
//                int rIndex = r + 1;

//                if (worksheet.Range[$"A{rIndex}"].DisplayText.StartsWith("UPI TRANSACTIONS"))
//                {
//                    upiTransactionsRowIndex = rIndex;
//                    break;
//                }
//            }

//            rowCountInWorksheet = worksheet.Rows.Count();

//            for (int r = 0; r < rowCountInWorksheet; r++)
//            {
//                int rIndex = r + 1;

//                if (worksheet.Range[$"A{rIndex}"].DisplayText.StartsWith("PG"))
//                {
//                    string trackingId = string.Empty;
//                    string settlementDate = string.Empty;
//                    decimal settlementAmount = 0.0M;

//                    if (rIndex < upiTransactionsRowIndex)
//                    {
//                        string recFmt = worksheet.Range[$"C{rIndex}"].DisplayText.Trim();

//                        if (recFmt == "BAT")
//                        {
//                            trackingId = worksheet.Range[$"N{rIndex}"].DisplayText.Substring(1);
//                            settlementDate = DTU.ConvertToString(DateTime.FromOADate(worksheet.Range[$"H{rIndex}"].Number));
//                            settlementAmount = Utils.GetDecimal(worksheet.Range[$"W{rIndex}"].DisplayText, 2);
//                        }
//                    }
//                    //else if (rIndex > upiTransactionsRowIndex)
//                    //{
//                    //    trackingId = worksheet.Range[$"H{rIndex}"].DisplayText.Substring(1);
//                    //    settlementDate = DTU.ConvertToString(DateTime.FromOADate(worksheet.Range[$"K{rIndex}"].Number));
//                    //    settlementAmount = Utils.GetDecimal(worksheet.Range[$"S{rIndex}"].DisplayText, 2);
//                    //}

//                    if (trackingId.Trim().Length > 0 && settlementDate.Trim().Length > 0
//                        && settlementAmount > 0.0M)
//                    {
//                        result.Add(Tuple.Create(trackingId, settlementDate, settlementAmount));
//                    }
//                }
//            }
//        }
//        finally
//        {
//            fStream.Close();
//        }

//        return result;
//    }

//    public static TransactionResult AcceptCCAvenuePayoutFileUploadAndAbsorbReport(IFormFileCollection files,
//        PayloadPacket incomingPkt, Action ccavenuePayoutReportProcessor)
//    {
//        TransactionResult tr = new TransactionResult();

//        var DAU = SessionController.DAU;

//        var cToken = DAU.OpenConnectionAndBeginTransaction();

//        try
//        {
//            SessionController.SUOM.CheckUserSessionValidityForPerformingOperations(incomingPkt);

//            SessionController.SUOM.UpdateSystemUserLastActiveDateTime(incomingPkt);

//            if (incomingPkt.PayloadDescriptor != "CCAvenuePayoutReport") throw new DomainException("Upload metadata is not in the correct format.");

//            foreach (var file in files)
//            {
//                if (file == null || file.Length == 0)
//                    throw new DomainException($"Only files with extension 'xlsx' are allowed.");

//                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

//                if (ext != ".xlsx")
//                {
//                    throw new DomainException($"Only files with extension 'xlsx' are allowed.");
//                }

//                var folderName = Path.Combine("SystemUploads", "CCAvenue_Payout_Reports");
//                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//                if (!Directory.Exists(folderPath))
//                {
//                    Directory.CreateDirectory(folderPath);
//                }

//                var fileName = $"CCAvenue_Payout_Report.xls";
//                var filePath = Path.Combine(folderPath, fileName);

//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    file.CopyTo(fileStream);
//                }

//                ccavenuePayoutReportProcessor?.Invoke();
//            }

//            DAU.CommitTransaction(cToken);
//            tr.Successful = true;
//        }
//        catch (Exception ex)
//        {
//            DAU.RollbackTransaction(cToken);
//            tr.AbsorbException(ex);
//        }
//        finally
//        {
//            DAU.CloseConnection(cToken);
//        }

//        return tr;
//    }

//    public static List<Tuple<string, string, decimal>> GetSettlementDatesAndAmountsFromCCAvenuePayoutReportFile()
//    // Return Value : 0: TrackingId, 1: SettlementDateTime, 2: SettlementAmount
//    {
//        List<Tuple<string, string, decimal>> result = new List<Tuple<string, string, decimal>>();

//        var eng = new ExcelEngine();

//        IApplication application = eng.Excel;

//        application.DefaultVersion = ExcelVersion.Excel2007;

//        var folderName = Path.Combine("SystemUploads", "CCAvenue_Payout_Reports");
//        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        var fileName = $"CCAvenue_Payout_Report.xls";
//        var ccavenuePayoutReportFilePath = Path.Combine(folderPath, fileName);

//        if (!File.Exists(ccavenuePayoutReportFilePath)) return result;

//        using FileStream fStream = new FileStream(ccavenuePayoutReportFilePath, FileMode.Open);

//        try
//        {
//            IWorkbook workbook = application.Workbooks.Open(fStream);

//            if (workbook.Worksheets.Count < 2) return result;

//            IWorksheet worksheet = workbook.Worksheets[1];

//            int rowCountInWorksheet = worksheet.Rows.Count();

//            for (int r = 0; r < rowCountInWorksheet; r++)
//            {
//                int rIndex = r + 1;

//                if (worksheet.Range[$"A{rIndex}"].DisplayText == "Shipped")
//                {
//                    string trackingId = worksheet.Range[$"J{rIndex}"].DisplayText;
//                    string settlementDate = DTU.ConvertToString(DateTime.ParseExact(worksheet.Range[$"R{rIndex}"].DisplayText, "dd-MM-yyyy", null));
//                    decimal settlementAmount = Utils.GetDecimal(worksheet.Range[$"I{rIndex}"].DisplayText, 2);

//                    if (trackingId.Trim().Length > 0 && settlementDate.Trim().Length > 0)
//                    {
//                        result.Add(Tuple.Create(trackingId, settlementDate, settlementAmount));
//                    }
//                }
//            }
//        }
//        finally
//        {
//            fStream.Close();
//        }

//        return result;
//    }

//    public static TransactionResult AcceptUPIPayoutFileUploadAndAbsorbReport(IFormFileCollection files,
//        PayloadPacket incomingPkt, Action UPIPayoutReportProcessor)
//    {
//        TransactionResult tr = new TransactionResult();

//        var DAU = SessionController.DAU;

//        var cToken = DAU.OpenConnectionAndBeginTransaction();

//        try
//        {
//            SessionController.SUOM.CheckUserSessionValidityForPerformingOperations(incomingPkt);

//            SessionController.SUOM.UpdateSystemUserLastActiveDateTime(incomingPkt);

//            if (incomingPkt.PayloadDescriptor != "UPIPayoutReport") throw new DomainException("Upload metadata is not in the correct format.");

//            foreach (var file in files)
//            {
//                if (file == null || file.Length == 0)
//                    throw new DomainException($"Only files with extension 'xlsx' are allowed.");

//                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

//                if (ext != ".xlsx")
//                {
//                    throw new DomainException($"Only files with extension 'xlsx' are allowed.");
//                }

//                var folderName = Path.Combine("SystemUploads", "UPI_Payout_Reports");
//                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//                if (!Directory.Exists(folderPath))
//                {
//                    Directory.CreateDirectory(folderPath);
//                }

//                var fileName = $"UPI_Payout_Report.xls";
//                var filePath = Path.Combine(folderPath, fileName);

//                using (var fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    file.CopyTo(fileStream);
//                }

//                UPIPayoutReportProcessor?.Invoke();
//            }

//            DAU.CommitTransaction(cToken);
//            tr.Successful = true;
//        }
//        catch (Exception ex)
//        {
//            DAU.RollbackTransaction(cToken);
//            tr.AbsorbException(ex);
//        }
//        finally
//        {
//            DAU.CloseConnection(cToken);
//        }

//        return tr;
//    }

//    public static List<Tuple<string, string, decimal>> GetSettlementDatesAndAmountsFromUPIPayoutReportFile()
//    // Return Value : 0: TrackingId, 1: SettlementDateTime, 2: SettlementAmount
//    {
//        List<Tuple<string, string, decimal>> result = new List<Tuple<string, string, decimal>>();

//        var eng = new ExcelEngine();

//        IApplication application = eng.Excel;

//        application.DefaultVersion = ExcelVersion.Excel2007;

//        var folderName = Path.Combine("SystemUploads", "UPI_Payout_Reports");
//        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        var fileName = $"UPI_Payout_Report.xls";
//        var UPIPayoutReportFilePath = Path.Combine(folderPath, fileName);

//        if (!File.Exists(UPIPayoutReportFilePath)) return result;

//        using FileStream fStream = new FileStream(UPIPayoutReportFilePath, FileMode.Open);

//        try
//        {
//            IWorkbook workbook = application.Workbooks.Open(fStream);

//            if (workbook.Worksheets.Count == 0) return result;

//            IWorksheet worksheet = workbook.Worksheets[0];

//            int rowCountInWorksheet = worksheet.Rows.Count();

//            for (int r = 0; r < rowCountInWorksheet; r++)
//            {
//                int rIndex = r + 1;

//                if (worksheet.Range[$"A{rIndex}"].DisplayText.StartsWith("PG"))
//                {
//                    string trackingId = worksheet.Range[$"H{rIndex}"].DisplayText;
//                    string settlementDate = DTU.ConvertToString(DateTime.Parse(worksheet.Range[$"K{rIndex}"].DisplayText));
//                    decimal settlementAmount = Utils.GetDecimal(worksheet.Range[$"S{rIndex}"].DisplayText, 2);

//                    if (trackingId.Trim().Length > 0 && settlementDate.Trim().Length > 0)
//                    {
//                        result.Add(Tuple.Create(trackingId, settlementDate, settlementAmount));
//                    }
//                }
//            }
//        }
//        finally
//        {
//            fStream.Close();
//        }

//        return result;
//    }

//    public static TransactionResult AcceptCCAvenueExcelFileUpload(IFormFileCollection files, PayloadPacket incomingPkt)
//    {
//        TransactionResult tr = new TransactionResult();

//        var DAU = SessionController.DAU;

//        var cToken = DAU.OpenConnectionAndBeginTransaction();

//        try
//        {
//            SessionController.SUOM.CheckUserSessionValidityForPerformingOperations(incomingPkt);

//            SessionController.SUOM.UpdateSystemUserLastActiveDateTime(incomingPkt);

//            if (incomingPkt.PayloadDescriptor != "CCAvenueExcelFile") throw new DomainException("Upload metadata is not in the correct format.");

//            var file = files[0];

//            if (file == null || file.Length == 0)
//                throw new DomainException($"Only files with extension 'xls' are allowed.");

//            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

//            if (ext != ".xls")
//            {
//                throw new DomainException($"Only files with extension 'xls' are allowed.");
//            }

//            var folderName = Path.Combine("SystemUploads", "CCAvenueExcelReports");
//            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//            if (!Directory.Exists(folderPath))
//            {
//                Directory.CreateDirectory(folderPath);
//            }

//            var fileName = $"CCAvenue_Excel_Report.xls";
//            var filePath = Path.Combine(folderPath, fileName);

//            using (var fileStream = new FileStream(filePath, FileMode.Create))
//            {
//                file.CopyTo(fileStream);
//            }

//            DAU.CommitTransaction(cToken);
//            tr.Successful = true;
//        }
//        catch (Exception ex)
//        {
//            DAU.RollbackTransaction(cToken);
//            tr.AbsorbException(ex);
//        }
//        finally
//        {
//            DAU.CloseConnection(cToken);
//        }

//        return tr;
//    }

//    public static List<CCA_HDFC_OrderNominalInfo> GetOrderNominalInfoListFromCCAvenueExcelReportFile()
//    {
//        List<CCA_HDFC_OrderNominalInfo> result = new List<CCA_HDFC_OrderNominalInfo>();

//        var eng = new ExcelEngine();

//        IApplication application = eng.Excel;

//        application.DefaultVersion = ExcelVersion.Excel2007;

//        var folderName = Path.Combine("SystemUploads", "CCAvenueExcelReports");
//        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        var fileName = $"CCAvenue_Excel_Report.xls";
//        var ccAvenueExcelReportFilePath = Path.Combine(folderPath, fileName);

//        if (!File.Exists(ccAvenueExcelReportFilePath)) return result;

//        using FileStream fStream = new FileStream(ccAvenueExcelReportFilePath, FileMode.Open);

//        IWorkbook workbook = application.Workbooks.Open(fStream);

//        if (workbook.Worksheets.Count == 0) return result;

//        IWorksheet worksheet = workbook.Worksheets[0];

//        static string GetSystemFormatDateTimeStringFromExcelString(string value)
//        {
//            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
//            if (parts.Length == 2)
//            {
//                var datePart = parts[0];
//                var timePart = parts[1];

//                timePart = timePart.Replace(':', '-');
//                timePart = timePart.Replace('.', '-');

//                return datePart + "-" + timePart;
//            }

//            return string.Empty;
//        }

//        int rowCountInWorksheet = worksheet.Rows.Count();

//        for (int r = 3; r <= rowCountInWorksheet; r++)
//        {
//            int rIndex = r;
            
//            string trackingId = worksheet.Range[$"A{rIndex}"].DisplayText.Trim();
//            string responseDateTime = GetSystemFormatDateTimeStringFromExcelString(worksheet.Range[$"E{rIndex}"].DisplayText.Trim());
//            string orderId = worksheet.Range[$"C{rIndex}"].DisplayText.Trim();
//            string status = worksheet.Range[$"AK{rIndex}"].DisplayText.Trim();

//            result.Add(new CCA_HDFC_OrderNominalInfo
//            {
//                OrderId = orderId,
//                ResponseDateTime = responseDateTime,
//                Status = status,
//                TrackingId = trackingId
//            });
//        }

//        return result;
//    }
//}
