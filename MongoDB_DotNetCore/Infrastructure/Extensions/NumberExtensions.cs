namespace VJCore.Infrastructure.Extensions;

public static class NumberExtensions
{
    public static int Inch(this int value)
    {
        return value * 96;
    }

    public static long Inch(this long value)
    {
        return value * 96;
    }

    public static decimal Inch(this decimal value)
    {
        return value * 96;
    }
}
