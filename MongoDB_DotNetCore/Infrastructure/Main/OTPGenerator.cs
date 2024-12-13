using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

public class OTPGenerator
{
    public static string GenerateOTP()
    {
        var r = new Random();
        return r.Next(100000, 999999).ToString("D6");
    }
}
