using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using System.Text.Json;

namespace FeedHiveAuth.Data.Helpers
{
    public static class SocialServiceHelper
    {
        public static ConfigsRepository _configsService = Instances.Repositories.ConfigsRepository;

        public static SocialConfigs getSocialConfigs()
        {
            var configs = _configsService.GetAllConfigs();
            var fbConf = configs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.Facebook.Value()).FirstOrDefault();
            var telegramConf = configs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.Telegram.Value()).FirstOrDefault();
            var wpConf = configs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.WhatsApp.Value()).FirstOrDefault();
            var dailymotionConf = configs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.DailyMotion.Value()).FirstOrDefault();
            var technicalConf = configs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.Technical.Value()).FirstOrDefault();
            var twitterConf = configs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.Twitter.Value()).FirstOrDefault();

            FacebookApp fbApp = JsonSerializer.Deserialize<FacebookApp>(fbConf.JsonValue) ?? new FacebookApp();
            TelegramBot telegramBot = JsonSerializer.Deserialize<TelegramBot>(telegramConf.JsonValue) ?? new TelegramBot();
            WhatsappApp wpApp = JsonSerializer.Deserialize<WhatsappApp>(wpConf.JsonValue) ?? new WhatsappApp();
            DailymotionApp dmApp = JsonSerializer.Deserialize<DailymotionApp>(dailymotionConf.JsonValue) ?? new DailymotionApp();
            AppTechnicalConfigs appTech = JsonSerializer.Deserialize<AppTechnicalConfigs>(technicalConf.JsonValue) ?? new AppTechnicalConfigs();
            TwitterApp twitterApp = JsonSerializer.Deserialize<TwitterApp>(twitterConf.JsonValue) ?? new TwitterApp();


            var socialConfigs = new SocialConfigs
            {
                TechnicalConfigs = new TechnicalConfigs
                {
                    Id = technicalConf.Id,
                    EnumKey = technicalConf.EnumKey,
                    appTechnicalConfigs = appTech
                },
                FacebookConfigs = new FacebookConfigs
                {
                    Id = fbConf.Id,
                    EnumKey = fbConf.EnumKey,
                    Application = fbApp

                },
                TelegramConfigs = new TelegramConfigs
                {
                    Id = telegramConf.Id,
                    EnumKey = telegramConf.EnumKey,
                    Bot = telegramBot
                },
                DailymotionConfigs = new DailymotionConfigs
                {
                    Id = dailymotionConf.Id,
                    EnumKey = dailymotionConf.EnumKey,
                    Application = dmApp
                },
                TwitterConfigs = new TwitterConfigs
                {
                    Id = twitterConf.Id,
                    EnumKey = twitterConf.EnumKey,
                    Application = twitterApp
                },
                WhatsappConfigs = new WhatsappConfigs
                {
                    Id = wpConf.Id,
                    EnumKey = wpConf.EnumKey,
                    Application = wpApp
                }
            };

            return socialConfigs;

        }

        //public static SocialConfigs GetConfigs()
        //{
        //    var allConfigs = _configsService.GetAllConfigs();
        //    var fbConfigs = allConfigs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.Facebook.Value()).FirstOrDefault();
        //    var TelegramConfigs = allConfigs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.Telegram.Value()).FirstOrDefault();
        //    var DailymotionConfigs = allConfigs.Where(conf => conf.EnumKey == SocialNetworkTypeEnum.DailyMotion.Value()).FirstOrDefault();
        //    var TechnicalConfigs = allConfigs.Where(conf=>conf.EnumKey == SocialNetworkTypeEnum.Technical.Value()).FirstOrDefault();
        //    var TwitterConfigs = allConfigs.Where(conf=>conf.EnumKey == SocialNetworkTypeEnum.Twitter.Value()).FirstOrDefault();
        //    var wpConfigs = allConfigs.Where(conf=>conf.EnumKey == SocialNetworkTypeEnum.WhatsApp.Value()).FirstOrDefault();
        //    var socialConfigs = new SocialConfigs
        //    {
        //        FacebookConfigs = new FacebookConfigs
        //        {
        //            Id = fbConfigs !=null ? fbConfigs.Id : "",
        //            EnumKey = SocialNetworkTypeEnum.Facebook.Value(),
        //            Application = fbConfigs != null ? (FacebookApp)JsonSerializer.Deserialize(fbConfigs?.JsonValue, typeof(FacebookApp)) : null
        //        },
        //        TelegramConfigs = new TelegramConfigs
        //        {
        //            Id = TelegramConfigs != null ? TelegramConfigs.Id : "",
        //            EnumKey = SocialNetworkTypeEnum.Telegram.Value(),
        //            Bot = TelegramConfigs != null ? (TelegramBot)JsonSerializer.Deserialize(TelegramConfigs?.JsonValue, typeof(TelegramBot)) : null
        //        },
        //        DailymotionConfigs = new DailymotionConfigs
        //        {
        //            Id = DailymotionConfigs !=null ? DailymotionConfigs.Id : "",
        //            EnumKey = SocialNetworkTypeEnum.DailyMotion.Value(),
        //            Application = DailymotionConfigs != null ? (DailymotionApp)JsonSerializer.Deserialize(DailymotionConfigs?.JsonValue, typeof(DailymotionApp)) : null
        //        },
        //        TechnicalConfigs = new TechnicalConfigs
        //        {
        //            Id = TechnicalConfigs !=null ? TechnicalConfigs.Id : "",
        //            EnumKey = SocialNetworkTypeEnum.Technical.Value(),
        //            appTechnicalConfigs = TechnicalConfigs != null ? (AppTechnicalConfigs)JsonSerializer.Deserialize(TechnicalConfigs?.JsonValue, typeof(AppTechnicalConfigs)) : null
        //        },
        //        TwitterConfigs = new TwitterConfigs
        //        {
        //            Id = TwitterConfigs !=null ? TwitterConfigs.Id : "",
        //            EnumKey = SocialNetworkTypeEnum.Twitter.Value(),
        //            Application = TwitterConfigs != null ? (TwitterApp)JsonSerializer.Deserialize(TwitterConfigs?.JsonValue, typeof(TwitterApp)) : null
        //        },
        //        WhatsappConfigs = new WhatsappConfigs
        //        {
        //            Id = wpConfigs !=null ? wpConfigs.Id : "",
        //            EnumKey = SocialNetworkTypeEnum.WhatsApp.Value(),
        //            Application = wpConfigs != null ? (WhatsappApp)JsonSerializer.Deserialize(wpConfigs?.JsonValue, typeof(WhatsappApp)) : null      
        //        }

        //    };
        //    //var configs = socialConfigs;
        //    return socialConfigs;
        //}

        public static void SetChannelExpired(this Channel channel)
        {
            channel.Status = StatusEnum.Expired.Value();
            Instances.Repositories.ChannelRepository.UpdateStatus(channel);
        }
        public static void UpdateCurrentState(this Operation operation, string state)
        {
            operation.CurrentState = state;
            Instances.Repositories.OperationRepository.UpdateCurrentState(operation);
        }

    }
}
