using System;
using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class RssSetting
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
        
        [JsonPropertyName("from_id")]
        public string FromId { get; set; }
        
        [JsonPropertyName("url_feed")]
        public string UrlFeed { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}