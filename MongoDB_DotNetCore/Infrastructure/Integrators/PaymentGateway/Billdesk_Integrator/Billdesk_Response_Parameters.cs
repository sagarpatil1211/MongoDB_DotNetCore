//using System;

//public class Billdesk_Response_Parameters
//{
//    public string merchant_id { get; set; } = string.Empty;
//    public string order_id { get; set; } = string.Empty;
//    public string txn_reference { get; set; } = string.Empty;
//    public string bank_reference { get; set; } = string.Empty;
//    public string txn_amount { get; set; } = string.Empty;
//    public string currency { get; set; } = string.Empty;
//    public string txn_date { get; set; } = string.Empty;
//    public string auth_status { get; set; } = string.Empty;
//    public string additional_info1 { get; set; } = string.Empty; // Customer EMail Id
//    public string additional_info2 { get; set; } = string.Empty; // Customer Mobile No
//    public string additional_info3 { get; set; } = string.Empty; // Customer / Student Name
//    public string additional_info4 { get; set; } = string.Empty; // Destination Bank Account Indicator
//    public string additional_info5 { get; set; } = string.Empty;
//    public string additional_info6 { get; set; } = string.Empty;
//    public string additional_info7 { get; set; } = string.Empty;
//    public string error_status { get; set; } = string.Empty;
//    public string error_description { get; set; } = string.Empty;
//    public string response_checksum { get; set; } = string.Empty;

//    public string order_status => auth_message;

//    public string auth_message
//    {
//        get
//        {
//            switch (auth_status)
//            {
//                case "0300" : return "Success";
//                case "0399": return "Failure";
//                case "NA": return "Error Condition";
//                case "0002": return "Pending/Abandoned";
//                case "0001": return "Technical Error";
//                default: return "Error : Exact Error not known.";
//            }
//        }
//    }

//    public static string ConvertResponseTransDateToTransDateTimeInSystemFormat(string transDateTimeValue)
//    {
//        if (transDateTimeValue == "null") return DTU.GetCurrentDateTime();

//        var partsDateAndTime = transDateTimeValue.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
//        if (partsDateAndTime.Length != 2) throw new DomainException("Response Date & Time are not in the correct format.");

//        var datePart = partsDateAndTime[0];
//        var timePart = partsDateAndTime[1];

//        var dateSubParts = datePart.Split("-", StringSplitOptions.RemoveEmptyEntries);
//        if (dateSubParts.Length != 3) throw new DomainException("Response Date & Time are not in the correct format.");

//        var intDay = Utils.GetInt32(dateSubParts[0]);
//        var intMonth = Utils.GetInt32(dateSubParts[1]);
//        var intYear = Utils.GetInt32(dateSubParts[2]);

//        var timeSubParts = timePart.Split(":", StringSplitOptions.RemoveEmptyEntries);
//        if (timeSubParts.Length != 3) throw new DomainException("Response Date & Time are not in the correct format.");

//        var intHour = Utils.GetInt32(timeSubParts[0]);
//        var intMinute = Utils.GetInt32(timeSubParts[1]);
//        var intSecond = Utils.GetInt32(timeSubParts[2]);

//        var dt = new DateTime(intYear, intMonth, intDay, intHour, intMinute, intSecond, 0);

//        return DTU.ConvertToString(dt);
//    }

//    public static Billdesk_Response_Parameters FromOrder(Billdesk_Order ordr)
//    {
//        var result = new Billdesk_Response_Parameters();

//        result.txn_amount = ordr.amount.ToString("0.00");
//        result.bank_reference = ordr.bank_reference;
//        result.additional_info3 = ordr.billing_name;
//        result.currency = ordr.currency;
//        result.error_status = ordr.error_status;
//        result.error_description = ordr.error_description;
//        result.additional_info1 = ordr.additional_info1;
//        result.additional_info2 = ordr.additional_info2;
//        result.additional_info3 = ordr.additional_info3;
//        result.additional_info4 = ordr.additional_info4;
//        result.additional_info5 = ordr.additional_info5;
//        result.txn_amount = ordr.txn_amount.ToString("0.00");
//        result.order_id = ordr.order_id;
//        //result.order_status = ordr.order_status;
//        result.response_checksum = ordr.response_checksum;
//        result.auth_status = ordr.auth_status;
//        result.txn_reference = ordr.txn_reference;
//        result.txn_date = ordr.trans_date;

//        return result;
//    }
//}
