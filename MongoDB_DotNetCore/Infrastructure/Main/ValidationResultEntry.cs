using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

public class ValidationResultEntry
{
    public ValidationResultEntry() {}

    public string ValidationMessage { get; set; } = string.Empty;
    public string ValidationTarget { get; set; } = string.Empty;
}