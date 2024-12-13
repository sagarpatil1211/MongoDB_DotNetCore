using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;

public class HardwareIDGenerator
{
    public static string GenerateHardwareID(string basicHardwareID)
    {
        var enc = new UnicodeEncoding();
        var provSHA1 = SHA1.Create();

        var sha1ByteSource = enc.GetBytes(basicHardwareID);
        var sha1ByteHash = provSHA1.ComputeHash(sha1ByteSource);

        var hardwareIDConverted = Convert.ToBase64String(sha1ByteHash);

        var provMD5 = MD5.Create();

        var byteSource = enc.GetBytes(hardwareIDConverted);
        var byteHash = provMD5.ComputeHash(byteSource);

        hardwareIDConverted = GenerateDelimitedStringFromBasicString(NormalizeMD5EncryptedString(Convert.ToBase64String(byteHash)), 6, "-", false);

        return GenerateDelimitedStringFromBasicString(hardwareIDConverted.Replace("-", string.Empty), 6, "-", true);
    }

    private static string NormalizeMD5EncryptedString(string encryptedString)
    {
        if (!encryptedString.EndsWith("==")) return encryptedString;

        encryptedString = encryptedString.Remove(encryptedString.Length - 2);
        encryptedString += encryptedString[3].ToString();
        encryptedString += encryptedString[17].ToString();

        return encryptedString;
    }

    public static string GenerateDelimitedStringFromBasicString(string basicString, int charCountPerString,
                                                            string delimiter, bool reverseStringBeforeSplit)
    {
        var enc = new UnicodeEncoding();
        var provMD5 = MD5.Create();

        var byteSource = enc.GetBytes(basicString);
        var byteHash = provMD5.ComputeHash(byteSource);

        var keyBaseString = NormalizeMD5EncryptedString(Convert.ToBase64String(byteHash));

        var lstStrings = SplitIntoStrings(keyBaseString, charCountPerString, reverseStringBeforeSplit);

        var sb = new StringBuilder();

        foreach (string tempString in lstStrings)
        {
            if (sb.Length > 0) sb.Append(delimiter);
            var keyCharArray = tempString.ToCharArray();

            foreach (char c in keyCharArray)
            {
                sb.Append(TransformCharacterToEasilyReadable(c.ToString()));
            }
        }

        return sb.ToString();
    }

    private static List<string> SplitIntoStrings(string basicString, int charCountPerString, bool splitAfterReversing)
    {
        IEnumerable<char> keyChars = null;

        if (splitAfterReversing)
        {
            keyChars = basicString.Reverse();
        }
        else
        {
            keyChars = basicString.ToCharArray();
        }

        var lst = new List<string>();

        int charCount = 0;
        string tempString = string.Empty;

        foreach (char c in keyChars)
        {
            charCount++;
            tempString += c.ToString();

            if (charCount == charCountPerString)
            {
                lst.Add(tempString);
                tempString = string.Empty;
                charCount = 0;
            }
        }

        if (charCount > 0 && charCount < charCountPerString) lst.Add(tempString);

        return lst;
    }

    private static string TransformCharacterToEasilyReadable(string c)
    {
        byte asciiValue = Encoding.Default.GetBytes(c)[0];
        if (asciiValue >= 49 && asciiValue <= 57) return c;
        if (asciiValue >= 65 && asciiValue <= 90) return c;
        if (asciiValue >= 96 && asciiValue <= 116) return c.ToUpper();

        if (asciiValue < 49) return "1";
        if (asciiValue > 57 && asciiValue < 65) return "4";
        if (asciiValue > 90 && asciiValue < 96) return "S";
        if (asciiValue > 116) return "K";

        return string.Empty;
    }
}
