using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialDailymotion.Clients;
using FeedHiveAuth.Areas.Social.SocialDailymotion.Models;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FeedHiveAuth.Areas.Social.SocialDailymotion.Handlers
{
    public class DailymotionService
    {
        public PostRepository _postService = new PostRepository();
        private class finalApiResponse
        {
            public string id { get; set; }
            public string status { get; set; }
            public string method { get; set; }
            public string result { get; set; }
        }
        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 20 * 60 * 1000;
                return w;
            }
        }

        public static async Task<UploadedResponse> PublishVideo(string videourl,MediaItem media)
        {
            try
            {
                var client = new WebClient();
                var socialConfigs = SocialServiceHelper.GetConfigs();
                var dailymotionConfigs = socialConfigs.DailymotionConfigs;
                var ext = videourl.GetFileExtension();
                var filePath = string.Format("{0}{1}{2}", dailymotionConfigs.Application.LocalPath, Guid.NewGuid(), "." + ext);

                client.DownloadFile(videourl, filePath);

                var accessToken = DailymotionClient.GetAccessToken(dailymotionConfigs);

                if (accessToken == null)
                {
                    throw new Exception("invalid access token");
                }
                DailymotionClient.CreateAuthorized(accessToken, dailymotionConfigs);

                var uploadUrl = DailymotionClient.GetFileUploadUrl(accessToken);
                var response = DailymotionClient.GetFileUploadResponse(filePath, accessToken, uploadUrl);
                var uploadedResponse = DailymotionClient.PublishVideo(response, accessToken, media);
                return uploadedResponse;
            }
            catch (Exception ex)
            {
                var response = new UploadedResponse();
                var txt = ex.ToString();
                response.txt = txt;
                return response;

            }
        }
        private static bool IsAccessTokenExpired(DateTime tokenIssuedTime)
        {
            var expirationDurationInSeconds = 36000;
            // Calculate the expiration time by adding the duration to the token issuance time
            DateTime expirationTime = tokenIssuedTime.AddSeconds(expirationDurationInSeconds);

            // Check if the current time is greater than or equal to the expiration time
            bool isExpired = DateTime.UtcNow >= expirationTime;

            return isExpired;
        }


        public static async Task<ServiceOperationResult> Send(Operation operation, Channel channel)
        {

            var operationData = operation.Parameters.FromJson<DailymotionSendOperationData>();
            var opResult = new ServiceOperationResult();
            try
            {
                var title = operationData.Title;
                var text = operationData.Text;
                var tags = operationData.Tags;
                if (text.IsNotNullOrEmpty() && string.IsNullOrEmpty(title))
                {
                    title = text;

                }
                var post = Instances.Repositories.PostRepository.GetPostById(operation.PostId);
                var subscriptionId = operation.SubscriptionId;
                var media = Instances.Repositories.MediaItemRepository.GetMediaByPostId(operation.PostId);
                //var media = !operation.PostId.HasValue && operation.MediaId.HasValue ? Instances.Repositories.MediaRepository.Get(operation.MediaId.Value) : null;
                var postMediaIds = post != null && post.PostMediaItems.NotEmpty() ? post.PostMediaItems.Select(m => m.Id).ToList() : null;

                
                //var postMIds = post.PostMediaItems.Select(m=>m.Id).ToList();
                //var medias = postMediaIds != null ? Instances.Repositories.MediaItemRepository.Get(postMediaIds).ToList() : null;

                //var url = medias.FirstOrDefault(x => x.Type == 30) != null ? medias.Where(x => x.Type == 30).FirstOrDefault().Path : (media != null ? media.Path : null);
                //var mediainfo = medias.Where(x => x.Type == 30).FirstOrDefault() != null ? medias.Where(x => x.Type == 30).FirstOrDefault() : (media != null ? media : null);
                var success = true;
                var mediaSsl = true;
                var postContent = string.IsNullOrEmpty(post.Content) ? "" : Regex.Replace(post.Content, "<.*?>", String.Empty);
                var apiResult = "";
                //var isUrgent = post.PostTerms.Any(m => m._term.Code == "urgent");
                //title = isUrgent ? post.PostTerms.Where(m => m._term.Code == "urgent").Select(m => m._term.Name).FirstOrDefault() + " : " + title : title;
                //apiResult = client.SendMessage(title, postContent, post.CoverImage.PublicImageUrl(mediaSsl), operation.Post.PublicId.ToString(), operation.Post.PublicUrl(true));
                var result = PublishVideo(media.Path, media);
                //var bsObj = JsonConvert.DeserializeObject<UploadedResponse>(result);
                string message = result.Result.id + result.Result.txt;
                string id = result.Result.id;

                if (id != null)
                {
                    var webSocialResult = new SocialMetaResult
                    {
                        Id = id,
                        NetworkId = channel.NetworkId,
                        OperationId = operation.Id
                    };
                    //webSocialResult.AddPostServiceMeta(SocialNetworkTypeEnum.DailyMotion.Key(), operation.PostId.ToString());
                    //webSocialResult.AddMediaServiceMeta(SocialNetworkTypeEnum.DailyMotion.Key(), mediainfo.Id.ToString());
                    //Instances.Repositories.OperationsRepository.UpdateStatus(operation.Id, StatusEnum.Success.Value());
                }
                else
                {
                    success = false;
                }
                opResult.Result = message;
                opResult.Success = success;
                opResult.Message = (message.IsNotNullOrEmpty() ? message : "");
            }
            catch (Exception ex)
            {
                //todo: read exception message from ex.Response
                var error = ex is System.Net.WebException ? new StreamReader(((System.Net.WebException)ex).Response.GetResponseStream()).ReadToEnd() : "";
                opResult.Success = false;
                opResult.Message = ex.FullMessage(error);
            }
            return opResult;
        }

        private static async Task<OAuthResponse> RefreshAccessToken(string refreshToken, string appKey, string appSecret)
        {
            var requestUrl = "https://api.dailymotion.com/oauth/token";
            var requestString = $"grant_type=refresh_token&client_id={appKey}&client_secret={appSecret}&refresh_token={refreshToken}";

            using (var httpClient = new HttpClient())
            {
                var requestData = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(requestUrl, requestData);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<OAuthResponse>(responseString);

                    return jsonResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // Handle the error response
                    throw new Exception("Token refresh failed: " + errorContent);
                }
            }
        }

    }
}
