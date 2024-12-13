using System;
using System.Collections.Generic;
using System.Text;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public static void ThrowFromTransactionResultIfRequired(TransactionResult tr)
    {
        if (tr.Successful) return;
        throw new DomainException(tr.Message);
    }
}
