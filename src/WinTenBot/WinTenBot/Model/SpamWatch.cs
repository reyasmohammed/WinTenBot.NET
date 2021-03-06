﻿using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WinTenBot.Model
{
    public partial class SpamWatch
    {
        [JsonProperty("admin")]
        public long Admin { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
        
        public bool IsBan { get; set; }
    }

    public partial class SpamWatch
    {
        public static SpamWatch FromJson(string json) => 
            JsonConvert.DeserializeObject<SpamWatch>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this SpamWatch self) => 
            JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}