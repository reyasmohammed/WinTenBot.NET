using System.Text.Json.Serialization;
using WinTenBot.Handlers.Commands.Rss;

namespace WinTenBot.Model
{
    public class WordFilter
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }
        
        [JsonPropertyName("deep_filter")]
        public bool DeepFilter { get; set; }
        public bool IsGlobal { get; set; }
        public string FromId { get; set; }
        public string ChatId { get; set; }
    }
}