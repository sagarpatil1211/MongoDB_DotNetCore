using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CCAvenue_HDFC_PG_Credentials
{
    public string MerchantId = string.Empty;
    public string AccessCode = string.Empty;
    public string WorkingKey = string.Empty;
    public string RedirectURL = string.Empty;
    public string CancelURL = string.Empty;
    public string PostActionURL = string.Empty;
    public string StatusInquiryURL = string.Empty;

    public static CCAvenue_HDFC_PG_Credentials ReadCredentials(long campusRef)
    {
        var result = new CCAvenue_HDFC_PG_Credentials();

        var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}CCAvenue_HDFC_PG_Credentials_{campusRef}.cfg";

        var lines = File.ReadAllLines(filePath);
        if (lines.Length != 7) throw new DomainException("Cannot read payment gateway credentials.");

        result.MerchantId = lines[0].Trim();
        result.AccessCode = lines[1].Trim();
        result.WorkingKey = lines[2].Trim();
        result.RedirectURL = lines[3].Trim().Replace("{{campusRef}}", campusRef.ToString());
        result.CancelURL = lines[4].Trim().Replace("{{campusRef}}", campusRef.ToString());
        result.PostActionURL = lines[5].Trim();
        result.StatusInquiryURL = lines[6].Trim();

        return result;
    }
}
