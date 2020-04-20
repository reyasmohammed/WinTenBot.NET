using System;
using Newtonsoft.Json;
using WinTenBot.Enums;

namespace WinTenBot.Model
{
    public class CloudTag
    {
        [JsonProperty("id_chat")]
        public string ChatId { get; set; }
        
        [JsonProperty("id_user")]
        public string FromId { get; set; }
        
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        
        [JsonProperty("btn_data")]
        public string BtnData { get; set; }
        
        [JsonProperty("type_data")]
        public MediaType TypeData { get; set; }
        
        [JsonProperty("id_data")]
        public string IdData { get; set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}