using Newtonsoft.Json;

public struct AllocateProcessTokensRequest
{
    public long Count;

    public static AllocateProcessTokensRequest Deserialize(string input)
    {
        var result = JsonConvert.DeserializeObject<AllocateProcessTokensRequest>(input);
        if (result.Count <= 0) result.Count = 1;

        return result;
    }
}
