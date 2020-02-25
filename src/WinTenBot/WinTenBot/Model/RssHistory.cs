using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class RssHistory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
        
        [JsonPropertyName("rss_source")]
        public string RssSource { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("publish_date")]
        public string PublishDate { get; set; }
        
        [JsonPropertyName("author")]
        public string Author { get; set; }
        
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}