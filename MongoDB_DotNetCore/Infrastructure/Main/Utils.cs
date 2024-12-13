using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public sealed partial class Utils
{
    private static readonly Dictionary<int, string> dictDayNames = null;

    static Utils()
    {
        dictDayNames = new Dictionary<int, string>();
        SetDayNames();
    }

    public static string FormulateQueryStringWithFilters(string query, List<string> lstFilterStrings)
    {
        string fString = Utils.CombineListIntoSingleString(lstFilterStrings, " AND ");

        return FormulateQueryStringWithFilterString(query, fString);
    }

    public static string FormulateQueryStringWithFilterString(string query, string strFilter)
    {
        string result = query;
        if (strFilter.Length > 0) result += $" WHERE {strFilter}";

        return result;
    }

    public static JObject CopyJObject(JObject input)
    {
        return JObject.Parse(input.ToString());
    }

    public static JObject ConvertDataRowToJObject(DataRow row)
    {
        JObject result = new JObject();

        foreach (DataColumn dc in row.Table.Columns)
        {
            result[dc.ColumnName] = JToken.FromObject(row[dc.ColumnName]);
        }

        return result;
    }

    private static void SetDayNames()
    {
        dictDayNames.Add(1, "First");
        dictDayNames.Add(2, "Second");
        dictDayNames.Add(3, "Third");
        dictDayNames.Add(4, "Fourth");
        dictDayNames.Add(5, "Fifth");
        dictDayNames.Add(6, "Sixth");
        dictDayNames.Add(7, "Seventh");
        dictDayNames.Add(8, "Eighth");
        dictDayNames.Add(9, "Ninth");
        dictDayNames.Add(10, "Tenth");
        dictDayNames.Add(11, "Eleventh");
        dictDayNames.Add(12, "Twelfth");
        dictDayNames.Add(13, "Thirteenth");
        dictDayNames.Add(14, "Fourteenth");
        dictDayNames.Add(15, "Fifteenth");
        dictDayNames.Add(16, "Sixteenth");
        dictDayNames.Add(17, "Seventeenth");
        dictDayNames.Add(18, "Eighteenth");
        dictDayNames.Add(19, "Ninteenth");
        dictDayNames.Add(20, "Twentieth");
        dictDayNames.Add(21, "Twenty First");
        dictDayNames.Add(22, "Twenty Second");
        dictDayNames.Add(23, "Twenty Third");
        dictDayNames.Add(24, "Twenty Fourth");
        dictDayNames.Add(25, "Twenty Fifth");
        dictDayNames.Add(26, "Twenty Sixth");
        dictDayNames.Add(27, "Twenty Seventh");
        dictDayNames.Add(28, "Twenty Eighth");
        dictDayNames.Add(29, "Twenty Ninth");
        dictDayNames.Add(30, "Thirtieth");
        dictDayNames.Add(31, "Thirty First");
    }

    public static List<string> FormulateStringList(DataTable dtb, string columnName,
                                                       string filterString = "", string sortPropertyName = "",
                                                       bool uniqueOnly = true)
    {
        string sortPropName = sortPropertyName;
        if (sortPropName.Trim().Length == 0) sortPropName = columnName;

        List<string> lst = new List<string>();

        foreach (DataRow dr in dtb.Select(filterString, sortPropName))
        {
            string value = Convert.ToString(dr[columnName]);

            if (uniqueOnly)
            {
                if (!lst.Contains(value)) lst.Add(value);
            }
            else
            {
                lst.Add(value);
            }
        }

        return lst;
    }

    public static List<string> FormulateStringList(IEnumerable<DataTable> lstDTB, string columnName,
                                                       string filterString = "", string sortPropertyName = "",
                                                       bool uniqueOnly = true)
    {
        string sortPropName = sortPropertyName;
        if (sortPropName.Trim().Length == 0) sortPropName = columnName;

        List<string> lst = new List<string>();

        foreach (DataTable dtb in lstDTB)
        {
            foreach (DataRow dr in dtb.Select(filterString, sortPropName))
            {
                string value = Convert.ToString(dr[columnName]);

                if (uniqueOnly)
                {
                    if (!lst.Contains(value)) lst.Add(value);
                }
                else
                {
                    lst.Add(value);
                }
            }
        }

        return lst;
    }

    public static List<long> FormulateLongList(DataTable dtb, string columnName, bool zeroIfEmpty = false,
                                                       string filterString = "", string sortPropertyName = "",
                                                       bool uniqueOnly = true)
    {
        string sortPropName = sortPropertyName;
        if (sortPropName.Trim().Length == 0) sortPropName = columnName;

        List<long> lst = new List<long>();

        foreach (DataRow dr in dtb.Select(filterString, sortPropName))
        {
            long value = Convert.ToInt64(dr[columnName]);

            if (uniqueOnly)
            {
                if (lst.BinarySearch(value) < 0)
                {
                    lst.Add(value);
                    lst.Sort();
                }
            }
            else
            {
                lst.Add(value);
                lst.Sort();
            }
        }

        if (lst.Count == 0)
        {
            if (zeroIfEmpty) lst.Add(0L);
        }

        return lst;
    }

    public static List<int> FormulateIntegerList(DataTable dtb, string columnName, bool zeroIfEmpty = false,
                                                       string filterString = "", string sortPropertyName = "",
                                                       bool uniqueOnly = true)
    {
        string sortPropName = sortPropertyName;
        if (sortPropName.Trim().Length == 0) sortPropName = columnName;

        List<int> lst = new List<int>();

        foreach (DataRow dr in dtb.Select(filterString, sortPropName))
        {
            int value = Convert.ToInt32(dr[columnName]);

            if (uniqueOnly)
            {
                if (lst.BinarySearch(value) < 0)
                {
                    lst.Add(value);
                    lst.Sort();
                }
            }
            else
            {
                lst.Add(value);
                lst.Sort();
            }
        }

        if (lst.Count == 0)
        {
            if (zeroIfEmpty) lst.Add(0);
        }

        return lst;
    }

    public static string FormulateRefsString(DataTable dtb, string columnName, bool zeroIfEmpty = false,
        string filterString = "", string sortPropertyName = "")
    {
        return Utils.FormulateRefsString(FormulateLongList(dtb, columnName, zeroIfEmpty, filterString, sortPropertyName), zeroIfEmpty);
    }

    public static string FormulateRefsString(IEnumerable<long> lst, bool zeroIfEmpty = false)
    {
        StringBuilder sb = new StringBuilder();

        foreach (long v in lst)
        {
            sb.Append(",");
            sb.Append(v.ToString());
        }

        if (sb.Length > 0) sb.Remove(0, 1);

        if (sb.Length == 0 && zeroIfEmpty) sb.Append("0");

        return sb.ToString();
    }

    public static string FormulateRefsString(IEnumerable<int> lst, bool zeroIfEmpty = false)
    {
        StringBuilder sb = new StringBuilder();

        foreach (int v in lst)
        {
            sb.Append(",");
            sb.Append(v.ToString());
        }

        if (sb.Length > 0) sb.Remove(0, 1);

        if (sb.Length == 0 && zeroIfEmpty) sb.Append("0");

        return sb.ToString();
    }

    public static bool GetBoolean(object input)
    {
        if (input == System.DBNull.Value) return false;
        return Convert.ToBoolean(input);
    }

    public static bool GetBoolean(JToken input)
    {
        return GetBoolean(input.ToObject(typeof(object)));
    }

    public static byte GetByte(object input)
    {
        if (input == null || input == System.DBNull.Value || (!byte.TryParse(input.ToString(), out _))) return 0;
        return Convert.ToByte(input);
    }

    public static bool IsNumeric(string value) => decimal.TryParse(value, out _);

    public static int GetInt32(object input)
    {
        if (input == null || input == System.DBNull.Value || (!int.TryParse(input.ToString(), out _))) return 0;
        return Convert.ToInt32(input);
    }

    public static int GetInt32(JToken input)
    {
        return GetInt32(input.ToObject(typeof(object)));
    }

    public static long GetInt64(object input)
    {
        if (input == null || input == System.DBNull.Value || (!long.TryParse(input.ToString(), out _))) return 0L;
        return Convert.ToInt64(input);
    }

    public static long GetInt64(JToken input)
    {
        return GetInt64(input.ToObject(typeof(object)));
    }

    public static decimal GetDecimal(object input, int decimalPlaces)
    {
        if (input == null || input == System.DBNull.Value || (!decimal.TryParse(input.ToString(), out _))) return 0M;
        return Math.Round(Convert.ToDecimal(input), decimalPlaces);
    }

    public static decimal GetDecimal(JToken input, int decimalPlaces)
    {
        return GetDecimal(input.ToObject(typeof(object)), decimalPlaces);
    }

    public static double GetDouble(object input, int decimalPlaces)
    {
        if (input == null || input == System.DBNull.Value || (!decimal.TryParse(input.ToString(), out _))) return 0.0;
        return Math.Round(Convert.ToDouble(input), decimalPlaces);
    }

    public static double GetDouble(JToken input, int decimalPlaces)
    {
        return GetDouble(input.ToObject(typeof(object)), decimalPlaces);
    }

    public static string GetString(object input)
    {
        if (input == null || input == System.DBNull.Value) return string.Empty;
        return input.ToString();
    }

    public static string GetString(JToken input)
    {
        return GetString(input.ToObject(typeof(object)));
    }

    public static string ConvertToProperCase(string input)
    {
        TextInfo ti = new CultureInfo("en-US", false).TextInfo;

        string outputInternal = ti.ToTitleCase(input.ToLower());

        string output = new Regex(@"\b(?!Xi\b)(X|XX|XXX|XL|L|LX|LXX|LXXX|XC|C)?(I|II|III|IV|V|VI|VII|VIII|IX)?\b",
            RegexOptions.IgnoreCase).Replace(outputInternal, match => match.Value.ToUpperInvariant());

        return output;
    }

    public static int GetTimeValueInSeconds(DateTime span)
    {
        return ((span.Hour * 3600) + (span.Minute * 60) + (span.Second));
    }
    public static TimeSpan GetTimeSpanFromSeconds(int seconds)
    {
        return new TimeSpan(0, 0, seconds);
    }
    public static string GetIndianDate(string dateTimeValue, string separator = "/")
    {
        if (dateTimeValue.Length == 0) return string.Empty;

        var dtValue = DTU.FromString(dateTimeValue);
        return $"{dtValue.Day.ToString("D2")}{separator}{dtValue.Month.ToString("D2")}{separator}{dtValue.Year.ToString("D4")}";
    }

    public static string GetIndianTime(string dateTimeValue, string separator = ":")
    {
        if (dateTimeValue.Length == 0) return string.Empty;

        var dtValue = DTU.FromString(dateTimeValue);

        int hourValue = dtValue.Hour;
        int minuteValue = dtValue.Minute;
        int secondValue = dtValue.Second;

        return $"{hourValue.ToString("D2")}{separator}{minuteValue.ToString("D2")}{separator}{secondValue.ToString("D2")}";
    }

    public static string GetIndianDateTime(string dateTimeValue, string separator = "/")
    {
        return $"{GetIndianDate(dateTimeValue, separator)} {GetIndianTime(dateTimeValue)}";
    }

    public static int GetMonthValue(int dateValue)
    {
        //This will return the absolute month value since Epoch
        var dt = DateTime.FromOADate(dateValue);
        return ((dt.Year - 1) * 12) + dt.Month;
    }

    public static string GetIndianNumber(decimal value, int decimalPlaces, bool blankIfZero = false)
    {
        if (value == 0)
        {
            if (blankIfZero) return string.Empty;
            if (decimalPlaces > 0)
            {
                return "0." + new string('0', decimalPlaces);
            }
            else
            {
                return "0";
            }
        }

        bool isNegative = false;

        if (value < 0) isNegative = true;

        value = Math.Round(Math.Abs(value), decimalPlaces);

        string strValue = value.ToString("#0." + new string('0', decimalPlaces));

        string intPart = strValue;
        if (decimalPlaces > 0) intPart = strValue.Substring(0, strValue.IndexOf("."));

        string decPart = string.Empty;
        if (decimalPlaces > 0) decPart = "." + strValue.Substring(strValue.IndexOf(".") + 1);

        string result = intPart + decPart;
        if (intPart.Length > 3)
        {
            result = intPart.Substring(intPart.Length - 3) + decPart;
            intPart = intPart.Substring(0, intPart.Length - 3);

            string tmpResult = string.Empty;
            bool removeLeadingZero = false;

            if ((Convert.ToDecimal(intPart.Length) / 2) != (Convert.ToInt32(intPart.Length) / 2))
            {
                removeLeadingZero = true;
                intPart = "0" + intPart;
            }

            int i;
            for (i = 0; i <= (intPart.Length - 2); i += 2)
            {
                tmpResult += intPart.Substring(i, 2) + ",";
            }

            if (removeLeadingZero)
            {
                tmpResult = tmpResult.Substring(1);
            }

            result = tmpResult + result;
        }

        if (isNegative) result = "-" + result;

        return result;
    }

    public static string Space(int chars)
    {
        return new string(' ', chars);
    }

    private static string GetWordsForNumber(int number)
    {
        string wfn = string.Empty;

        if (number > 0)
        {
            string strNumber = number.ToString("000000000");
            if (GetInt32(strNumber.Substring(0, 2)) > 0)
            {
                if (wfn.Length > 0) wfn += Space(1);

                wfn += GetWordForTens(GetInt32(strNumber.Substring(0, 2)));

                if (GetInt32(strNumber.Substring(0, 2)) == 1)
                {
                    wfn += " Crore";
                }
                else
                {
                    wfn += " Crores";
                }

            }

            if (GetInt32(strNumber.Substring(2, 2)) > 0)
            {
                if (wfn.Length > 0) wfn += Space(1);

                wfn += GetWordForTens(GetInt32(strNumber.Substring(2, 2)));

                if (GetInt32(strNumber.Substring(2, 2)) == 1)
                {
                    wfn += " Lakh";
                }
                else
                {
                    wfn += " Lakhs";
                }
            }

            if (GetInt32(strNumber.Substring(4, 2)) > 0)
            {
                if (wfn.Length > 0) wfn += Space(1);
                wfn += GetWordForTens(GetInt32(strNumber.Substring(4, 2))) + " Thousand";
            }

            if (GetInt32(Right(strNumber, 3)) > 0)
            {
                if (wfn.Length > 0) wfn += Space(1);
                wfn += GetWordForHundreds(GetInt32(Right(strNumber, 3)));
            }
        }

        return wfn;
    }

    private static string Right(string str, int places)
    {
        return str.Substring(str.Length - places);
    }

    private static string GetWordForTens(int Number)
    {
        string wft = string.Empty;
        char[] CA = Number.ToString("00").ToCharArray();

        if (GetInt32(CA[0].ToString()) > 1)
        {
            wft += GetWordForTensPlace(GetInt32(CA[0].ToString()));

        }
        else if (GetInt32(CA[0].ToString()) == 1)
        {
            if (GetInt32(CA[1].ToString()) > 0)
            {
                wft += GetWordForTeens(GetInt32(CA[1].ToString()));
            }
            else
            {
                wft += GetWordForTensPlace(GetInt32(CA[0].ToString()));
            }

            return wft;
        }

        if (GetInt32(CA[1].ToString()) > 0)
        {
            if (wft.Length > 0) wft += Space(1);
            wft += GetWordForUnitsPlace(GetInt32(CA[1].ToString()));
            return wft;
        }

        return wft;
    }

    private static string GetWordForHundreds(int Number)
    {
        string wfh = string.Empty;

        char[] CA = Number.ToString("000").ToCharArray();

        if (GetInt32(CA[0].ToString()) > 0)
        {
            wfh += GetWordForUnitsPlace(GetInt32(CA[0].ToString())) + " Hundred";
            if (GetInt32(Right(Number.ToString(), 2)) > 0) wfh += " And";
        }

        if (GetInt32(CA[1].ToString()) > 1)
        {
            if (wfh.Length > 0) wfh += Space(1);
            wfh += GetWordForTensPlace(GetInt32(CA[1].ToString()));
        }
        else if (GetInt32(CA[1].ToString()) == 1)
        {
            if (wfh.Length > 0) wfh += Space(1);

            if (GetInt32(CA[2].ToString()) > 0)
            {
                wfh += GetWordForTeens(GetInt32(CA[2].ToString()));
            }
            else
            {
                wfh += GetWordForTensPlace(GetInt32(CA[1].ToString()));
            }

            return wfh;
        }


        if (GetInt32(CA[2].ToString()) > 0)
        {
            if (wfh.Length > 0) wfh += Space(1);
            wfh += GetWordForUnitsPlace(GetInt32(CA[2].ToString()));
        }

        return wfh;
    }

    private static string GetWordForUnitsPlace(int I)
    {
        string wfi = I switch
        {
            1 => "One",
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            6 => "Six",
            7 => "Seven",
            8 => "Eight",
            9 => "Nine",
            _ => string.Empty,
        };
        return wfi;
    }

    private static string GetWordForTensPlace(int I)
    {
        string wfi = I switch
        {
            1 => "Ten",
            2 => "Twenty",
            3 => "Thirty",
            4 => "Forty",
            5 => "Fifty",
            6 => "Sixty",
            7 => "Seventy",
            8 => "Eighty",
            9 => "Ninty",
            _ => string.Empty,
        };
        return wfi;
    }

    private static string GetWordForTeens(int I)
    {
        string wfi = I switch
        {
            1 => "Eleven",
            2 => "Twelve",
            3 => "Thirteen",
            4 => "Fourteen",
            5 => "Fifteen",
            6 => "Sixteen",
            7 => "Seventeen",
            8 => "Eighteen",
            9 => "Nineteen",
            _ => string.Empty,
        };
        return wfi;
    }

    public static string DateInWords(string dateTimeString)
    {
        int dateValue = Convert.ToInt32(Math.Floor(DTU.FromString(dateTimeString).ToOADate()));
        return DateInWords(dateValue);
    }

    public static string DateInWords(int dateValue)
    {
        DateTime dt = DateTime.FromOADate(dateValue);

        string strDate = GetDayName(dt.Day) + " Day Of";
        strDate += Space(1) + GetMonthName(dt.Month);

        var strCentury = dt.Year.ToString().Substring(0, 2);

        if (strCentury == "19")
        {
            strDate += Space(1) + "Ninteen";
        }
        else if (strCentury == "20")
        {
            strDate += Space(1) + "Two Thousand";
        }

        strDate += Space(1) + GetWordsForNumber(GetInt32(dt.Year.ToString().Substring(2)));

        return strDate.Trim();
    }

    public static string GetMonthName(int month)
    {
        string result = month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => string.Empty,
        };
        return result;
    }

    public static string GetAlphabetForMonth(int month)
    {
        string result = month switch
        {
            1 => "A",
            2 => "B",
            3 => "C",
            4 => "D",
            5 => "E",
            6 => "F",
            7 => "G",
            8 => "H",
            9 => "I",
            10 => "J",
            11 => "K",
            12 => "L",
            _ => string.Empty,
        };
        return result;
    }

    public static string GetDayName(int day)
    {
        if (day > 0 & day <= 31) return dictDayNames[day];
        return string.Empty;
    }

    public static string GetCurrencyInWords(decimal Amount, string PrefixBeforeDecimal, string PrefixAfterDecimal)
    {
        string strWords = string.Empty;
        int PostDecimalPart = 0;

        int PreDecimalPart = Convert.ToInt32(Math.Floor(Amount));

        if (Amount != PreDecimalPart)
        {
            PostDecimalPart = GetInt32(((Amount - PreDecimalPart) * 100).ToString("00"));
        }

        if (PreDecimalPart > 0)
        {
            strWords += PrefixBeforeDecimal + GetWordsForNumber(PreDecimalPart);
        }

        if (PostDecimalPart > 0)
        {
            if (strWords.Length > 0) strWords += Space(1);
            strWords += PrefixAfterDecimal + GetWordsForNumber(PostDecimalPart);
        }

        return strWords;
    }

    public static string GetMultiplierString(int Multiplier)
    {
        if (Multiplier == 1) return "Dr";
        else return "Cr";
    }

    public Mps GetMultiplierByInteger(int MInteger)
    {
        if (MInteger >= 0) return Mps.Dr;
        else return Mps.Cr;
    }

    public static string GetBalanceString(decimal value)
    {
        if (value > 0) return Math.Abs(value).ToString("0.00") + Space(1) + GetMultiplierString(Mps.Dr);
        else if (value < 0) return Math.Abs(value).ToString("0.00") + Space(1) + GetMultiplierString(Mps.Cr);
        else return string.Empty;
    }

    public static string GetMultiplierString(Mps multiplier)
    {
        if (multiplier == Mps.Dr) return "Dr";
        else return "Cr";
    }

    public static Mps GetMultiplierForAmount(decimal amount)
    {
        if (amount >= 0) return Mps.Dr;
        else return Mps.Cr;
    }

    public static string GetStringForAmount(decimal amount)
    {
        if (amount >= 0) return GetIndianNumber(Math.Abs(amount), 2) + Space(1) + GetMultiplierString(Mps.Dr);
        else return GetIndianNumber(Math.Abs(amount), 2) + Space(1) + GetMultiplierString(Mps.Cr);
    }

    public static List<string> SplitStringIntoLinesWithWordBoundaries(string strToSplit, int charsPerLine)
    {
        List<string> strParts = strToSplit.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(e => e.Trim().Length > 0).ToList();

        List<string> result = new List<string>();
        string currentLine = string.Empty;

        for (int i = 0; i < strParts.Count; i++)
        {
            string projectedCurrentLine = (currentLine + " " + strParts[i]).Trim();

            if (projectedCurrentLine.Length > charsPerLine)
            {
                result.Add(currentLine);
                currentLine = strParts[i];
            }
            else
            {
                currentLine = projectedCurrentLine;
            }
        }

        if (currentLine.Length > 0) result.Add(currentLine);

        return result;
    }

    public static List<string> SplitString(string strToSplit, int charsPerLine)
    {
        List<string> result = new List<string>();

        var strArray = strToSplit.Split(' ');

        string strLine = string.Empty;

        foreach (var strSplit in strArray)
        {
            if (strSplit.Trim().Length > 0)
            {
                if (strLine.Length + 1 + strSplit.Length > charsPerLine)
                {
                    result.Add(strLine);
                    strLine = string.Empty;
                }

                if (strLine.Length == 0)
                {
                    strLine = strSplit;
                }
                else
                {
                    strLine += Space(1) + strSplit;
                }
            }
        }

        if (strLine.Length > 0) result.Add(strLine);

        return result;
    }

    public static bool IsValidForSplit(string strToSplit, int charsPerLine)
    {
        foreach (var strSplit in SplitString(strToSplit, charsPerLine))
        {
            if (strSplit.Length > charsPerLine) return false;
        }

        return true;
    }

    public static string CombineListIntoSingleString(List<string> lst, string combinator)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string str in lst)
        {
            if (sb.Length > 0) sb.Append(combinator);
            sb.Append(str);
        }

        return sb.ToString();
    }

    public static string CombineListIntoSingleStringForSqlQuery(List<string> lst)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string str in lst)
        {
            string strEncoded = str.Replace("'", "''");
            strEncoded = $"'{strEncoded}'";
            if (sb.Length > 0) sb.Append(",");
            sb.Append(strEncoded);
        }

        return sb.ToString();
    }

    public static string CombineListIntoSingleString(List<string> lst, char combinator)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string str in lst)
        {
            if (sb.Length > 0) sb.Append(combinator);
            sb.Append(str);
        }

        return sb.ToString();
    }

    public static string FormulateRepeatedString(string element, int count)
    {
        StringBuilder sb = new StringBuilder(element.Length * count);
        for (int i = 0; i < count; i++) sb.Append(element);

        return sb.ToString();
    }

    public static List<string> FormulateStringList(DataTable dtb, string columnName,
                                                    string filterString, string sortPropertyName)
    {
        List<string> result = new List<string>();

        foreach (DataRow dr in dtb.Select(filterString, sortPropertyName))
        {
            result.Add(GetString(dr[columnName]));
        }

        return result;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static bool IsValidIPv4(string ipString)
    {
        if (string.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

    public static bool IsValidPort(int port)
    {
        return (port >= 0 && port <= 65535);
    }

    public static bool IsValidGSTNumber(string gstNumber)
    {
        if (string.IsNullOrEmpty(gstNumber))
            return false;

        // Remove white spaces and convert to uppercase
        gstNumber = gstNumber.Trim().ToUpper();

        // GSTIN format regex pattern
        string pattern = @"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[1-9A-Z]{1}[Z]{1}[A-Z\d]{1}$";

        return Regex.IsMatch(gstNumber, pattern);
    }

    public static List<DateTimeRange> SplitDateTimeRangeIntoBuckets(string fromDateTime, string toDateTime,
        DateTimeRangeBucketTypes bucketType)
    {
        List<DateTimeRange> result = new List<DateTimeRange>();

        DateTime dtStart = DTU.FromString(fromDateTime);
        DateTime dtEnd = DTU.FromString(toDateTime);

        DateTime dtIterator = dtStart;

        while (true)
        {
            DateTime dtBucketStart = dtIterator;
            DateTime dtBucketEnd = dtBucketStart;

            switch (bucketType)
            {
                case DateTimeRangeBucketTypes.Seconds:
                    dtBucketEnd = dtIterator.AddSeconds(1).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Minutes:
                    dtBucketEnd = dtIterator.AddMinutes(1).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.FifteenMinutes:
                    dtBucketEnd = dtIterator.AddMinutes(15).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.HalfHours:
                    dtBucketEnd = dtIterator.AddMinutes(30).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Hours:
                    dtBucketEnd = dtIterator.AddHours(1).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.ThreeHours:
                    dtBucketEnd = dtIterator.AddHours(3).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.SixHours:
                    dtBucketEnd = dtIterator.AddHours(6).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.EightHours:
                    dtBucketEnd = dtIterator.AddHours(8).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.TwelveHours:
                    dtBucketEnd = dtIterator.AddHours(12).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Days:
                    dtBucketEnd = dtIterator.AddDays(1).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Weeks:
                    dtBucketEnd = dtIterator.AddDays(7).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Fortnights:
                    dtBucketEnd = dtIterator.AddDays(15).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Months:
                    dtBucketEnd = dtIterator.AddMonths(1).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Quarters:
                    dtBucketEnd = dtIterator.AddMonths(3).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Trimesters:
                    dtBucketEnd = dtIterator.AddMonths(4).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Semesters:
                    dtBucketEnd = dtIterator.AddMonths(6).AddMilliseconds(-1); break;
                case DateTimeRangeBucketTypes.Years:
                    dtBucketEnd = dtIterator.AddYears(1).AddMilliseconds(-1); break;
            }

            string strDTBucketStart = DTU.ConvertToString(dtBucketStart);
            string strDTBucketEnd = DTU.ConvertToString(dtBucketEnd);

            DateTimeRange dtrBucket = new DateTimeRange(strDTBucketStart, strDTBucketEnd);

            result.Add(dtrBucket);

            dtIterator = dtBucketEnd.AddMilliseconds(1);

            if (dtIterator >= dtEnd) break;
        }

        return result;
    }

    public static byte[] ImageFileToByteArray(string imageFilePath)
    {
        if (!File.Exists(imageFilePath)) return new byte[0];
        return File.ReadAllBytes(imageFilePath);
    }

    public static JObject GenerateDifferentialObject(JObject oldObject, JObject newObject,
        Dictionary<string, Tuple<PropertyTypes, string>> propertyTypeMap = null)
    {
        JObject result = new JObject();

        if (propertyTypeMap == null)
        {
            foreach (var p in newObject.Properties())
            {
                if (!oldObject.ContainsKey(p.Name) || !oldObject[p.Name].Equals(newObject[p.Name]))
                {
                    result[p.Name] = newObject[p.Name];
                }
            }
        }
        else
        {
            foreach (var p in newObject.Properties())
            {
                if (!propertyTypeMap.ContainsKey(p.Name))
                {
                    if (!oldObject.ContainsKey(p.Name) || !oldObject[p.Name].Equals(newObject[p.Name]))
                    {
                        result[p.Name] = newObject[p.Name];
                    }
                }
                else
                {
                    PropertyTypes pType = propertyTypeMap[p.Name].Item1;
                    string pTypeSpecific = propertyTypeMap[p.Name].Item2;

                    switch (pType)
                    {
                        case PropertyTypes.String:
                            if (GetString(oldObject[p.Name]) != GetString(newObject[p.Name]))
                                result[p.Name] = newObject[p.Name];
                            break;

                        case PropertyTypes.Decimal:
                            if (GetDecimal(oldObject[p.Name], 10) != GetDecimal(newObject[p.Name], 10))
                                result[p.Name] = newObject[p.Name];
                            break;

                        case PropertyTypes.Integer:
                            if (GetInt32(oldObject[p.Name]) != GetInt32(newObject[p.Name]))
                                result[p.Name] = newObject[p.Name];
                            break;

                        case PropertyTypes.Long:
                            if (GetInt64(oldObject[p.Name]) != GetInt64(newObject[p.Name]))
                                result[p.Name] = newObject[p.Name];
                            break;

                        case PropertyTypes.Boolean:
                            if (GetBoolean(oldObject[p.Name]) != GetBoolean(newObject[p.Name]))
                                result[p.Name] = newObject[p.Name];
                            break;

                        case PropertyTypes.ByteArray:
                            if (!oldObject[p.Name].Equals(newObject[p.Name]))
                                result[p.Name] = newObject[p.Name];
                            break;

                        case PropertyTypes.Collection:
                            JObject joOldCollection = JObject.FromObject((JArray)oldObject[p.Name]);
                            JObject joNewCollection = JObject.FromObject((JArray)newObject[p.Name]);

                            JObject joDiffCollection = GenerateDifferentialObject(joOldCollection, joNewCollection);
                            if (joDiffCollection.Properties().Count() > 0)
                            {
                                result[p.Name] = newObject[p.Name];
                            }
                            break;

                        case PropertyTypes.Complex:
                            JObject joOldComplex = JObject.FromObject(oldObject[p.Name]);
                            JObject joNewComplex = JObject.FromObject(newObject[p.Name]);

                            Dictionary<string, Tuple<PropertyTypes, string>> pTypeMapProp = null;
                            if (pTypeSpecific.Trim().Length > 0)
                            {
                                pTypeMapProp = EntityTypes.GetPropertyTypeMap(pTypeSpecific);
                            }

                            JObject joDiffComplex = GenerateDifferentialObject(joOldComplex, joNewComplex, pTypeMapProp);

                            if (joDiffComplex.Properties().Count() > 0)
                            {
                                result[p.Name] = newObject[p.Name];
                            }
                            break;
                    }
                }
            }
        }

        return result;
    }

    public static List<T> FormulateEntitiesFromCollection<T>(DataContainer dc, string collectionName, bool allowEdit,
        Func<JObject, DataContainer, bool, T> generatorFunction)
    {
        List<T> result = new List<T>();

        var coll = dc.GetOrCreateCollection(collectionName);

        coll.Entries.ForEach(e =>
        {
            var entity = generatorFunction(e, dc, allowEdit);
            result.Add(entity);
        });

        return result;
    }

    public static List<T> FormulateEntitiesFromCollection<T>(DataContainer dc, string collectionName, bool allowEdit,
        bool allowInPlaceEditing, Func<JObject, DataContainer, bool, bool, T> generatorFunction)
    {
        List<T> result = new List<T>();

        var coll = dc.GetOrCreateCollection(collectionName);

        coll.Entries.ForEach(e =>
        {
            var entity = generatorFunction(e, dc, allowEdit, allowInPlaceEditing);
            result.Add(entity);
        });

        return result;
    }





    private const string GSTIN_REGEX = "[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9A-Za-z]{1}[Z]{1}[0-9a-zA-Z]{1}";
    private const string CHECKSUM_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static bool IsValidGSTIN(string GSTIN)
    {
        GSTIN = GSTIN.Trim();

        if (GSTIN.Length == 0) return false;

        bool isValidFormat = false;

        if (string.IsNullOrEmpty(GSTIN))
            isValidFormat = false;
        else if (Regex.IsMatch(GSTIN, GSTIN_REGEX))
            isValidFormat = GSTIN[^1].Equals(GenerateCheckSum(GSTIN[..^1]));
        else
            isValidFormat = false;

        return isValidFormat;
    }

    private static char GenerateCheckSum(string GSTIN)
    {
        int factor = 2;
        int sum = 0;
        int checkCodePoint = 0;
        char[] cpChars;
        char[] inputChars;

        if (string.IsNullOrEmpty(GSTIN)) throw new Exception("GSTIN supplied for checkdigit calculation is null");

        cpChars = CHECKSUM_CHARS.ToCharArray();
        inputChars = GSTIN.ToUpper().ToCharArray();

        int Mod_ = cpChars.Length;
        for (int i = inputChars.Length - 1; i >= 0; i += -1)
        {
            int codePoint = -1;
            for (int j = 0; j <= cpChars.Length - 1; j++)
            {
                if (cpChars[j] == inputChars[i])
                {
                    codePoint = j;
                    break;
                }
            }
            int digit = factor * codePoint;
            factor = factor == 2 ? 1 : 2;
            digit = (digit / Mod_) + (digit % Mod_);
            sum += digit;
        }
        checkCodePoint = (Mod_ - (sum % Mod_)) % Mod_;
        return cpChars[checkCodePoint];
    }

}
