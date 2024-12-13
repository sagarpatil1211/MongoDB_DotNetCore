using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public delegate Func<TransportData, Func<JObject, bool>, Func<JObject, string>, List<BaseObject>> 
    DomainEntityListFormulator(TransportData td, Func<JObject, bool> selector, Func<JObject, string> sortFunction);
