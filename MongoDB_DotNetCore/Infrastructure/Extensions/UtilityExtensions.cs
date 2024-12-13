using System.Collections.Generic;
using System.Linq;

namespace VJCore.Infrastructure.Extensions;

public static class UtilityExtensions
{
    //public static void AutoNumberPages(this List<PrintPage> lstPages, string pageNoPlaceHolderString)
    //{
    //    int pageNo = 0;
    //    int totalPageCount = lstPages.Count;

    //    foreach (PrintPage pp in lstPages)
    //    {
    //        pageNo++;

    //        string pageNoString = $"{pageNo} of {totalPageCount}";

    //        foreach (var spp in pp.GetAllStringPrintingParameters().Where(sppCheck => sppCheck.PrintString.Contains(pageNoPlaceHolderString)))
    //        {
    //            spp.PrintString = spp.PrintString.Replace(pageNoPlaceHolderString, pageNoString);
    //        }
    //    }
    //}

    public static List<string> SplitIntoChunksByLength(this string str, int chunkLength)
    {
        if (string.IsNullOrEmpty(str) || chunkLength < 1) return new List<string>();

        var result = new List<string>();

        for (int i = 0; i < str.Length; i += chunkLength)
            if (str.Length - i >= chunkLength) result.Add(str.Substring(i, chunkLength));
            else result.Add(str[i..]);

        return result;
    }
}
