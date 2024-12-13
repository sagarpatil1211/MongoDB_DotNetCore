//using System;
//using System.Collections.Generic;
//using System.Text;

//public class CCAvenue_HDFC_Response_Parameters
//{
//    public string order_id { get; set; } = string.Empty;
//    public string tracking_id { get; set; } = string.Empty;
//    public string bank_ref_no { get; set; } = string.Empty;
//    public string order_status { get; set; } = string.Empty;
//    public string failure_message { get; set; } = string.Empty;
//    public string payment_mode { get; set; } = string.Empty;
//    public string card_name { get; set; } = string.Empty;
//    public string status_code { get; set; } = string.Empty;
//    public string status_message { get; set; } = string.Empty;
//    public string currency { get; set; } = string.Empty;
//    public string amount { get; set; } = string.Empty;
//    public string billing_name { get; set; } = string.Empty;
//    public string merchant_param1 { get; set; } = string.Empty;
//    public string merchant_param2 { get; set; } = string.Empty;
//    public string merchant_param3 { get; set; } = string.Empty;
//    public string merchant_param4 { get; set; } = string.Empty;
//    public string merchant_param5 { get; set; } = string.Empty;
//    public string vault { get; set; } = string.Empty;
//    public string mer_amount { get; set; } = string.Empty;
//    public string eci_value { get; set; } = string.Empty;
//    public string retry { get; set; } = string.Empty;
//    public string response_code { get; set; } = string.Empty;
//    public string billing_notes { get; set; } = string.Empty;
//    public string trans_date { get; set; } = string.Empty;
//    public string bin_country { get; set; } = string.Empty;

//    public static string ConvertResponseTransDateToTransDateTimeInSystemFormat(string transDateTimeValue)
//    {
//        if (transDateTimeValue == "null") return DTU.GetCurrentDateTime();

//        var partsDateAndTime = transDateTimeValue.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
//        if (partsDateAndTime.Length != 2) throw new DomainException("Response Date & Time are not in the correct format.");

//        var datePart = partsDateAndTime[0];
//        var timePart = partsDateAndTime[1];

//        var dateSubParts = datePart.Split("/", StringSplitOptions.RemoveEmptyEntries);
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

//    public static CCAvenue_HDFC_Response_Parameters FromOrder(CCAvenue_HDFC_Order ordr)
//    {
//        var result = new CCAvenue_HDFC_Response_Parameters();

//        result.amount = ordr.amount.ToString("0.00");
//        result.bank_ref_no = ordr.bank_ref_no;
//        result.billing_name = ordr.billing_name;
//        result.billing_notes = ordr.billing_notes;
//        result.bin_country = ordr.bin_country;
//        result.card_name = ordr.card_name;
//        result.currency = ordr.currency;
//        result.eci_value = ordr.eci_value;
//        result.failure_message = ordr.failure_message;
//        result.merchant_param1 = ordr.merchant_param1;
//        result.merchant_param2 = ordr.merchant_param2;
//        result.merchant_param3 = ordr.merchant_param3;
//        result.merchant_param4 = ordr.merchant_param4;
//        result.merchant_param5 = ordr.merchant_param5;
//        result.mer_amount = ordr.mer_amount.ToString("0.00");
//        result.order_id = ordr.order_id;
//        result.order_status = ordr.order_status;
//        result.payment_mode = ordr.payment_mode;
//        result.response_code = ordr.response_code;
//        result.retry = ordr.retry;
//        result.status_code = ordr.status_code;
//        result.status_message = ordr.status_message;
//        result.tracking_id = ordr.tracking_id;
//        result.trans_date = ordr.trans_date;
//        result.vault = ordr.vault;

//        return result;
//    }
//}
