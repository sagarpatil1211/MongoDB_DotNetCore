using System;
using System.IO;

public class Billdesk_PG_Credentials
{
    public string MerchantId = string.Empty;
    public string SecurityId = string.Empty;
    public string Key = string.Empty;
    public string CallbackURL = string.Empty;
    public string StatusInquiryURL = string.Empty;

    public static Billdesk_PG_Credentials ReadCredentials(long campusRef)
    {
        var result = new Billdesk_PG_Credentials();

        var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Billdesk_PG_Credentials_{campusRef}.cfg";

        var lines = File.ReadAllLines(filePath);
        if (lines.Length != 5) throw new DomainException("Cannot read payment gateway credentials.");

        result.MerchantId = lines[0].Trim();
        result.SecurityId = lines[1].Trim();
        result.Key = lines[2].Trim();
        result.CallbackURL = lines[3].Trim().Replace("{{campusRef}}", campusRef.ToString());
        result.StatusInquiryURL = lines[4].Trim();

        return result;
    }
}
