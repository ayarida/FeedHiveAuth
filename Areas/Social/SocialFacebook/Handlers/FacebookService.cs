using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Clients;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Authorization;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Newtonsoft.Json.Linq;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Handlers
{
    public static class FacebookService
    {
        #region account
        public static IEnumerable<Channel> GetChannelsInfo(string baseCallbackUrl, string code, string state,out string msg)
        {
            var credentials = AuthorizationManager.ExchangeCode(baseCallbackUrl, code, out msg);
            if (credentials == null)
            {
                return null;
            }

            var stateObj = state.FromJson<JObject>();
            var accountType = EnumExtension.FromKey<SocialAccountTypeEnum>(stateObj["type"].Value<string>());
            switch (accountType)
            {
                case SocialAccountTypeEnum.Page:
                    return GetPages(credentials);
                case SocialAccountTypeEnum.AdManager:
                    return GetAdManagers(credentials);
                case SocialAccountTypeEnum.Profile:
                default:
                    var profile = GetProfile(credentials);
                    return profile == null ? new List<Channel>() : new List<Channel> { profile };
            }
        }
        private static Channel GetProfile(FacebookCredentials credentials)
        {
            var configs = SocialServiceHelper.GetConfigs().FacebookConfigs;
            var client = new AccountClient(configs, credentials.AccessToken);
            var result = client.GetUser();
            if (!result.IsSuccessful)
            {
                return null;
            }

            var user = result.Data;
            var pictureResult = client.GetProfilePic();
            var picture = pictureResult.IsSuccessful ? pictureResult.Data.Data : null;

            return new Channel
            {
                OriginalName = user.Name,
                ProfileImageUrl = picture?.Url,
                NetworkId = user.Id,
                NetworkUrl = "https://www.facebook.com/" + user.Id,
                Network = SocialNetworkTypeEnum.Facebook.Key(),
                Account = SocialAccountTypeEnum.Profile.Key(),
                Credentials = credentials.AccessToken
            };
        }

        private static IEnumerable<Channel> GetPages(FacebookCredentials credentials)
        {
            var configs = SocialServiceHelper.GetConfigs().FacebookConfigs;
            var client = new AccountClient(configs, credentials.AccessToken);
            var result = client.GetPages();
            if (!result.IsSuccessful) { return null; }

            var pages = result.Data.Data.ToList();
            while (result.Data.Paging.Next.IsNotNullOrEmpty())
            {
                result = client.GetPages(result.Data.Paging.Cursors.After);
                if (result.IsSuccessful && result.Data.Data.NotEmpty())
                {
                    pages.AddRange(result.Data.Data);
                }
            }

            var channels = new List<Channel>();
            foreach (var page in pages)
            {
                var pageClient = new AccountClient(configs, page.AccessToken);
                var pictureResult = pageClient.GetProfilePic();
                var picture = pictureResult.IsSuccessful ? pictureResult.Data.Data : null;
                channels.Add(new Channel
                {
                    OriginalName = page.Name,
                    ProfileImageUrl = picture?.Url,
                    NetworkId = page.Id,
                    NetworkUrl = "https://www.facebook.com/" + page.Id,
                    Network = SocialNetworkTypeEnum.Facebook.Key(),
                    Account = SocialAccountTypeEnum.Page.Key(),
                    Credentials = page.AccessToken
                });
            }

            return channels;
        }

        private static IEnumerable<Channel> GetAdManagers(FacebookCredentials credentials)
        {
            var configs = SocialServiceHelper.GetConfigs().FacebookConfigs;
            var client = new AccountClient(configs, credentials.AccessToken);
            var result = client.GetAdManagers();
            if (!result.IsSuccessful) { return null; }

            var adAccounts = result.Data.Data;
            var channels = new List<Channel>();
            foreach (var adAccount in adAccounts)
            {
                var pictureResult = client.GetProfilePic();
                var picture = pictureResult.IsSuccessful ? pictureResult.Data.Data : null;
                channels.Add(new Channel
                {
                    OriginalName = adAccount.Name,
                    ProfileImageUrl = picture?.Url,
                    NetworkId = adAccount.AccountId,
                    NetworkUrl = "https://business.facebook.com/home/accounts?business_id=" + adAccount.AccountId,
                    Network = SocialNetworkTypeEnum.Facebook.Key(),
                    Account = SocialAccountTypeEnum.AdManager.Key(),
                    Credentials = credentials.AccessToken
                });
            }

            return channels;
        }

        #endregion

        #region share 
/*        public static ServiceOperationResult Send(Operation operation, Channel channel)
        {
            var configs = SocialServiceHelper.GetConfigs(channel.SubscriptionId).FacebookConfigs;
            var operationData = operation.Parameters.FromJson<FacebookOperationData>();

            var scheduled = operationData.ScheduleTime.HasValue && operationData.ScheduleTime.Value > DateTime.Now;
            var scheduledPublishTime = scheduled ? operationData.ScheduleTime.Value.UtcDate(operation.SubscriptionId).ToUnixTimespans() : 0;

            if (operation.MediaItem() == null)
            {
                var client = new FeedClient(configs, channel.Credentials);
                var result = client.Publish(operationData.Text, operationData.Link, !scheduled, scheduledPublishTime);
                return ProcessFeedResult(result, channel, operation, operationData);
            }
            switch (EnumExtension.FromValue<MediaTypeEnum>(operation.MediaItem().Type))
            {
                case MediaTypeEnum.Image:
                    var photoClient = new PhotoClient(configs, channel.Credentials);
                    var photoResult = photoClient.Publish(operationData.Text, operation.MediaItem().PublicUrl(false).ComposeSafeUrl(false), !scheduled, scheduledPublishTime);
                    return ProcessPhotoResult(photoResult, channel, operation, operationData);
                case MediaTypeEnum.Video:
                    //var videoClient = new VideoClient(configs, channel.Credentials, "https://graph-video.facebook.com");
                    //var uploadResult = ChunkUpload(videoClient, operation, out var error);
                    var videoClient = new VideoClient(configs, channel.Credentials);
                    var uploadResult = videoClient.Publish(operationData.Title, operationData.Text, operation.MediaItem().PublicUrl(false).ComposeSafeUrl(false), !scheduled, scheduledPublishTime);
                    if (uploadResult.IsSuccessful)
                    {
                        //var result = videoClient.FinishUpload(uploadResult.Data.UploadSessionId, null, operationData.Text, !scheduled, scheduledPublishTime);
                        Instances.Repositories.OperationsRepository.UpdateStatus(operation.Id, StatusEnum.Waiting.Value());
                        //videoClient = new VideoClient(configs, channel.Credentials);
                        //var video = CheckVideoStatus(operation, videoClient, uploadResult.Data.VideoId);
                        var video = CheckVideoStatus(operation, videoClient, uploadResult.Data.Id);
                        return ProcessVideoResult(video, channel, operation, operationData);
                    }
                    else
                    {
                        //return ProcessFailed(error, channel, operation);
                        return ProcessFailed(uploadResult.Data.Error, channel, operation);
                    }
                default:
                    return new ServiceOperationResult
                    {
                        Success = false,
                        Message = "Unsupported media type"
                    };
            }
        }
*/
        #endregion

    }
}
