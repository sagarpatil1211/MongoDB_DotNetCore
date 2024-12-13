//using System;
//using System.Text;
//using System.Security.Cryptography;
//using System.Data.Common;
////using DBObjectExtensions;

//public class Billdesk_Integrator
//{
//    //public static void GetOrderStatus(DbCommand cmd, string orderId, out Billdesk_Response_Parameters result)
//    //{
//    //    long orderRef = long.Parse(orderId);

//    //    cmd.CommandText = $"SELECT CampusRef FROM {Billdesk_Order.MasterTableName} WHERE Ref = @OrderRef";
//    //    cmd.AddLongParameter("@OrderRef", orderRef);

//    //    long campusRef = Utils.GetInt64(cmd.ExecuteScalar2());

//    //    if (campusRef == 0) throw new DomainException("Campus Reference not found.");

//    //    Billdesk_PG_Credentials cred = Billdesk_PG_Credentials.ReadCredentials(campusRef);
//    //    result = null;
//    //}

//    public static string GetHMACSHA256(string text, string key)
//    {
//        UTF8Encoding encoder = new UTF8Encoding();

//        byte[] hashValue;
//        byte[] keybyt = encoder.GetBytes(key);
//        byte[] message = encoder.GetBytes(text);

//        HMACSHA256 hashString = new HMACSHA256(keybyt);
//        string hex = "";

//        hashValue = hashString.ComputeHash(message);
//        foreach (byte x in hashValue)
//        {
//            hex += String.Format("{0:x2}", x);
//        }

//        return hex.ToUpper();
//    }

//    public static Billdesk_PostActionObject FormulatePostActionObject(Billdesk_Order ordr,
//        Billdesk_PG_Credentials cred)
//    {
//        string strAdditionalInfo1 = ordr.additional_info1.Trim().Length > 0 ? ordr.additional_info1 : "NA";
//        string strAdditionalInfo2 = ordr.additional_info2.Trim().Length > 0 ? ordr.additional_info2 : "NA";
//        string strAdditionalInfo3 = ordr.additional_info3.Trim().Length > 0 ? ordr.additional_info3 : "NA";
//        string strAdditionalInfo4 = ordr.additional_info4.Trim().Length > 0 ? ordr.additional_info4 : "NA";
//        string strAdditionalInfo5 = ordr.additional_info5.Trim().Length > 0 ? ordr.additional_info5 : "NA";
//        string strAdditionalInfo6 = ordr.additional_info6.Trim().Length > 0 ? ordr.additional_info6 : "NA";
//        string strAdditionalInfo7 = ordr.additional_info7.Trim().Length > 0 ? ordr.additional_info7 : "ABC";

//        string rawMessage = $"{cred.MerchantId}|{ordr.Ref}|NA|{ordr.amount:0.00}|NA|NA|NA|INR|NA|R|{cred.SecurityId}|" +
//            $"NA|NA|F|{strAdditionalInfo1}|{strAdditionalInfo2}|{strAdditionalInfo3}|{strAdditionalInfo4}|" +
//            $"{strAdditionalInfo5}|{strAdditionalInfo6}|{strAdditionalInfo7}|NA";

//        string checksum = GetHMACSHA256(rawMessage, cred.Key);

//        string finalMsg = $"{rawMessage}|{checksum}";

//        return new Billdesk_PostActionObject
//        {
//            msg = finalMsg,
//            options = new Billdesk_PostActionObject.Options
//            {
//                enableChildWindowPosting = false,
//                enablePaymentRetry = false,
//                retry_attempt_count = 1
//            },
//            callbackUrl = cred.CallbackURL
//        };
//    }
//}

////txtPayCategory = "PRESICREDITDEBITNETBANKINGCASHCARDEMIUPI"
////txtPayCategory = "NETBANKING"
