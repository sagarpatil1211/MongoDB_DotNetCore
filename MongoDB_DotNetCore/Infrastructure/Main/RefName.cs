public class RefName
{
    public RefName(long @ref, string name)
    {
        Ref = @ref;
        Name = name;
    }

    public RefName(long @ref, string name, string shortName)
    {
        Ref = @ref;
        Name = name;
        ShortName = shortName;
    }

    public long Ref { get; set; } = 0L;
    public string Name { get; set; } = string.Empty;

    private string m_shortName = null;

    public string ShortName
    {
        get
        {
            if (m_shortName == null) return Name;
            return m_shortName;
        }
        set
        {
            m_shortName = value;
        }
    }
}
