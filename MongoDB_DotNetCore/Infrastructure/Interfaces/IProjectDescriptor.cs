using System;
using System.Collections.Generic;
using System.Text;

public interface IProjectDescriptor
{
    string ProjectName { get; }
    bool CombinedCacheDataBroadcast { get; }
}
