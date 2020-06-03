using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class GlobalBanData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("reason_ban")]
        public string ReasonBan { get; set; }

        [JsonPropertyName("banned_by")]
        public int BannedBy { get; set; }

        [JsonPropertyName("banned_from")]
        public long BannedFrom { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

    }

}
