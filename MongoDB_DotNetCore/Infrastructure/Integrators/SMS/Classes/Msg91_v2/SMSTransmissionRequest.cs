public class SMSTransmissionRequest
{
    public string Message { get; set; } = string.Empty;
    public string PhoneNumbers { get; set; } = string.Empty;
    
    public static readonly SMSTransmissionRequest Blank = null;

    static SMSTransmissionRequest()
    {
        Blank = new SMSTransmissionRequest();
    }
}