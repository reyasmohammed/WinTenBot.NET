using System;
using Newtonsoft.Json;

namespace WinTenBot.Model
{
    public class CatMeow
    {
        [JsonProperty("file")]
        public Uri File { get; set; }
    }
}