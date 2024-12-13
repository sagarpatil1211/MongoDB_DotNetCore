public interface IPaymentGatewayOrder
{
    public long Ref { get; set; }
    public string ResponseDateTime { get; set; }
    public string order_id => Utils.GetString(Ref);
    public string bank_ref_no { get; set; }
}
