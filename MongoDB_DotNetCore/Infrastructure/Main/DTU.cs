using Humanizer;
using System;
using Humanizer.Localisation;

public class DTU
{
    private static readonly string timeZoneSeparator = "TZ";
    private static readonly string dateTimePartsSeparator = "-";

    public static string StringFromNumericValue(long value)
    {
        return ConvertToString(new DateTime(value));
    }

    public static long NumericValueFromString(string value)
    {
        return NumericValueFromDateTime(FromString(value));
    }

    public static long NumericValueFromDateTime(DateTime value)
    {
        return value.Ticks;
    }

    public static long ComputeDurationInMS(string fromDateTime, string toDateTime)
    {
        return Convert.ToInt64(DTU.FromString(toDateTime).Subtract(DTU.FromString(fromDateTime)).TotalMilliseconds);
    }

    public static long ComputeDurationInDays(string fromDateTime, string toDateTime)
    {
        return Convert.ToInt64(DTU.FromString(toDateTime).Subtract(DTU.FromString(fromDateTime)).TotalDays);
    }

    public static DateTime FromNumericValue(long value)
    {
        return new DateTime(value);
    }

    public static long TimeOfDayNumericFromNumericValue(long value)
    {
        return FromNumericValue(value).TimeOfDay.Ticks;
    }

    public static DateTime DateStartValueFromDateValue(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
    }

    public static DateTime DateStartValueFromString(string value)
    {
        var dt = FromString(value);
        return DateStartValueFromDateValue(dt);
    }

    public static string DateStartStringFromString(string value)
    {
        var dt = FromString(value);
        var dtDateStart = DateStartValueFromDateValue(dt);

        return ConvertToString(dtDateStart);
    }

    public static DateTime DateEndValueFromDateValue(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
    }

    public static DateTime DateEndValueFromString(string value)
    {
        var dt = FromString(value);
        return DateEndValueFromDateValue(dt);
    }

    public static string DateEndStringFromString(string value)
    {
        var dt = FromString(value);
        var dtDateEnd = DateEndValueFromDateValue(dt);

        return ConvertToString(dtDateEnd);
    }


