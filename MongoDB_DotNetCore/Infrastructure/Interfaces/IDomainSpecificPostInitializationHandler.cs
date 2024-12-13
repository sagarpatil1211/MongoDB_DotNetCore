using System;
using System.Collections.Generic;
using System.Text;

public interface IDomainSpecificPostInitializationHandler
{
    void InvokeProcesses();
    void PostCacheDataToMessageHub();
}
