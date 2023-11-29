using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.SocialTelegram.Mdels
{
    public class TelegramGroupMedia
    {
        public MediaTypeEnum MediaType { get; internal set; }
        public Stream Stream { get; internal set; }
        public string FileName { get; internal set; }
        public string Text { get; internal set; }
        public string Thumbnail { get; internal set; }
    }
}