    public static DateTime FromString(string value)
    {
        if (value.Trim().Length == 0) return DateTime.MinValue;

        var parts = value.Split(timeZoneSeparator, StringSplitOptions.RemoveEmptyEntries);

        var vDateTime = parts[0];

        var dateTimeParts = vDateTime.Split(dateTimePartsSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (dateTimeParts.Length == 3)
        {
            int year = Utils.GetInt32(dateTimeParts[0]);
            int month = Utils.GetInt32(dateTimeParts[1]);
            int day = Utils.GetInt32(dateTimeParts[2]);

            return new DateTime(year, month, day, 0, 0, 0, 0);
        }
        else
        {
            int year = Utils.GetInt32(dateTimeParts[0]);
            int month = Utils.GetInt32(dateTimeParts[1]);
            int day = Utils.GetInt32(dateTimeParts[2]);
            int hour = Utils.GetInt32(dateTimeParts[3]);
            int minute = Utils.GetInt32(dateTimeParts[4]);
            int second = Utils.GetInt32(dateTimeParts[5]);
            int millisecond = Utils.GetInt32(dateTimeParts[6]);

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }
    }

    public static string ConvertToString(DateTime value)
    {
        return $"{value.Year:D4}{dateTimePartsSeparator}" +
            $"{value.Month:D2}{dateTimePartsSeparator}" +
            $"{value.Day:D2}{dateTimePartsSeparator}" +
            $"{value.Hour:D2}{dateTimePartsSeparator}" +
            $"{value.Minute:D2}{dateTimePartsSeparator}" +
            $"{value.Second:D2}{dateTimePartsSeparator}" +
            $"{value.Millisecond:D3}";
    }

    public static string ConvertToDateOnlyString(DateTime value)
    {
        return $"{value.Year:D4}{dateTimePartsSeparator}" +
            $"{value.Month:D2}{dateTimePartsSeparator}" +
            $"{value.Day:D2}";
    }

    public static string ConvertToDateOnlyString(string strDateTime)
    {
        DateTime value = FromString(strDateTime);
        return ConvertToDateOnlyString(value);
    }

    public static string ConvertToTimeOnlyString(DateTime value)
    {
        return $"{value.Hour:D2}{dateTimePartsSeparator}" +
            $"{value.Minute:D2}{dateTimePartsSeparator}" +
            $"{value.Second:D2}";
    }

    public static DayOfWeek GetDayOfWeek(string strDateTime)
    {
        DateTime value = FromString(strDateTime);
        return value.DayOfWeek;
    }

    public static string ConvertToDayStartString(string value)
    {
        return ConvertToDayStartString(FromString(value));
    }

    public static string ConvertToDayStartString(DateTime value)
    {
        return $"{value.Year:D4}{dateTimePartsSeparator}" +
            $"{value.Month:D2}{dateTimePartsSeparator}" +
            $"{value.Day:D2}{dateTimePartsSeparator}" +
            $"00{dateTimePartsSeparator}00{dateTimePartsSeparator}00{dateTimePartsSeparator}000";
    }

    public static string ConvertToDayEndString(string value)
    {
        return ConvertToDayEndString(FromString(value));
    }

    public static string ConvertToDayEndString(DateTime value)
    {
        return $"{value.Year:D4}{dateTimePartsSeparator}" +
            $"{value.Month:D2}{dateTimePartsSeparator}" +
            $"{value.Day:D2}{dateTimePartsSeparator}" +
            $"23{dateTimePartsSeparator}59{dateTimePartsSeparator}59{dateTimePartsSeparator}999";
    }

    public static int GetYearValue(string value)
    {
        return FromString(value).Year;
    }

    public static string ValidateDateTimeRange(string fromDateTime, string toDateTime)
    {
        if (fromDateTime.CompareTo(toDateTime) > 0) return @"""To Date/Time"" cannot be prior to ""From Date/Time""";
        return string.Empty;
    }

    public static string AddMilliseconds(string dateTimeValue, int milliSeconds)
    {
        var dt = FromString(dateTimeValue).AddMilliseconds(milliSeconds);
        return ConvertToString(dt);
    }

    public static string GetMaxDateTime()
    {
        var dt = new DateTime(9999, 12, 31, 23, 59, 59, 999);
        return ConvertToString(dt);
    }

    public static string GetCurrentDateTime()
    {
        return ConvertToString(DateTime.Now);
    }

    public static long GetCurrentNumericDateTime()
    {
        return DateTime.Now.Ticks;
    }

    public static long GetEndOfDayNumeric()
    {
        var ts = new TimeSpan(0, 23, 59, 59, 999);
        return ts.Ticks;
    }

    public static PayloadPacket GetCurrentDateTime(PayloadPacket incomingPkt)
    {
        TransactionResult tr = new TransactionResult();

        var DAU = SessionController.DAU;

        var cToken = DAU.OpenConnectionAndBeginTransaction();

        try
        {
            if (incomingPkt.PayloadDescriptor == "CurrentDateTimeRequest")
            {
                //SystemUserOperationsManager.CheckUserSessionValidityForPerformingOperations(incomingPkt);

                //var req = CurrentDateTimeRequest.Deserialize(incomingPkt.Payload.ToString());

                //var resp = new CurrentDateTimeResponse() { DateTimeValue = GetCurrentDateTime() };

                //tr.Successful = true;
                //tr.Tag = resp.Serialize();
                tr.TagType = typeof(CurrentDateTimeResponse).FullName;
            }

            DAU.CommitTransaction(cToken);
        }
        catch (Exception ex)
        {
            DAU.RollbackTransaction(cToken);
            tr.AbsorbException(ex);
        }
        finally
        {
            DAU.CloseConnection(cToken);
        }

        PayloadPacket pktResult = PayloadPacket.FromTransactionResult(tr, incomingPkt);
        return pktResult;
    }

    public static string TimeSpanToString(TimeSpan ts)
    {
        return ts.ToString(@"hh\:mm\:ss");
    }

    public static string TimeSpanToStringWithMilliseconds(TimeSpan ts)
    {
        return ts.ToString(@"hh\:mm\:ss\.fff");
    }

    public static string TimeSpanToHumanReadableStringWithMilliseconds(TimeSpan ts)
    {
        return ts.Humanize(precision: 5, countEmptyUnits: false, minUnit: TimeUnit.Millisecond);
    }

    public static string GetDateOnlyString(string dateTimeString)
    {
        if (dateTimeString.Length < 10) return string.Empty;
        return dateTimeString.Substring(0, 10);
    }

    public static string AbbreviateTimeUnits(string input)
    {
        return input.Replace("minutes", "min")
            .Replace("minute", "min")
            .Replace("milliseconds", "ms")
            .Replace("millisecond", "ms")
            .Replace("seconds", "s")
            .Replace("second", "s");
    }
}
