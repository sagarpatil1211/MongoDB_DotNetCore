public class DateTimeRange
{
    public string FromDateTime { get; set; } = string.Empty;
    public string ToDateTime { get; set; } = string.Empty;

    public DateTimeRange(string fromDateTime = "", string toDateTime = "")
    {
        FromDateTime = fromDateTime;
        ToDateTime = toDateTime;
    }

    public bool Includes(string value)
    {
        return (string.Compare(FromDateTime, value) <= 0 && string.Compare(ToDateTime, value) >= 0);
    }

    public DateTimeRange CreateCopy()
    {
        return new DateTimeRange(FromDateTime, ToDateTime);
    }
}
