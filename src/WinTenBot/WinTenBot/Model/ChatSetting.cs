using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    public class ChatSetting
    {
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
        
        [JsonPropertyName("member_count")]
        public long MemberCount { get; set; }
        
        [JsonPropertyName("welcome_message")]
        public string WelcomeMessage { get; set; }
        
        [JsonPropertyName("welcome_button")]
        public string WelcomeButton { get; set; }
        
        [JsonPropertyName("welcome_media")]
        public string WelcomeMedia { get; set; }
        
        [JsonPropertyName("welcome_media_type")]
        public string WelcomeMediaType { get; set; }
        
        [JsonPropertyName("rules_text")]
        public string RulesText { get; set; }
        
        [JsonPropertyName("last_tags_message_id")]
        public string LastTagsMessageId { get; set; }
    }
}