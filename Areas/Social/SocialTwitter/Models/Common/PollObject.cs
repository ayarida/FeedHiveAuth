namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common
{
    public class PollObject
    {
        public IEnumerable<Option> Options { get; set; }
        public string EndDatetime { get; set; }
        public string DurationMinutes { get; set; }
    }

    public class Option
    {
        public int Position { get; set; }
        public string Text { get; set; }
    }
}
