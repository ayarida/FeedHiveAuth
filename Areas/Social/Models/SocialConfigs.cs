using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class SocialConfigs
    {
        public string PersonalShareNetworks { get; set; }
        public bool EnableShareOnAllNetworks { get; set; }
        public FacebookConfigs FacebookConfigs { get; set; }

        public static SocialConfigs Construct(string subscriptionId = "")
        {
            return new SocialConfigs
            {
                PersonalShareNetworks = string.Join(',', EnumExtension.GetAllList<SocialNetworkTypeEnum>()),
                FacebookConfigs = new FacebookConfigs
                {
                    Application = new FacebookApp
                    {
                        DisplayName = "FHiveAuth",
                        AppId = "315090498130849",
                        AppSecret = "d3fb385638ae42a8079be0d2e4266481"
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
}
