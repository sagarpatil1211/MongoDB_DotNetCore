public class Billdesk_Request_Parameters
{
    public string merchant_id { get; set; } = string.Empty;
    public string order_id { get; set; } = string.Empty;
    public string trans_date { get; set; } = string.Empty;
    public string amount { get; set; } = string.Empty;
    public string currency { get; set; } = string.Empty;
    public string security_id { get; set; } = string.Empty;
    public string additional_info1 { get; set; } = string.Empty; // Customer EMail Id
    public string additional_info2 { get; set; } = string.Empty; // Customer Mobile No
    public string additional_info3 { get; set; } = string.Empty; // Customer / Student Name
    public string additional_info4 { get; set; } = string.Empty; // Destination Bank Account Indicator
    public string additional_info5 { get; set; } = string.Empty;
    public string additional_info6 { get; set; } = string.Empty;
    public string additional_info7 { get; set; } = string.Empty;
    public string checksum { get; set; } = string.Empty;

    public bool enable_child_window_posting { get; set; } = false;
    public bool enable_payment_retry { get; set; } = false;
    public int retry_attempt_count { get; set; } = 0;
    public string pay_category { get; set; } = string.Empty;

    public string callback_url { get; set; } = string.Empty;

    public string order_status { get; set; } = string.Empty;
}
