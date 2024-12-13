//using CCA.Util;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.IO;
////using VJ1Core.Infrastructure.Main;

//public class DocumentGenerationRequestManager
//{
//    public static string CodeCollectionName { get => "DocumentGenerationRequestCode"; }

//    private class Entry
//    {
//        public TransportData TransportData { get; } = null;
//        public string Code { get; } = string.Empty;
//        public long RequestArrivalDateTimeNumeric { get; } = 0L;

//        public Entry(TransportData transportData, string code, long requestArrivalDateTime)
//        {
//            TransportData = transportData;
//            Code = code;
//            RequestArrivalDateTimeNumeric = requestArrivalDateTime;
//        }
//    }

//    private static Dictionary<string, Entry> entries = new Dictionary<string, Entry>();

//    private static long m_documentGenerationRequestId = Utils.GetInt64($"{SessionController.CustomerLicenseNo}0000000000");

//    public static string AddDocumentGenerationRequestAndReturnCode(TransportData td)
//    {
//        long cdtValue = DTU.GetCurrentNumericDateTime();
//        CCACrypto ccaCrypto = new CCACrypto();

//        m_documentGenerationRequestId++;

//        //string strCode = ccaCrypto.Encrypt(td.ConvertToJsonObject(), cdtValue.ToString());
//        string strCode = ccaCrypto.Encrypt(m_documentGenerationRequestId.ToString(), cdtValue.ToString());

//        foreach (IFormFile f in td.GetFileCollection())
//        {
//            using (MemoryStream ms = new MemoryStream())
//            {
//                f.CopyTo(ms);
//                byte[] fileBytes = ms.ToArray();

//                td.SetFileBytes(f.Name, fileBytes);
//            }
//        }

//        var entry = new Entry(td, strCode, cdtValue);
//        entries.Add(strCode, entry);

//        return strCode;
//    }

//    public static TransactionResult<DocumentStreamContainer> GenerateDocument(string code)
//    {
//        TransactionResult<DocumentStreamContainer> result = new TransactionResult<DocumentStreamContainer>();

//        try
//        {
//            if (!entries.ContainsKey(code)) throw new DomainException("INVALID REQUEST");

//            Entry entry = entries[code];

//            TransportData td = entry.TransportData;

//            long cdtValue = DTU.GetCurrentNumericDateTime();

//            DateTime cDT = new DateTime(cdtValue);

//            List<string> keysToRemove = new List<string>();

//            foreach(var kvp in entries)
//            {
//                var entryInDictionary = kvp.Value;
//                DateTime reqDT = new DateTime(entryInDictionary.RequestArrivalDateTimeNumeric);
//                if (cDT.Subtract(reqDT).TotalHours > 24) keysToRemove.Add(kvp.Key);
//            }

//            //DateTime reqDT = new DateTime(entry.RequestArrivalDateTimeNumeric);

//            foreach (string key in keysToRemove) entries.Remove(key);
            
//            if (!entries.ContainsKey(code)) throw new DomainException("REQUEST HAS EXPIRED. PLEASE TRY AGAIN!");

//            DocumentStreamContainer str = SessionController.DIS.PerformDocumentGeneration(td);

//            result.Successful = true;
//            result.Data = str;

//            entries.Remove(code);
//        }
//        catch (Exception ex)
//        {
//            result.Successful = false;
//            result.Message = ex.Message;
//        }

//        return result;
//    }
//}
