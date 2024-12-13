using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class RazorPayIntegrator
{
    public static TransactionResult<Razorpay_Direct_Order> GenerateNewRazorpayOrder(long amountInSmallestCurrencyUnits, 
        string currency, bool autoCapturePayment, long entityRef, JObject notes)
    {
        TransactionResult<Razorpay_Direct_Order> result = new TransactionResult<Razorpay_Direct_Order>();

        try
        {
            var client = new RazorpayClient("rzp_test_h9NYGhxYGae4te", "NFcaKmUurn6CUx1a1MjJVZYT");

            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "amount", amountInSmallestCurrencyUnits }, // amount in the smallest currency unit
                { "currency", currency },
                { "payment_capture", autoCapturePayment ? "1" : "0" },
                { "receipt", entityRef.ToString() },
                { "notes", notes }
            };

            Order order = client.Order.Create(options);

            var rpo = JsonConvert.DeserializeObject<Razorpay_Direct_Order>(Utils.GetString(order.Attributes));

            result.TagType = "RazorPayOrder";
            result.Data = rpo;
            result.Successful = true;
        }
        catch (Exception ex)
        {
            result.AbsorbException(ex);
        }

        return result;
    }


    public static string GetPaymentStatus(string paymentId)
    {
        var result = string.Empty;

        var client = new RazorpayClient("rzp_test_h9NYGhxYGae4te", "NFcaKmUurn6CUx1a1MjJVZYT");

        var pymt = client.Payment.Fetch(paymentId);
        if (pymt != null)
        {
            var attr = JObject.Parse(pymt.Attributes.ToString());
            result = Utils.GetString(attr["status"]);
        }

        return result;
    }



    public static string GetPaymentIdAgainstOrderId(string orderId)
    {
        var result = string.Empty;

        var client = new RazorpayClient("rzp_test_h9NYGhxYGae4te", "NFcaKmUurn6CUx1a1MjJVZYT");

        var ordr = client.Order.Fetch(orderId);
        if (ordr != null)
        {
            var pymts = ordr.Payments();
            if (pymts.Count > 0)
            {
                var pymt = pymts[0];
                var attr = pymt.Attributes;

                result = JsonConvert.SerializeObject(attr);
            }
        }

        return result;
    }


    private static string GetHash(string text, string key)
    {
        // change according to your needs, an UTF8Encoding
        // could be more suitable in certain situations
        UTF8Encoding encoding = new UTF8Encoding();

        Byte[] textBytes = encoding.GetBytes(text);
        Byte[] keyBytes = encoding.GetBytes(key);

        Byte[] hashBytes;

        using (HMACSHA256 hash = new HMACSHA256(keyBytes))
            hashBytes = hash.ComputeHash(textBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public static bool VerifySignature(string orderId, string paymentId, string signature)
    {
        var text = orderId + "|" + paymentId;
        var secret = "NFcaKmUurn6CUx1a1MjJVZYT";

        var hash = GetHash(text, secret);

        return hash == signature;
    }
}
