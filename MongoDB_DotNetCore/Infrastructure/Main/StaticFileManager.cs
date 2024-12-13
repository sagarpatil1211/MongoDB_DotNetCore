//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;

//public class StaticFileManager
//{
//    public static FileStreamResult GetStaticFile(string fileIdentifier)
//    {
//        string path = Path.Combine(Directory.GetCurrentDirectory(), @"Resources", @"UploadedDocuments", fileIdentifier);

//        if (File.Exists(path))
//        {
//            FileStream fs = new FileStream(path, FileMode.Open);
//            fs.Position = 0;

//            string contentType = MimeMapping.MimeUtility.GetMimeMapping(path);

//            FileStreamResult fileStreamResult = new FileStreamResult(fs, contentType)
//            {
//                FileDownloadName = fileIdentifier
//            };

//            return fileStreamResult;
//        }
//        else
//        {
//            return null;
//        }
//    }
//}
