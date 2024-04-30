using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Clients;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Feed;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Photo;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Newtonsoft.Json.Linq;
using RestSharp;
using Video = FeedHiveAuth.Areas.Social.SocialFacebook.Models.Video.Video;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Handlers
{
    public static class FacebookService
    {
        public static MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;

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
            var pictureRes = client.GetProfilePictureUrl(user.Id,credentials.AccessToken);
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
                var pictureRes = pageClient.GetProfilePictureUrl(page.Id, page.AccessToken);
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
        public static ServiceOperationResult Send(Operation operation, Channel channel)
        {
            var configs = SocialServiceHelper.GetConfigs().FacebookConfigs;
            var operationData = operation.Parameters.FromJson<FacebookOperationData>();

            var scheduled = operationData.ScheduleTime.HasValue && operationData.ScheduleTime.Value > DateTime.Now;
            var scheduledPublishTime = 0;

            if (operation.MediaId == null) { 
                var client = new FeedClient(configs, channel.Credentials);
                var result = client.Publish(operationData.Text, operationData.Link, !scheduled, scheduledPublishTime);
                return ProcessFeedResult(result, channel, operation, operationData);
            }
            //get the media from operation then retrieve from db to get its enum value and publish based on type
            var getMedia = _mediaItemService.Get(operation.MediaId);
            switch (EnumExtension.FromValue<MediaTypeEnum>(getMedia.Type))
            {
                case MediaTypeEnum.Image:
                    var photoClient = new PhotoClient(configs, channel.Credentials);
                    var photoResult = photoClient.Publish(operationData.Text, getMedia.Path, !scheduled, scheduledPublishTime);
                    return ProcessPhotoResult(photoResult, channel, operation, operationData);
                case MediaTypeEnum.Video:                   
                    var videoClient = new VideoClient(configs, channel.Credentials);
                    var uploadResult = videoClient.Publish(operationData.Title, operationData.Text, getMedia.Path, !scheduled, scheduledPublishTime);
                    if (uploadResult.IsSuccessful)
                    {
                       
                        Instances.Repositories.OperationRepository.UpdateStatus(operation.Id, StatusEnum.Waiting.Value());        
                        var video = CheckVideoStatus(operation, videoClient, uploadResult.Data.Id);
                        return ProcessVideoResult(video, channel, operation, operationData);
                    }
                    else
                    {
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
        private static IRestResponse<Video> CheckVideoStatus(Operation operation, VideoClient client, string videoId)
        {
            try
            {
                operation.UpdateCurrentState("Media is being processed by facebook");
                var parms = new Dictionary<string, object>
                {
                    { "fields", "id,title,description,status" }
                };
                var result = client.Get<Social.SocialFacebook.Models.Video.Video>(videoId, parms);
                if (!result.IsSuccessful)
                {
                    return (IRestResponse<Video>)result;
                }

                var processingInfo = result.Data.Status.VideoStatus;
                switch (processingInfo)
                {
                    case "ready":
                    case "error":
                        return (IRestResponse<Video>)result;
                    default:
                        Thread.Sleep(60 * 1000);
                        return CheckVideoStatus(operation, client, videoId);
                }
            }
            catch (Exception ex)
            {
                //operation.AddMessageOperation(ex.FullMessage());
                return null;
            }
            finally
            {
                operation.UpdateCurrentState("Media processing by facebook completed");
            }
        }
        private static ServiceOperationResult ProcessPhotoResult(IRestResponse<PhotoResponse> result, Channel channel, Operation operation, FacebookOperationData operationData)
        {
            if (result.IsSuccessful)
            {
                var meta = new SocialMetaResult
                {
                    Id = result.Data.PostId,
                    NetworkId = operationData.NetworkId,
                    OperationId = operation.Id
                };
                return ProcessSuccess(meta, result.Data.Serialize(), operation);
            }
            else
            {
                return ProcessFailed(result.Data.Error, channel, operation);
            }
        }

        private static ServiceOperationResult ProcessVideoResult(IRestResponse<Video> result, Channel channel, Operation operation, FacebookOperationData operationData)
        {
            if (result.IsSuccessful && result.Data.Status.VideoStatus.EqualsIgnoreCase("ready"))
            {
                var meta = new SocialMetaResult
                {
                    Id = result.Data.Id,
                    NetworkId = operationData.NetworkId,
                    OperationId = operation.Id
                };
                //meta.AddMediaServiceMeta(SocialNetworkTypeEnum.Facebook.Key(), operation.MediaId.ToString());
                return ProcessSuccess(meta, result.Data.Serialize(), operation);
            }
            else
            {
                return ProcessFailed(result.Data.Error, channel, operation);
            }
        }
        private static ServiceOperationResult ProcessFeedResult(IRestResponse<FeedResponse> result, Channel channel, Operation operation, FacebookOperationData operationData)
        {
            if (result.IsSuccessful)
            {
                var meta = new SocialMetaResult
                {
                    Id = result.Data.Id,
                    NetworkId = operationData.NetworkId,
                    OperationId = operation.Id
                };
                return ProcessSuccess(meta, result.Data.Serialize(), operation);
            }
            else
            {
                return ProcessFailed(result.Data.Error, channel, operation);
            }
        }
        #endregion

        private static ServiceOperationResult ProcessFailed(Error error, Channel channel, Operation operation)
        {
            if (error != null && (error.Code == 10 || error.Code == 190 || (error.Code >= 200 && error.Code <= 299)))
            {
                channel.SetChannelExpired();
            }

            return new ServiceOperationResult
            {
                Success = false,
                Message = error?.ErrorUserMsg ?? error?.Message
            };
        }

        private static ServiceOperationResult ProcessSuccess(SocialMetaResult socialResult, string result, Operation operation)
        {
            return new ServiceOperationResult
            {
                Success = true,
                Result = result
            };
        }

    }
}
