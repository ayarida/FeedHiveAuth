using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : Controller
    {
        protected ConfigsRepository _configsService = Instances.Repositories.ConfigsRepository;
        protected UserRepository _userService = Instances.Repositories.UserRepository;

        [PermissionFilter("Configs_TechnicalConfigs")]
        [HttpGet]
        public ActionResult TechnicalConfigs()
        {
            var socialConfigs = SocialServiceHelper.getSocialConfigs();
            return View(socialConfigs);
        }

        [HttpGet]
        public SocialConfigs SocialConfigs()
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
        [HttpPost]
        public IActionResult Save(SocialConfigsViewModel configs)
        {
           
            var isAdmin = User.IsInRole("Admin");
            var isMaster = User.IsInRole("Master");
            var masterId = GetCurrentUserId().GetAwaiter().GetResult();
            var socialConfigs = new SocialConfigs
            {
                TechnicalConfigs = new TechnicalConfigs
                {
                    Id = configs.TechnicalConfigsId,
                    EnumKey = SocialNetworkTypeEnum.Technical.Value(),
                    appTechnicalConfigs = new AppTechnicalConfigs {
                        PublicUrl = configs.PublicUrl,
                        LocalUrl = configs.LocalUrl,
                        DefaultImageUrl = configs.DefaultImageUrl
                    }
                },
                FacebookConfigs = new FacebookConfigs
                {
                    Id = configs.FacebookConfigsId,
                    EnumKey =SocialNetworkTypeEnum.Facebook.Value(),
                    Application = new FacebookApp
                    {
                        DisplayName = configs.DisplayName,
                        AppId = configs.AppId,
                        AppSecret = configs.AppSecret,
                        Enable = configs.EnableFb
                    }
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
            if (isMaster) //Master
            {
                masterId = masterId;
            }
            else //Admin and never an Editor 
            {
                masterId = _userService.GetUserParent(masterId);
            }
            
            if (socialConfigs.FacebookConfigs != null)
            {
                string fbConfigsJson = JsonSerializer.Serialize(socialConfigs.FacebookConfigs.Application);
                //var oldFbConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.Facebook.Value());
                
                if(socialConfigs.FacebookConfigs.Id == null) //insert new 
                    _configsService.InsertConfigs(socialConfigs.FacebookConfigs.EnumKey,"FacebookConfigs", fbConfigsJson, masterId);
                else //update old
                    _configsService.UpdateConfigs(socialConfigs.FacebookConfigs.EnumKey, socialConfigs.FacebookConfigs.Id, fbConfigsJson);
            }
            if (socialConfigs.TelegramConfigs != null)
            {
                string telegramConfigsJson = JsonSerializer.Serialize(socialConfigs.TelegramConfigs.Bot);
                //var oldtelegramConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.Telegram.Value());
                
                if (socialConfigs.TelegramConfigs.Id == null) //insert new
                    _configsService.InsertConfigs(socialConfigs.TelegramConfigs.EnumKey, "TelegramConfigs", telegramConfigsJson, masterId); 
                else //update old
                    _configsService.UpdateConfigs(socialConfigs.TelegramConfigs.EnumKey,socialConfigs.TelegramConfigs.Id,telegramConfigsJson);
            }
            if(socialConfigs.DailymotionConfigs != null)
            {
                string dailymotionConfigsJson = JsonSerializer.Serialize(socialConfigs.DailymotionConfigs.Application);
                //var olddailymotionConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.DailyMotion.Value());
                if(socialConfigs.DailymotionConfigs.Id == null)
                    _configsService.InsertConfigs(socialConfigs.DailymotionConfigs.EnumKey, "DailymotionConfigs", dailymotionConfigsJson, masterId);
                else
                    _configsService.UpdateConfigs(socialConfigs.DailymotionConfigs.EnumKey, socialConfigs.DailymotionConfigs.Id, dailymotionConfigsJson);
            }
            if (socialConfigs.TechnicalConfigs != null)
            {
                string appConfigsJson = JsonSerializer.Serialize(socialConfigs.TechnicalConfigs.appTechnicalConfigs);
                //var oldTechnicalConf = _configsService.GetConfigsByKey(SocialNetworkTypeEnum.Technical.Value());
                if (socialConfigs.TechnicalConfigs.Id == null)
                    _configsService.InsertConfigs(socialConfigs.TechnicalConfigs.EnumKey, "TechnicalConfigs", appConfigsJson, masterId);
                else
                    _configsService.UpdateConfigs(socialConfigs.TechnicalConfigs.EnumKey, socialConfigs.TechnicalConfigs.Id, appConfigsJson);
            }
            if (socialConfigs.WhatsappConfigs != null)            {
                string wpConfigsJson = JsonSerializer.Serialize(socialConfigs.WhatsappConfigs.Application);
                if (socialConfigs.WhatsappConfigs.Id == null)
                    _configsService.InsertConfigs(socialConfigs.WhatsappConfigs.EnumKey, "WhatsappConfigs", wpConfigsJson, masterId);
                else
                    _configsService.UpdateConfigs(socialConfigs.WhatsappConfigs.EnumKey, socialConfigs.WhatsappConfigs.Id, wpConfigsJson);
            }
            if(socialConfigs.TwitterConfigs != null)
            {
                string twitterConfigsJson = JsonSerializer.Serialize(socialConfigs.TwitterConfigs.Application);
                if (socialConfigs.TwitterConfigs.Id == null)
                    _configsService.InsertConfigs(socialConfigs.TwitterConfigs.EnumKey, "TwitterConfigs", twitterConfigsJson, masterId);
                else
                    _configsService.UpdateConfigs(socialConfigs.TwitterConfigs.EnumKey,socialConfigs.TwitterConfigs.Id, twitterConfigsJson);
            }
            return RedirectToAction("TechnicalConfigs", "Configs");
        }
        public async Task<string> GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
    }
}
