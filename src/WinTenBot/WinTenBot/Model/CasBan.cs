using Newtonsoft.Json;

namespace WinTenBot.Model
{
    public class CasBan
    {
        [JsonProperty("ok")] public bool Ok { get; set; }

        [JsonProperty("result")] public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("offenses")] public long Offenses { get; set; }

        [JsonProperty("messages")] public string[] Messages { get; set; }

        [JsonProperty("time_added")] public long TimeAdded { get; set; }
    }
}