using Newtonsoft.Json.Linq;

namespace VJCore.Infrastructure.Extensions;

public static class ConversionExtensions
{
    public static int ToInt32(this object input)
    {
        return Utils.GetInt32(input);
    }

    public static int ToInt32(this JToken input)
    {
        return Utils.GetInt32(input);
    }

    public static long ToInt64(this object input)
    {
        return Utils.GetInt64(input);
    }

    public static long ToInt64(this JToken input)
    {
        return Utils.GetInt64(input);
    }

    public static decimal ToIntDecimal(this object input, int decimalPlaces)
    {
        return Utils.GetDecimal(input, decimalPlaces);
    }

    public static bool ToBool(this object input)
    {
        return Utils.GetBoolean(input);
    }

    public static bool ToBool(this JToken input)
    {
        return Utils.GetBoolean(input);
    }

    public static string ObjectToString(this object input)
    {
        return Utils.GetString(input);
    }
}
