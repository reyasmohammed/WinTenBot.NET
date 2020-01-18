using System;
using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class CloudTag
    {
        [JsonPropertyName("id_chat")]
        public string ChatId { get; set; }
        
        [JsonPropertyName("id_user")]
        public string FromId { get; set; }
        
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("btn_data")]
        public string BtnData { get; set; }
        
        [JsonPropertyName("type_data")]
        public string TypeData { get; set; }
        
        [JsonPropertyName("id_data")]
        public string IdData { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}