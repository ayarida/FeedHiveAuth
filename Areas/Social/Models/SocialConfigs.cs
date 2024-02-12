using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class SocialConfigs
    {
        public TechnicalConfigs TechnicalConfigs { get; set; } 
        public string PersonalShareNetworks { get; set; }
        public bool EnableShareOnAllNetworks { get; set; }
        public FacebookConfigs FacebookConfigs { get; set; }
        public FacebookConfigs InstagramConfigs { get; set; }

        public TelegramConfigs TelegramConfigs { get; set; }

        public WhatsappConfigs WhatsappConfigs { get; set; }
        public TwitterConfigs TwitterConfigs { get; set; }

        public DailymotionConfigs DailymotionConfigs { get; set; }

        public static SocialConfigs Construct(string subscriptionId = null)
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
                },
                WhatsappConfigs = new WhatsappConfigs
                {
                    Application = new WhatsappApp()

                },
                TwitterConfigs = new TwitterConfigs
                {
                    Application = new TwitterApp()
                },
                InstagramConfigs = new FacebookConfigs { Application = new FacebookApp() },
                DailymotionConfigs = new DailymotionConfigs
                {
                    Application = new DailymotionApp
                    {
                        APIKey = "b886992e2d793e4d9ebe",
                        APISecret = "ec51475bcbb2235b9bfb465677699793817db1d2",
                        ChannelName = "dmoctipulsetesting",
                        Username = "rouba.87@hotmail.com",
                        Password = "0Ctip@lse",
                        CallBackUrl = "https://www.dailymotion.com/lb",
                        LocalPath = "C:/media/",
                        Enable = true
                    }
                }, 
                TechnicalConfigs = new TechnicalConfigs
                {
                    PublicUrl = "https://socialpublisher.net/",
                    LocalUrl = "https://localhost:7157/"

                }
            };
        }
    }

    public class TechnicalConfigs
    {
        public string? PublicUrl { get; set; }
        public string? LocalUrl { get; set; }
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

    public class WhatsappConfigs
    {
        public WhatsappApp Application { get; set; }
    }

    public class WhatsappApp
    {
        public string bbs { get; set; }
        public string Nickname { get; set; }
        public bool Enable { get; set; }
        public bool Enabled => Enable && bbs.IsNotNullOrEmpty() && Nickname.IsNotNullOrEmpty();
    }

    public class TwitterConfigs
    {
        public TwitterApp Application { get; set; }
        public int ChunkSizeMB { get; set; }
        public TrendingTopics TrendingTopics { get; set; }
    }

    public class TwitterApp
    {
        public string ScreenName { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public bool Enable { get; set; }
        public bool Enabled => Enable && ConsumerKey.IsNotNullOrEmpty() && ConsumerSecret.IsNotNullOrEmpty();
    }

    public class TrendingTopics
    {
        public bool Enable { get; set; }
        public ulong? PlaceId { get; set; }
        public string ExcludedTags { get; set; }
        public int TopicsNumber { get; set; }
        public string WoeidFile { get; set; }
    }

    public class DailymotionConfigs
    {
        public DailymotionApp Application { get; set; }
    }

    public class DailymotionApp
    {
        public string ChannelName { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CallBackUrl { get; set; }
        public string LocalPath { get; set; }
        public bool Enable { get; set; }
        public bool Enabled => Enable;
    }
}
