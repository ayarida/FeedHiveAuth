using FeedHiveAuth.Areas.Social.Controllers;
using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Tweetinvi.Core.Extensions;

namespace FeedHiveAuth.Controllers
{
    public class ConfigsController : BaseController<ConfigsController>
    {
        private readonly ConfigsRepository _configsService = Instances.Repositories.ConfigsRepository;
        private readonly UserRepository _userService = Instances.Repositories.UserRepository;
        private readonly ILogger<ConfigsController> _logger;

        public ConfigsController(UserManager<ApplicationUser> userManager, ILogger<ConfigsController> logger)
            : base(userManager, logger)
        {
        }

        // ============================================
        //  HELPER METHODS
        // ============================================

        private T Deserialize<T>(string json) where T : new()
            => string.IsNullOrEmpty(json) ? new T() : (JsonSerializer.Deserialize<T>(json) ?? new T());


        private void SaveConfig(int enumKey, string className, object model, string parentId, string id)
        {
            var json = JsonSerializer.Serialize(model);

            if (id == null)
                _configsService.InsertConfigs(enumKey, className, json, parentId);
            else
                _configsService.UpdateConfigs(enumKey, id, json);
        }


        // ============================================
        //  GET SOCIAL CONFIGS
        // ============================================

        [PermissionFilter("Configs_SocialConfigs")]
        [HttpGet]
        public ActionResult SocialConfigs()
        {
            var configs = _configsService.GetAllConfigs().ToList();
            var currUser = currUserId();
            var parentId = isSuperAdmin() ? currUser : "";

            Configs Find(SocialNetworkTypeEnum e) =>
                configs.FirstOrDefault(c => c.EnumKey == e.Value());

            var model = new SocialConfigs
            {
                TechnicalConfigs = BuildTechConfig(Find(SocialNetworkTypeEnum.Technical)),
                FacebookConfigs = BuildFacebookConfig(Find(SocialNetworkTypeEnum.Facebook)),
                TelegramConfigs = BuildTelegramConfig(Find(SocialNetworkTypeEnum.Telegram)),
                DailymotionConfigs = BuildDailymotionConfig(Find(SocialNetworkTypeEnum.DailyMotion)),
                TwitterConfigs = BuildTwitterConfig(Find(SocialNetworkTypeEnum.Twitter)),
                WhatsappConfigs = BuildWhatsappConfig(Find(SocialNetworkTypeEnum.WhatsApp))
            };

            return View("~/Views/Configs/TechnicalConfigs.cshtml", model);
        }

        private TechnicalConfigs BuildTechConfig(Configs cfg)
        {
            if (cfg == null) return null;

            return new TechnicalConfigs
            {
                Id = cfg.Id,
                EnumKey = SocialNetworkTypeEnum.Technical.Value(),
                appTechnicalConfigs = Deserialize<AppTechnicalConfigs>(cfg.JsonValue)
            };
        }

        private FacebookConfigs BuildFacebookConfig(Configs cfg)
        {
            if (cfg == null) return null;

            return new FacebookConfigs
            {
                Id = cfg.Id,
                EnumKey = cfg.EnumKey,
                Application = Deserialize<FacebookApp>(cfg.JsonValue)
            };
        }

        private TelegramConfigs BuildTelegramConfig(Configs cfg)
        {
            if (cfg == null) return null;

            return new TelegramConfigs
            {
                Id = cfg.Id,
                EnumKey = cfg.EnumKey,
                Bot = Deserialize<TelegramBot>(cfg.JsonValue)
            };
        }

        private DailymotionConfigs BuildDailymotionConfig(Configs cfg)
        {
            if (cfg == null) return null;

            return new DailymotionConfigs
            {
                Id = cfg.Id,
                EnumKey = cfg.EnumKey,
                Application = Deserialize<DailymotionApp>(cfg.JsonValue)
            };
        }

        private TwitterConfigs BuildTwitterConfig(Configs cfg)
        {
            if (cfg == null) return null;

            return new TwitterConfigs
            {
                Id = cfg.Id,
                EnumKey = cfg.EnumKey,
                Application = Deserialize<TwitterApp>(cfg.JsonValue)
            };
        }

        private WhatsappConfigs BuildWhatsappConfig(Configs cfg)
        {
            if (cfg == null) return null;

            return new WhatsappConfigs
            {
                Id = cfg.Id,
                EnumKey = cfg.EnumKey,
                Application = Deserialize<WhatsappApp>(cfg.JsonValue)
            };
        }


        // ============================================
        //  SAVE CONFIGS
        // ============================================

        [HttpPost]
        public IActionResult Save(SocialConfigsViewModel vm)
        {
            var parentId = isAdminOrMaster() ? currUserId() : "";
            //if (parentId.IsNullOrEmpty())
            //    return RedirectToAction("SocialConfigs");

            // ------------------ TECH ------------------
            SaveConfig(
                SocialNetworkTypeEnum.Technical.Value(),
                "TechnicalConfigs",
                new AppTechnicalConfigs
                {
                    PublicUrl = vm.PublicUrl,
                    LocalUrl = vm.LocalUrl,
                    DefaultImageUrl = vm.DefaultImageUrl
                },
                parentId,
                vm.TechnicalConfigsId
            );

            // ------------------ FACEBOOK ------------------
            SaveConfig(
                SocialNetworkTypeEnum.Facebook.Value(),
                "FacebookConfigs",
                new FacebookApp
                {
                    DisplayName = vm.DisplayName,
                    AppId = vm.AppId,
                    AppSecret = vm.AppSecret,
                    Enable = vm.EnableFb
                },
                parentId,
                vm.FacebookConfigsId
            );

            // ------------------ TELEGRAM ------------------
            SaveConfig(
                SocialNetworkTypeEnum.Telegram.Value(),
                "TelegramConfigs",
                new TelegramBot
                {
                    Username = vm.UsernameTelegram,
                    Token = vm.TokenTelegram,
                    Enable = vm.EnableTelegram
                },
                parentId,
                vm.TelegramConfigsId
            );

            // ------------------ DAILY MOTION ------------------
            SaveConfig(
                SocialNetworkTypeEnum.DailyMotion.Value(),
                "DailymotionConfigs",
                new DailymotionApp
                {
                    ChannelName = vm.ChannelName,
                    APIKey = vm.APIKey,
                    APISecret = vm.APISecret,
                    Username = vm.UsernameDM,
                    Password = vm.Password,
                    CallBackUrl = vm.CallBackUrl,
                    LocalPath = vm.LocalPath,
                    Enable = vm.EnableDM
                },
                parentId,
                vm.DailymotionConfigsId
            );

            // ------------------ TWITTER / X ------------------
            SaveConfig(
                SocialNetworkTypeEnum.Twitter.Value(),
                "TwitterConfigs",
                new TwitterApp
                {
                    ScreenName = vm.ScreenName,
                    ConsumerKey = vm.ConsumerKey,
                    ConsumerSecret = vm.ConsumerSecret,
                    Token = vm.TokenX,
                    TokenSecret = vm.TokenSecret,
                    Enable = vm.EnableX
                },
                parentId,
                vm.TwitterConfigsId
            );

            // ------------------ WHATSAPP ------------------
            SaveConfig(
                SocialNetworkTypeEnum.WhatsApp.Value(),
                "WhatsappConfigs",
                new WhatsappApp
                {
                    // Fill later
                },
                parentId,
                vm.WhatsappConfigsId
            );

            return RedirectToAction("SocialConfigs");
        }
    }
}
