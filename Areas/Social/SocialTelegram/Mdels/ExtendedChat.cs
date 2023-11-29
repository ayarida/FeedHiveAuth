using Telegram.Bot.Types;

namespace FeedHiveAuth.Areas.Social.SocialTelegram.Mdels
{
    public class ExtendedChat : Chat
    {
        public ChatPhoto Photo { get; set; }
        public string Description { get; set; }
    }
    public class ChatPhoto
    {
        public string SmallFileId { get; set; }
        public string BigFileId { get; set; }
    }
}
