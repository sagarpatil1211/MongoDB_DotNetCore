//name1space VJCore.Infrastructure;

public enum PropertyTypes
{
    String = 1,
    Decimal = 2,
    Integer = 3,
    Long = 4,
    Boolean = 5,
    ByteArray = 6,
    Collection = 7,
    Complex = 8
}

public enum EntityRelationshipTypes
{
    Parent_Child = 1,
    Associative = 2
}

public enum Mps
{
    Cr = -1,
    Dr = 1
}

public enum StringHorizontalAlignment
{
    Left = 1,
    Center = 2,
    Right = 3,
    Justified = 4
}

public enum StringVerticalAlignment
{
    Top = 1,
    Middle = 2,
    Bottom = 3,
}

public enum StringAlignment
{
    TopLeft = 1,
    TopCenter = 2,
    TopRight = 3,
    MiddleLeft = 4,
    MiddleCenter = 5,
    MiddleRight = 6,
    BottomLeft = 7,
    BottomCenter = 8,
    BottomRight = 9
}

[PersistedEnum]
public enum Genders
{
    [EnumMemberString("Male")] Male = 1,
    [EnumMemberString("Female")] Female = 2,
    [EnumMemberString("Transgender")] Transgender = 3
}

[PersistedEnum]
public enum Locales
{
    [EnumMemberString("Rural")] Rural = 1,
    [EnumMemberString("Urban")] Urban = 2
}

[PersistedEnum]
public enum MaritalStatus
{
    [EnumMemberString("Un-Married")] UnMarried = 1,
    [EnumMemberString("Married")] Married = 2,
    [EnumMemberString("Divorced")] Divorced = 3
}

[PersistedEnum]
public enum ModesOfPayment
{
    [EnumMemberString("Cash")] Cash = 1,
    [EnumMemberString("Cheque")] Cheque = 2,
    [EnumMemberString("Credit Card")] CreditCard = 3,
    [EnumMemberString("Bank")] Bank = 4
}

[PersistedEnum]
public enum LatitudeDirections
{
    [EnumMemberString("Zero")] Zero = 0,
    [EnumMemberString("N")] North = 1,
    [EnumMemberString("S")] South = 2
}

[PersistedEnum]
public enum LongitudeDirections
{
    [EnumMemberString("Zero")] Zero = 0,
    [EnumMemberString("E")] East = 1,
    [EnumMemberString("W")] West = 2
}

public enum RequestTypes
{
    Save = 1,
    Deletion = 2,
    Fetch = 3,
    CustomProcess = 4,
    GenerateDocument = 5
}

[PersistedEnum]
public enum BloodGroups
{
    [EnumMemberString("Unknown")] Unknown = 0,
    [EnumMemberString("A+")] A_Positive = 1,
    [EnumMemberString("A-")] A_Negative = 2,
    [EnumMemberString("B+")] B_Positive = 3,
    [EnumMemberString("B-")] B_Negative = 4,
    [EnumMemberString("O+")] O_Positive = 5,
    [EnumMemberString("O-")] O_Negative = 6,
    [EnumMemberString("AB+")] AB_Positive = 7,
    [EnumMemberString("AB-")] AB_Negative = 8
}

[PersistedEnum]
public enum NumberingSeriesTypes
{
    [EnumMemberString("Daily")] Daily = 1,
    [EnumMemberString("Monthly")] Monthly = 2,
    [EnumMemberString("Yearly")] Yearly = 3,
    [EnumMemberString("Continuous")] Continuous = 4
}

[PersistedEnum]
public enum PrintingOrientations
{
    [EnumMemberString("None")] None = 0,
    [EnumMemberString("Portrait")] Portrait = 1,
    [EnumMemberString("Landscape")] Landscape = 2
}

[PersistedEnum]
public enum PlatformArchitectureCombinations
{
    [EnumMemberString("Win_x86")] Win_x86 = 1,
    [EnumMemberString("Win_x64")] Win_x64 = 2,
    [EnumMemberString("Linux_x86")] Linux_x86 = 3,
    [EnumMemberString("Linux_x64")] Linux_x64 = 4,
    [EnumMemberString("Linux_Arm")] Linux_Arm = 5,
    [EnumMemberString("Linux_Arm64")] Linux_Arm64 = 6
}

[PersistedEnum]
public enum EMailSSLConnectionOptions
{
    None = 0,
    Auto = 1,
    SslOnConnect = 2,
    StartTls = 3,
    StartTlsWhenAvailable = 4
}

public enum DateTimeRangeBucketTypes
{
    Seconds = 1,
    Minutes = 2,
    FifteenMinutes = 3,
    HalfHours = 4,
    Hours = 5,
    ThreeHours = 6,
    SixHours = 7,
    EightHours = 8,
    TwelveHours = 9,
    Days = 10,
    Weeks = 11,
    Fortnights = 12,
    Months = 13,
    Quarters = 14,
    Trimesters = 15,
    Semesters = 16,
    Years = 17
}