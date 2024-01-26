using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialDailymotion.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;

namespace FeedHiveAuth.Areas.Social.SocialDailymotion.Clients
{
    public class DailymotionClient
    {
        private readonly string AccessToken;
        protected readonly DailymotionConfigs Configs;

        public DailymotionClient(DailymotionConfigs configs,  string accessToken = null)
        {
            Configs = configs ?? new DailymotionConfigs();
            if (accessToken.IsNotNullOrEmpty())
            {
                AccessToken = accessToken;
                CreateAuthorized(accessToken, configs);
            }
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
        public static void CreateAuthorized(string accessToken, DailymotionConfigs configs)
        {
            var authorizeUrl = String.Format("https://api.dailymotion.com/oauth/authorize?response_type=code&client_id={0}&scope=read+write+manage_videos+delete&redirect_uri={1}",
                HttpUtility.UrlEncode(configs.Application.APIKey),
                HttpUtility.UrlEncode("https://admin-news.mangopulse.net")); //if this function was needed later, the URL should be read as the admin-url config

            var client = new WebClient();
            client.Headers.Add("Authorization", "OAuth " + accessToken);
        }
        public static string GetFileUploadUrl(string accessToken)
        {
            try
            {

                var client = new MyWebClient();
                client.Headers.Add("Authorization", "OAuth " + accessToken);

                var urlResponse = client.DownloadString("https://api.dailymotion.com/file/upload");

                var response = JsonConvert.DeserializeObject<UploadRequestResponse>(urlResponse).upload_url;

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string GetAccessToken(DailymotionConfigs configs)
        {
            var request = WebRequest.Create("https://api.dailymotion.com/oauth/token");
            request.Timeout = 100000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var requestString = String.Format("grant_type=password&client_id={0}&client_secret={1}&username={2}&password={3}",
                HttpUtility.UrlEncode(configs.Application.APIKey),
                HttpUtility.UrlEncode(configs.Application.APISecret),
                HttpUtility.UrlEncode(configs.Application.Username),
                HttpUtility.UrlEncode(configs.Application.Password));
            var requestBytes = System.Text.Encoding.UTF8.GetBytes(requestString);
            var requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            string responseString;
            using (var reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }

            var oauthResponse = JsonConvert.DeserializeObject<OAuthResponse>(responseString);

            return oauthResponse.access_token;
        }
        public static UploadResponse GetFileUploadResponse(string fileToUpload, string accessToken, string uploadUrl)
        {
            var client = new MyWebClient();

            client.Headers.Add("Authorization", "OAuth " + accessToken);
            var responseBytes = client.UploadFile(uploadUrl, fileToUpload);
            var responseString = Encoding.UTF8.GetString(responseBytes);
            var response = JsonConvert.DeserializeObject<UploadResponse>(responseString);

            return response;
        }

        public static UploadedResponse PublishVideo(UploadResponse uploadResponse, string accessToken, MediaItem media)
        {
            var request = WebRequest.Create("https://api.dailymotion.com/me/videos?url=" + HttpUtility.UrlEncode(uploadResponse.url));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Authorization", "OAuth " + accessToken);

            var requestString = String.Format("title={0}&tags={1}&channel={2}&published={3}&is_created_for_kids={4}",
                HttpUtility.UrlEncode(media.Caption),
                HttpUtility.UrlEncode(media.Tags),
                HttpUtility.UrlEncode("news"),
                HttpUtility.UrlEncode("true"), HttpUtility.UrlEncode("true"));
            var requestBytes = Encoding.UTF8.GetBytes(requestString);
            var requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            string responseString;
            using (var reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }
            var uploadedResponse = JsonConvert.DeserializeObject<UploadedResponse>(responseString);
            return uploadedResponse;
        }


    }
}
