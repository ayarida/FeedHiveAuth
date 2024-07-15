namespace FeedHiveAuth.Models.ViewModels
{
    public class SocialConfigsViewModel
    {
        public string? PublicUrl { get; set; }
        public string? LocalUrl { get; set; }
        public string? DefaultImageUrl { get; set; }
        public string TechnicalConfigsId { get; set; }

        /*Facebook Configs*/
        public string FacebookConfigsId {  get; set; }
        public string DisplayName { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public bool EnableFb { get; set; }

        /*Twitter Configs*/
        public string TwitterConfigsId { get; set; }
        public string ScreenName { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string TokenX { get; set; }
        public string TokenSecret { get; set; }
        public bool EnableX { get; set; }


        /*Telegram Configs*/
        public string TelegramConfigsId {  get; set; }
        public string UsernameTelegram { get; set; }
        public string TokenTelegram { get; set; }
        public bool EnableTelegram { get; set; }

        /*Dailymotion Configs*/
        public string DailymotionConfigsId {  get; set; }
        public string ChannelName { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
        public string UsernameDM { get; set; }
        public string Password { get; set; }
        public string CallBackUrl { get; set; }
        public string LocalPath { get; set; }
        public bool EnableDM { get; set; }

        /*Whatsapp Configs*/
        public string WhatsappConfigsId {  get; set; }
        public string bbs { get; set; }
        public string Nickname { get; set; }
        public string EnableWhatsapp {  get; set; }
    }
}
