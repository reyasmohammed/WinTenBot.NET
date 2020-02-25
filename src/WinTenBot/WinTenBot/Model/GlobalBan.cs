using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class GlobalBan
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("reason_ban")]
        public string ReasonBan { get; set; }

        [JsonPropertyName("banned_by")]
        public string BannedBy { get; set; }

        [JsonPropertyName("banned_from")]
        public string BannedFrom { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

    }

}
