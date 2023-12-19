using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class SocialConfigs
    {
        public string PersonalShareNetworks { get; set; }
        public bool EnableShareOnAllNetworks { get; set; }
        public FacebookConfigs FacebookConfigs { get; set; }

        public TelegramConfigs TelegramConfigs { get; set; }

        public static SocialConfigs Construct(string subscriptionId=null)
        {
            return new SocialConfigs
            {
                PersonalShareNetworks = string.Join(',', EnumExtension.GetAllList<SocialNetworkTypeEnum>()),
                FacebookConfigs = new FacebookConfigs
                {
                    Application = new FacebookApp
                    {
                        DisplayName = "OCPublisher",
                        AppId = "1629657670987973",
                        AppSecret = "7e2fd21dedf5a4249c4fc638ee82fcb6"
                    }
                },
                TelegramConfigs = new TelegramConfigs
                {
                    Bot = new TelegramBot
                    {
                        Username = "octipulse_bot",
                        Token = "847874047:AAHOYz5UdHChoss2heeXPIcycsLwB6tNty0"
                    }
                }
            };
        }
    }
    public class FacebookConfigs
    {
        public FacebookApp Application { get; set; }
    }
    public class FacebookApp
    {
        public string DisplayName { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public bool Enable { get; set; }
        public bool Enabled => Enable && AppId.IsNotNullOrEmpty() && AppSecret.IsNotNullOrEmpty();
    }

    public class TelegramConfigs
    {
        public TelegramBot Bot { get; set; }
    }
    public class TelegramBot
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public bool Enable { get; set; } = true;
        public bool Enabled => Enable && Token.IsNotNullOrEmpty();
    }
}
