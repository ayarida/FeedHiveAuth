using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialFacebook.Clients;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Media;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Tweet;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models;
using Microsoft.CodeAnalysis;
using RestSharp;
using System.Net;
using FeedHiveAuth.Areas.Social.SocialTwitter.Clients;
using AccountClient = FeedHiveAuth.Areas.Social.SocialTwitter.Clients.AccountClient;
using Microsoft.Extensions.Logging;
using FeedHiveAuth.Areas.Identity.Pages.Account;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Handlers
{
    public class TwitterService
    {
        private readonly ILogger<ExternalLoginModel> _logger;
        #region account
        protected readonly TwitterConfigs tConfigs;
        public static Channel GetChannelInfo(string token, string verifier)
        {
            var credentials = AuthorizationManager.GenerateAccessToken(token, verifier);
            if (credentials == null)
            {
                return null;
            }

            var configs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;
            var accountClient = new AccountClient(configs, credentials);
            var users = accountClient.GetUser();
            if (!users.IsSuccessful)
            {
                return null;
            }

            var user = users.Data.FirstOrDefault(x => x.Id.Equals(credentials.UserId));
            if (user == null)
            {
                return null;
            }

            return new Channel()
            {
                Network = SocialNetworkTypeEnum.Twitter.Key(),
                OriginalName = user.Name,
                Description = user.Description,
                ProfileImageUrl = user.ProfileImageUrlHttps,
                NetworkId = credentials.UserId.ToString(),
                NetworkUrl = "https://twitter.com/" + user.ScreenName,
                Credentials = credentials.Serialize(),
                Account = SocialAccountTypeEnum.Profile.Key(),
            };
        }

        #endregion

        #region share

        public static ServiceOperationResult Send(Operation operation, Channel channel)
        {
            var token = channel.Credentials.FromJson<TwitterCredentials>();
            var operationData = operation.Parameters.FromJson<TwitterOperationData>();
            var mediaIds = UploadMedia(operation, token, operationData);

            var configs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;

            var client = new TweetClient(configs, token);
            operation.UpdateCurrentState("Sharing tweet");
            if (operationData.Link.IsNotNullOrEmpty() && !operationData.Text.ContainsIgnoreCase(operationData.Link))
            {
                operationData.Text += " " + operationData.Link;
            }

            var result = client.Tweet(operationData.Text, configs, token, string.Join(",", mediaIds));
            operation.UpdateCurrentState("Processing result");
            return ProcessTweetResult(result, channel, operation, operationData);
        }

        private static List<ulong> UploadMedia(Operation operation, TwitterCredentials token, TwitterOperationData operationData)
        {
            var mediaIds = new List<ulong>();
            if (operation.MediaId != null || operationData.MediaItemIds.NotEmpty())
            {
                var mediaItems = operation._mediaItem != null ? new List<MediaItem> { operation._mediaItem } : Instances.Repositories.MediaItemRepository.Get(operationData.MediaItemIds);
                var index = 1;
                foreach (var mediaItem in mediaItems)
                {
                    //operation.AddMessageOperation($"Uploading media {index} from {mediaItems.Count()}");
                    var uploadStatus = ChunkUpload(mediaItem, token, operation);
                    if (uploadStatus != null && (uploadStatus.Data.ProcessingInfo == null || uploadStatus.Data.ProcessingInfo.State.EqualsIgnoreCase("succeeded")))
                    {
                        var mediaId = uploadStatus.Data.MediaId;
                        //operation.AddMessageOperation($"Media Item successfully uploaded with id {mediaId}");
                        ProcessMediaResult(mediaId, mediaItem.Id.ToString(), operation.Id, operationData.NetworkId);
                        mediaIds.Add(mediaId);
                    }
                    Instances.Repositories.OperationRepository.UpdateStatus(operation.Id, StatusEnum.Processing.Value());
                    index++;
                }
            }

            return mediaIds;
        }

        #endregion

        #region delete

        //public static ServiceOperationResult Delete(Operation operation, Channel channel)
        //{
        //    var configs = SocialServiceHelper.GetConfigs(channel.SubscriptionId).TwitterConfigs;
        //    var token = channel.Credentials.FromJson<TwitterCredentials>();
        //    var operationData = operation.Parameters.FromJson<DeleteOperationData>();
        //    var client = new TweetClient(configs, token);

        //    var result = client.Destroy(operationData.SocialPostId);
        //    return ProcessDeleteResult(result, channel, operation, operationData);
        //}

        #endregion

        #region upload

        public static IRestResponse<UploadStatus> ChunkUpload(MediaItem mediaItem, TwitterCredentials token, Operation operation)
        {
            Stream fileStream = null;
            try
            {
                var isVideo = mediaItem.Type.Equals(MediaTypeEnum.Video.Value());

                //fileStream = MediaExtension.GetStream((isVideo ? null : "twitter"));
                fileStream = mediaItem.GetStream();
                var length = fileStream.Length;
                var type = MimeTypes.GetMimeType(mediaItem.Path);
                var configs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;
                var client = new MediaClient(configs, token);
                operation.UpdateCurrentState("Uploading media process started");

                var category = isVideo ? "tweet_video" : null;
                var init = client.InitUpload(length, type, category);
                if (!init.IsSuccessful)
                {
                    //operation.AddMessageOperation("Failed to initialize upload: " + string.Join(",", init.Data.Errors.Select(x => x.Message)));
                    return null;
                }

                var chunkSize = configs.Application.ChunkSizeMB * 1000000;
                var start = 0;
                var index = 0;
                var remaining = length;

                do
                {
                    var size = Convert.ToInt32(remaining > chunkSize ? chunkSize : remaining);
                    var chunk = new byte[size];
                    fileStream.Read(chunk, 0, Convert.ToInt32(size));
                    var chunkAppend = ChunkUploadWithRetry(operation, client, init.Data.MediaId, chunk, index);
                    if (chunkAppend.IsSuccessful)
                    {
                        //operation.UpdateProgress(100 * (start + size) / length);
                    }
                    else
                    {
                       // operation.AddMessageOperation("Upload process failed: " + string.Join(",", chunkAppend.Data.Errors.Select(x => x.Message)));
                        return null;
                    }
                    start += size;
                    remaining -= size;
                    index++;
                } while (remaining > 0);

                var finilize = client.FinalizeUpload(init.Data.MediaId);
                if (!finilize.IsSuccessful)
                {
                    //operation.AddMessageOperation("Failed to finilize upload: " + string.Join(",", finilize.Data.Errors.Select(x => x.Message)));
                    return null;
                }

                if (finilize.Data.ProcessingInfo != null)
                {
                    Instances.Repositories.OperationRepository.UpdateStatus(operation.Id, StatusEnum.Waiting.Value());
                }

                return finilize.Data.ProcessingInfo == null ? finilize : CheckMediaStatus(operation, client, init.Data.MediaId, finilize.Data.ProcessingInfo.CheckAfterSecs);
            }
            catch (Exception ex)
            {
               // _logger.Error(typeof(TwitterService), "Media upload to twitter failed", ex);
                //operation.AddMessageOperation("Upload failed. " + ex.FullMessage());
                return null;
            }
            finally
            {
                fileStream?.Close();
            }
        }

        private static IRestResponse<TwitterWebResponse> ChunkUploadWithRetry(Operation operation, MediaClient client, ulong id, byte[] chunk, int index, int retry = 5, int sleep = 1000)
        {
            var retryTimes = 6 - retry;
            operation.UpdateCurrentState("Uploading media retry " + retryTimes);
            var result = client.AppendUpload(id, chunk, index);
            if (result.IsSuccessful || retry <= 0)
            {
                return result;
            }

            //operation.AddMessageOperation("Video upload process nb. " + retryTimes + " failed, reason: " + result.Data.Errors.FirstOrDefault()?.Message);
            Thread.Sleep(sleep *= 2);
            return ChunkUploadWithRetry(operation, client, id, chunk, index, --retry, sleep);
        }

        #endregion

        #region process result

        private static void ProcessMediaResult(ulong mediaId, string mediaItemId, string operationId, string networkId)
        {
            var mediaMeta = new SocialMetaResult
            {
                Id = mediaId.ToString(),
                NetworkId = networkId,
                OperationId = operationId
            };
            //mediaMeta.AddMediaServiceMeta(SocialNetworkTypeEnum.Twitter.Key(), mediaItemId);
        }

        private static IRestResponse<UploadStatus> CheckMediaStatus(Operation operation, MediaClient client, ulong mediaId, int? checkAfter = null)
        {
            try
            {
                operation.UpdateCurrentState("Media is being processed by twitter");
                if (checkAfter.HasValue)
                {
                    Thread.Sleep(checkAfter.Value * 1000);
                }

                var result = client.UploadStatus(mediaId);
                if (!result.IsSuccessful)
                {
                    //operation.AddMessageOperation("Checking for media status failed: " + string.Join(",", result.Data.Errors.Select(x => x.Message)));
                    return null;
                }

                var processingInfo = result.Data.ProcessingInfo;
                switch (processingInfo.State)
                {
                    case "succeeded":
                        return result;
                    case "failed":
                        //operation.AddMessageOperation("Media processing failed: " + processingInfo.Error.Message);
                        return null;
                    default:
                        var secs = processingInfo.CheckAfterSecs ?? checkAfter ?? 60;
                        operation.UpdateCurrentState("Media is being processed by twitter. Will check again after " + secs + " seconds");
                        Thread.Sleep(secs * 1000);
                        return CheckMediaStatus(operation, client, mediaId);
                }
            }
            catch (Exception ex)
            {
                //operation.AddMessageOperation(ex.FullMessage());
                return null;
            }
            finally
            {
                operation.UpdateCurrentState("Media processing by twitter completed");
            }
        }

        private static ServiceOperationResult ProcessTweetResult(IRestResponse<Tweet> result, Channel channel, Operation operation, TwitterOperationData operationData)
        {
            if (result.IsSuccessful)
            {
                var postMeta = new SocialMetaResult
                {
                    Id = result.Data.IdStr,
                    NetworkId = operationData.NetworkId,
                    OperationId = operation.Id
                };
                return ProcessSuccess(postMeta, result.Data, operation);
            }
            else
            {
                return ProcessFailed(result.Data.Errors, channel, operation);
            }
        }

        //private static ServiceOperationResult ProcessDeleteResult(IRestResponse<Tweet> result, Channel channel, Operation operation, DeleteOperationData operationData)
        //{
        //    if (result.IsSuccessful || result.StatusCode.Equals(HttpStatusCode.NotFound))
        //    {
        //        var socialResult = new SocialMetaResult { Id = operationData.SocialPostId, NetworkId = operationData.NetworkId };
        //        //socialResult.DeletePostServiceMeta(SocialNetworkTypeEnum.Twitter.Key(), operation.PostId.ToString());
        //        //socialResult.DeleteMediaServiceMeta(SocialNetworkTypeEnum.Twitter.Key(), operation.MediaId.ToString());
        //        return new ServiceOperationResult
        //        {
        //            Success = true,
        //            Result = result.Data.Serialize()
        //        };
        //    }
        //    else
        //    {
        //        return ProcessFailed(result.Data.Errors, channel, operation);
        //    }
        //}

        private static ServiceOperationResult ProcessFailed(IEnumerable<Error> errors, Channel channel, Operation operation)
        {
            if (errors.Any(x => x.Code.In(new List<int> { 32, 63, 64, 87, 89, 93, 99, 135, 220 })))
            {
                channel.SetChannelExpired();
            }

            return new ServiceOperationResult
            {
                Success = false,
                Message = string.Join(", ", errors.Select(x => x.Message))
            };
        }

        private static ServiceOperationResult ProcessSuccess(SocialMetaResult socialResult, Tweet result, Operation operation)
        {
            //socialResult.AddPostServiceMeta(SocialNetworkTypeEnum.Twitter.Key(), operation.PostId.ToString());
            return new ServiceOperationResult
            {
                Success = true,
                Result = result.Serialize()
            };
        }

        #endregion
    }
}
