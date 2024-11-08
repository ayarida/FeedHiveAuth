using FeedHiveAuth.Areas.Social.Controllers;
using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : BaseController<ConfigsController>
    {
        protected ConfigsRepository _configsService = Instances.Repositories.ConfigsRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<ConfigsController> _logger;
        public ConfigsController(UserManager<IdentityUser> userManager, ILogger<ConfigsController> logger) : base  (userManager, logger)
        {

        }
/*
        [PermissionFilter("Configs_TechnicalConfigs")]
        [HttpGet]
        public ActionResult TechnicalConfigs()
        {
            var socialConfigs = SocialServiceHelper.getSocialConfigs();
            return View(socialConfigs);
        }*/

        [PermissionFilter("Configs_SocialConfigs")]
        [HttpGet]
        public ActionResult SocialConfigs()
        {
            var configs = _configsService.GetAllConfigs();
            var currUser = currUserId();
            //var parentId = (isAdminOrMaster()) ? currUser : "";

            Configs GetConfig(SocialNetworkTypeEnum enumValue) => configs.FirstOrDefault(conf => conf.EnumKey == enumValue.Value());

            T DeserializeOrDefault<T>(string jsonValue) where T : new() => string.IsNullOrEmpty(jsonValue) ? new T() : JsonSerializer.Deserialize<T>(jsonValue) ?? new T();

            var fbConf = GetConfig(SocialNetworkTypeEnum.Facebook);
            var telegramConf = GetConfig(SocialNetworkTypeEnum.Telegram);
            var wpConf = GetConfig(SocialNetworkTypeEnum.WhatsApp);
            var dailymotionConf = GetConfig(SocialNetworkTypeEnum.DailyMotion);
            var technicalConf = GetConfig(SocialNetworkTypeEnum.Technical);
            var twitterConf = GetConfig(SocialNetworkTypeEnum.Twitter);

            var fbApp = DeserializeOrDefault<FacebookApp>(fbConf?.JsonValue);
            var telegramBot = DeserializeOrDefault<TelegramBot>(telegramConf?.JsonValue);
            var wpApp = DeserializeOrDefault<WhatsappApp>(wpConf?.JsonValue);
            var dmApp = DeserializeOrDefault<DailymotionApp>(dailymotionConf?.JsonValue);
            var appTech = DeserializeOrDefault<AppTechnicalConfigs>(technicalConf?.JsonValue);
            var twitterApp = DeserializeOrDefault<TwitterApp>(twitterConf?.JsonValue);

            var socialConfigs = new SocialConfigs
            {
                TechnicalConfigs = technicalConf != null ? new TechnicalConfigs
                {
                    Id = technicalConf.Id,
                    EnumKey = SocialNetworkTypeEnum.Technical.Value(),
                    appTechnicalConfigs = appTech
                } : null,
                FacebookConfigs = fbConf != null ? new FacebookConfigs
                {
                    Id = fbConf.Id,
                    EnumKey = fbConf.EnumKey,
                    Application = fbApp
                } : null,
                TelegramConfigs = telegramConf != null ? new TelegramConfigs
                {
                    Id = telegramConf.Id,
                    EnumKey = SocialNetworkTypeEnum.Telegram.Value(),
                    Bot = telegramBot
                } : null,
                DailymotionConfigs = dailymotionConf != null ? new DailymotionConfigs
                {
                    Id = dailymotionConf.Id,
                    EnumKey = SocialNetworkTypeEnum.DailyMotion.Value(),
                    Application = dmApp
                } : null,
                TwitterConfigs = twitterConf != null ? new TwitterConfigs
                {
                    Id = twitterConf.Id,
                    EnumKey = SocialNetworkTypeEnum.Twitter.Value(),
                    Application = twitterApp
                } : null,
                WhatsappConfigs = wpConf != null ? new WhatsappConfigs
                {
                    Id = wpConf.Id,
                    EnumKey = SocialNetworkTypeEnum.WhatsApp.Value(),
                    Application = wpApp
                } : null
            };
            return View("~/Views/Configs/TechnicalConfigs.cshtml",socialConfigs);
        }
        [HttpPost]
        public async Task<IActionResult> Save(SocialConfigsViewModel configs)
        {
            //var parentId = (this.isAdminOrMaster()) ? this.currUserId() : ""; //Master or Admin
            var isSuperAdmin = this.isSuperAdmin();
            if(isSuperAdmin)
            {
                var socialConfigs = new SocialConfigs
                {
                    TechnicalConfigs = new TechnicalConfigs
                    {
                        Id = configs.TechnicalConfigsId,
                        EnumKey = SocialNetworkTypeEnum.Technical.Value(),
                        appTechnicalConfigs = new AppTechnicalConfigs
                        {
                            PublicUrl = configs.PublicUrl,
                            LocalUrl = configs.LocalUrl,
                            DefaultImageUrl = configs.DefaultImageUrl
                        },
                    },
                    FacebookConfigs = new FacebookConfigs
                    {
                        Id = configs.FacebookConfigsId,
                        EnumKey = SocialNetworkTypeEnum.Facebook.Value(),
                        Application = new FacebookApp
                        {
                            DisplayName = configs.DisplayName,
                            AppId = configs.AppId,
                            AppSecret = configs.AppSecret,
                            Enable = configs.EnableFb
                        },

                    },
                    TelegramConfigs = new TelegramConfigs
                    {
                        Id = configs.TelegramConfigsId,
                        EnumKey = SocialNetworkTypeEnum.Telegram.Value(),
                        Bot = new TelegramBot
                        {
                            Username = configs.UsernameTelegram,
                            Token = configs.TokenTelegram,
                            Enable = configs.EnableTelegram
                        }
                    },
                    DailymotionConfigs = new DailymotionConfigs
                    {
                        Id = configs.DailymotionConfigsId,
                        EnumKey = SocialNetworkTypeEnum.DailyMotion.Value(),
                        Application = new DailymotionApp
                        {
                            ChannelName = configs.ChannelName,
                            APIKey = configs.APIKey,
                            APISecret = configs.APISecret,
                            Username = configs.UsernameDM,
                            Password = configs.Password,
                            CallBackUrl = configs.CallBackUrl,
                            LocalPath = configs.LocalPath,
                            Enable = configs.EnableDM
                        }
                    },
                    TwitterConfigs = new TwitterConfigs
                    {
                        Id = configs.TwitterConfigsId,
                        EnumKey = SocialNetworkTypeEnum.Twitter.Value(),
                        Application = new TwitterApp
                        {
                            ScreenName = configs.ScreenName,
                            ConsumerKey = configs.ConsumerKey,
                            ConsumerSecret = configs.ConsumerSecret,
                            Token = configs.TokenX,
                            TokenSecret = configs.TokenSecret,
                            Enable = configs.EnableX

                        }
                    },
                    WhatsappConfigs = new WhatsappConfigs
                    {
                        Id = configs.WhatsappConfigsId,
                        EnumKey = SocialNetworkTypeEnum.WhatsApp.Value(),
                        Application = new WhatsappApp
                        {
                            //bbs = configs
                        }
                    }
                };
                if (socialConfigs.FacebookConfigs != null)
                {
                    string fbConfigsJson = JsonSerializer.Serialize(socialConfigs.FacebookConfigs.Application);
                    //var oldFbConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.Facebook.Value());

                    if (socialConfigs.FacebookConfigs.Id == null) //insert new 
                        _configsService.InsertConfigs(socialConfigs.FacebookConfigs.EnumKey, "FacebookConfigs", fbConfigsJson);
                    else //update old
                        _configsService.UpdateConfigs(socialConfigs.FacebookConfigs.EnumKey, socialConfigs.FacebookConfigs.Id, fbConfigsJson);
                }
                if (socialConfigs.TelegramConfigs != null)
                {
                    string telegramConfigsJson = JsonSerializer.Serialize(socialConfigs.TelegramConfigs.Bot);
                    //var oldtelegramConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.Telegram.Value());

                    if (socialConfigs.TelegramConfigs.Id == null) //insert new
                        _configsService.InsertConfigs(socialConfigs.TelegramConfigs.EnumKey, "TelegramConfigs", telegramConfigsJson);
                    else //update old
                        _configsService.UpdateConfigs(socialConfigs.TelegramConfigs.EnumKey, socialConfigs.TelegramConfigs.Id, telegramConfigsJson);
                }
                if (socialConfigs.DailymotionConfigs != null)
                {
                    string dailymotionConfigsJson = JsonSerializer.Serialize(socialConfigs.DailymotionConfigs.Application);
                    //var olddailymotionConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.DailyMotion.Value());
                    if (socialConfigs.DailymotionConfigs.Id == null)
                        _configsService.InsertConfigs(socialConfigs.DailymotionConfigs.EnumKey, "DailymotionConfigs", dailymotionConfigsJson);
                    else
                        _configsService.UpdateConfigs(socialConfigs.DailymotionConfigs.EnumKey, socialConfigs.DailymotionConfigs.Id, dailymotionConfigsJson);
                }
                if (socialConfigs.TechnicalConfigs != null)
                {
                    string appConfigsJson = JsonSerializer.Serialize(socialConfigs.TechnicalConfigs.appTechnicalConfigs);
                    //var oldTechnicalConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.Technical.Value());
                    if (socialConfigs.TechnicalConfigs.Id == null)
                        _configsService.InsertConfigs(socialConfigs.TechnicalConfigs.EnumKey, "TechnicalConfigs", appConfigsJson);
                    else
                        _configsService.UpdateConfigs(socialConfigs.TechnicalConfigs.EnumKey, socialConfigs.TechnicalConfigs.Id, appConfigsJson);
                }
                if (socialConfigs.WhatsappConfigs != null)
                {
                    string wpConfigsJson = JsonSerializer.Serialize(socialConfigs.WhatsappConfigs.Application);
                    if (socialConfigs.WhatsappConfigs.Id == null)
                        _configsService.InsertConfigs(socialConfigs.WhatsappConfigs.EnumKey, "WhatsappConfigs", wpConfigsJson);
                    else
                        _configsService.UpdateConfigs(socialConfigs.WhatsappConfigs.EnumKey, socialConfigs.WhatsappConfigs.Id, wpConfigsJson);
                }
                if (socialConfigs.TwitterConfigs != null)
                {
                    string twitterConfigsJson = JsonSerializer.Serialize(socialConfigs.TwitterConfigs.Application);
                    if (socialConfigs.TwitterConfigs.Id == null)
                        _configsService.InsertConfigs(socialConfigs.TwitterConfigs.EnumKey, "TwitterConfigs", twitterConfigsJson);
                    else
                        _configsService.UpdateConfigs(socialConfigs.TwitterConfigs.EnumKey, socialConfigs.TwitterConfigs.Id, twitterConfigsJson);
                }
            }
            
            return RedirectToAction("SocialConfigs", "Configs");
        }
    }
}
