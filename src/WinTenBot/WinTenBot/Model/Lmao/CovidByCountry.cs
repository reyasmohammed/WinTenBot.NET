using System;
using Newtonsoft.Json;

namespace WinTenBot.Model.Lmao
{
    public partial class CovidByCountry
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countryInfo")]
        public CountryInfo CountryInfo { get; set; }

        [JsonProperty("cases")]
        public long Cases { get; set; }

        [JsonProperty("todayCases")]
        public long TodayCases { get; set; }

        [JsonProperty("deaths")]
        public long Deaths { get; set; }

        [JsonProperty("todayDeaths")]
        public long TodayDeaths { get; set; }

        [JsonProperty("recovered")]
        public long Recovered { get; set; }

        [JsonProperty("active")]
        public long Active { get; set; }

        [JsonProperty("critical")]
        public long Critical { get; set; }

        [JsonProperty("casesPerOneMillion")]
        public long CasesPerOneMillion { get; set; }

        [JsonProperty("deathsPerOneMillion")]
        public double DeathsPerOneMillion { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }
    }

    public partial class CountryInfo
    {
        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("iso2")]
        public string Iso2 { get; set; }

        [JsonProperty("iso3")]
        public string Iso3 { get; set; }

        [JsonProperty("lat")]
        public long Lat { get; set; }

        [JsonProperty("long")]
        public long Long { get; set; }

        [JsonProperty("flag")]
        public Uri Flag { get; set; }
    }

    public partial class CovidByCountry
    {
        public static CovidByCountry FromJson(string json) => JsonConvert.DeserializeObject<CovidByCountry>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this CovidByCountry self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}