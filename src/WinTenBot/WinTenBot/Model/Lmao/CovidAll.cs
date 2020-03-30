using Newtonsoft.Json;

namespace WinTenBot.Model.Lmao
{
    public partial class CovidAll
    {
        [JsonProperty("cases")]
        public long Cases { get; set; }

        [JsonProperty("deaths")]
        public long Deaths { get; set; }

        [JsonProperty("recovered")]
        public long Recovered { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("active")]
        public long Active { get; set; }
    }

    public partial class CovidAll
    {
        public static CovidAll FromJson(string json) => JsonConvert.DeserializeObject<CovidAll>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CovidAll self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    // internal static class Converter
    // {
    //     public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //     {
    //         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //         DateParseHandling = DateParseHandling.None,
    //         Converters =
    //         {
    //             new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //         },
    //     };
    // }
}