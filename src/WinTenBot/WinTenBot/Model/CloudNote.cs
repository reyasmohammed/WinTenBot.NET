using System;
using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class CloudNote
    {
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
        
        [JsonPropertyName("user_id")]
        public string FromId { get; set; }
        
        [JsonPropertyName("slug")]
        public string Tag { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("btn_data")]
        public string BtnData { get; set; }
        
        [JsonPropertyName("author")]
        public string TypeData { get; set; }
        
        [JsonPropertyName("id_data")]
        public string IdData { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}