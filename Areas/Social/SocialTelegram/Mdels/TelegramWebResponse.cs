namespace FeedHiveAuth.Areas.Social.SocialTelegram.Mdels
{
    public class TelegramWebResponse<T>
    {
        public bool Ok { get; set; }
        public T Result { get; set; }
        public int ErrorCode { get; set; }
        public string Description { get; set; }
    }
}
