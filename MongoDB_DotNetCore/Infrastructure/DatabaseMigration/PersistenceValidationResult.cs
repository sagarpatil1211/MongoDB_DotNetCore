using System.Collections.Generic;
using Microsoft.VisualBasic;
//using VJ1Core.Infrastructure.DatabaseMigration.Interfaces;

//name1space VJCore.Infrastructure.DatabaseMigration;

public class PersistenceValidationResult<T> : IDBCADataPersistenceResult<T>
{
    public PersistenceValidationResult(List<string> exList)
    {
        Successful = false;
        if (exList.Count == 1)
        {
            ExceptionMessage = exList[0];
        }
        else
        {
            string stringFormat = Strings.StrDup(exList.Count.ToString().Length, "0");
            int iEx = 0;
            foreach (string exMessage in exList)
            {
                iEx += 1;
                if (iEx > 1)
                    ExceptionMessage += ControlChars.NewLine;
                ExceptionMessage += iEx.ToString(stringFormat) + ". " + exMessage;
            }
        }
    }

    public string ExceptionMessage { get; set; }
    public bool Successful { get; set; }
    public T Tag { get; set; }
}
