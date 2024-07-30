using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialWhatsapp.Clients;
using FeedHiveAuth.Areas.Social.SocialWhatsapp.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models.Enums;
using Newtonsoft.Json;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Areas.Social.SocialWhatsapp.Handlers
{
    public class WhatsappService
    {
        private class finalApiResponse
        {
            public string severity { get; set; }
            public string reason { get; set; }
        }
        public static ServiceOperationResult Send(Operation operation, Channel channel)
        {
            var operationData = operation.Parameters.FromJson<WhatsappOperationData>();
            var credentials = channel.Credentials.FromJson<WhatsappCredentials>();
            var client = new WhatsappClient(credentials);
            var opResult = new ServiceOperationResult();
            string imageUrl = null;
            string videoUrl = null;
            string audioUrl = null;
            string documentUrl = null;

            try
            {

                var title = operationData.Title;
                var text = operationData.Text;
                string link = operationData.Link;
                var sendToGroups = operationData.SendToGroups;
                var sendTocontacts = operationData.SendToContacts;
                var setCaption = operationData.SetCaption;
                var useTemplate = operationData.UseTemplate;
                if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(text))
                {
                    opResult.Message = "Title and Text are EMPTY!";
                    return opResult;
                }


                var post = Instances.Repositories.PostRepository.GetPostById(operation.PostId);
                var mediaItem = Instances.Repositories.MediaItemRepository.GetMediaByPostId(post.Id);
                //var mediaItem = operation.MediaItem() ?? (operationData.MediaItemIds.NotEmpty() ? Instances.Repositories.MediaRepository.Get(operationData.MediaItemIds.FirstOrDefault()) : null);
                if (mediaItem != null)
                {
                    var sslEnabled = true;
                    var mediaUrl = mediaItem.FirstOrDefault().Path;
                    switch (EnumExtension.FromValue<MediaTypeEnum>(mediaItem.FirstOrDefault().Type))
                    {
                        case MediaTypeEnum.Image:
                            imageUrl = mediaUrl;
                            break;
                        case MediaTypeEnum.Video:
                            videoUrl = mediaUrl;
                            break;
                        case MediaTypeEnum.Audio:
                            audioUrl = mediaUrl;
                            break;
                        case MediaTypeEnum.File:
                            documentUrl = mediaUrl;
                            break;
                        default:
                            link = operationData.Link.Nval(mediaUrl);
                            break;
                    }
                }

                var success = true;
                var apiResult = "";

                apiResult = client.SendMessage(audioUrl, documentUrl, videoUrl, imageUrl, link.ComposeSafeUrl(true), text, title, null, sendToGroups, sendTocontacts, setCaption, useTemplate);

                var bsObj = JsonConvert.DeserializeObject<finalApiResponse>(apiResult);

                string message = bsObj.reason;
                string status = bsObj.severity;

                if (status == "success")
                {
                    var webSocialResult = new SocialMetaResult
                    {
                        Id = apiResult,
                        NetworkId = channel.NetworkId,
                        OperationId = operation.Id
                    };
                }
                else
                {
                    success = false;
                }

                opResult.Result = status;
                opResult.Success = success;
                opResult.Message = message.IsNotNullOrEmpty() ? message : "";
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
    }
}
