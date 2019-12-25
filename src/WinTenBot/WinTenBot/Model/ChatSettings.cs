namespace WinTenBot.Model
{
    public class ChatSettings
    {
        public long ChatId { get; set; }
        public long MemberCount { get; set; }
        public string WelcomeMessage { get; set; }
        public string WelcomeButton { get; set; }
        public string WelcomeMedia { get; set; }
        public string WelcomeMediaType { get; set; }
    }
}