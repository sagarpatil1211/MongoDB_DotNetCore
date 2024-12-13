using System;
using System.Collections.Generic;
using System.Text;

public struct Razorpay_Direct_Order
{
    public string id;
    public string entity;
    public int amount;
    public int amount_paid;
    public int amount_due;
    public string currency;
    public string receipt;
    public string offer_id;
    public string status;
    public int attempts;
    public string[] notes;
    public long created_at;
}
