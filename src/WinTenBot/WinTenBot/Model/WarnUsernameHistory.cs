using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WinTenBot.Model
{
    class WarnUsernameHistory
    {
        [JsonPropertyName("from_id")]
        public int FromId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("step_count")]
        public long StepCount { get; set; }

        [JsonPropertyName("chat_id")]
        public long ChatId { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }
}
