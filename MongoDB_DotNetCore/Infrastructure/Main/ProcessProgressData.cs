using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ProcessProgressData
{
    public string ProcessToken { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; } = 0.0M;
    public string ProgressDescriptionString { get; set; } = string.Empty;
    public string ProgressDescriptionHTML { get; set; } = string.Empty;
    public bool ProcessCompleted { get; set; } = false;
    public TransactionResult ProcessResult { get; set; } = null;

    private static ProcessProgressData FromJsonObject(JObject data)
    {
        return JsonConvert.DeserializeObject<ProcessProgressData>(JsonConvert.SerializeObject(data));
    }
}
