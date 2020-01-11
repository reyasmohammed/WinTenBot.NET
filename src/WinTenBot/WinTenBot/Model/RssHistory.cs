using System;

namespace WinTenBot.Model
{
    public class RssHistory
    {
        public int Id { get; set; }
        public string ChatId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}