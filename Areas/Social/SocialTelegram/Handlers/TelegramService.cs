using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTelegram.Clients;
using FeedHiveAuth.Areas.Social.SocialTelegram.Mdels;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;


namespace FeedHiveAuth.Areas.Social.SocialTelegram.Handlers
{
    public class TelegramService
    {
        public static string GetBotUrl()
        {
            try
            {
/*                return "https://t.me/" + GetBot(subscriptionId).Username;
 *                
*/          return "https://t.me/@octipulse_bot"; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting bot url");
                return null;
            }
        }

        public static Telegram.Bot.Types.User GetBot()
        {
            var configs = SocialServiceHelper.getSocialConfigs().TelegramConfigs;
            var client = new TelegramClient(configs);
            return client.GetCurrentUser();
        }

        public static Channel GetChannelInfo(string username)
        {
            var configs = SocialServiceHelper.getSocialConfigs().TelegramConfigs;
            var client = new TelegramClient(configs, username);
            var result = client.GetCurrentChat();
            if (!result.IsSuccessful)
            {
               Console.WriteLine("Error while getting telegram chat info " + result.Data.ErrorCode + ": " + result.Data.Description);
                return null;
            }

            var chat = result.Data.Result;
            var imageData = GetUserPhoto(configs, client, chat);

            //ToDo: this was changed, it is not fully tested in the new way yet
            //previously:
            //OriginalName = chat.Title,
            //ProfileImageUrl = imageData,
            //NetworkId = chat.Username,
            //Network = SocialNetworkTypeEnum.Telegram.Key(),
            //NetworkUrl = "https://t.me/" + chat.Username,
            //Account = SocialAccountTypeEnum.Page.Key()

            return new Channel()
            {
                OriginalName = chat.Title,
                ProfileImageUrl = imageData,
                NetworkId = chat.Id.ToString(),
                Network = SocialNetworkTypeEnum.Telegram.Key(),
                NetworkUrl = chat.InviteLink,
                Account = SocialAccountTypeEnum.Page.Key()
            };
        }
        private static string GetUserPhoto(TelegramConfigs configs, TelegramClient client, ExtendedChat chat)
        {
            try
            {
                var profilePhoto = chat.Photo != null ? client.GetFile(chat.Photo.BigFileId) : null;
                if (profilePhoto != null && profilePhoto.FilePath.IsNotNullOrEmpty())
                    using (var httpClient = new HttpClient())
                    {
                        var bytes = httpClient.GetByteArrayAsync("https://api.telegram.org/file/bot" + configs.Bot.Token + "/" + profilePhoto.FilePath)?.Result;
                        return "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while downloading telegram bot profile pic " + ex);
            }

            return null;
        }

        #region share
        public static async Task<ServiceOperationResult> Send(Operation operation, Channel channel)
        {
            var opResult = new ServiceOperationResult();
            try
            {
                var operationData = operation.Parameters.FromJson<TelegramOperationData>();
                var configs = SocialServiceHelper.GetConfigs().TelegramConfigs;
                var client = new TelegramClient(configs, channel.NetworkId);
                var text = operationData.Text?.Replace("<br />", "\n");
                if (operationData.Link.IsNotNullOrEmpty() && !text.Contains(operationData.Link))
                {
                    text += "\n" + operationData.Link;
                }

                List<Message> results = new List<Message>();
                var isMultipleMediaPost = operationData.MediaItemIds.NotEmpty() && operationData.MediaItemIds.Count() > 1;
                var isSingleMediaPost = operation.MediaId.Any() || (operationData.MediaItemIds.NotEmpty() && operationData.MediaItemIds.Count().Equals(1));
                var isTextMessage = !isMultipleMediaPost && !isSingleMediaPost;

                if (isTextMessage)
                {
                    var result = await client.SendMessageAsync(text).ConfigureAwait(false);
                    results.Add(result);
                }
               /* else if (isMultipleMediaPost)
                {
                    var streams = new List<TelegramGroupMedia>();
                    //var medias = Instances.Repositories.MediaItemRepository.Get(operationData.MediaItemIds);
                    var medias = List<MediaItem>();
                    foreach (var media in medias)
                    {
                        var mediaType = EnumExtension.FromValue<MediaTypeEnum>(media.Type);
                        var fileStream = mediaType.Equals(MediaTypeEnum.Image) ?
                            media.PublicUrl(false).ComposeSafeUrl(false).AddParameter("width", "1080").GetStreamFromUrl() :
                            media.GetStream(operation);
                        var stream = new TelegramGroupMedia
                        {
                            Stream = fileStream,
                            MediaType = mediaType,
                            Text = text,
                            Thumbnail = media.ThumbnailUrl(false).AddParameter("format", "jpeg").AddParameter("width", "90"),
                            FileName = Path.GetFileName(media.Path)
                        };
                        streams.Add(stream);
                    }
                    var messages = await client.SendMediaGroup(streams).ConfigureAwait(false);
                    results.AddRange(messages);
                }
                else
                {
                    Message result = null;
                    var mediaItem = operation.MediaItem() ?? Instances.Repositories.MediaRepository.Get(operationData.MediaItemIds.FirstOrDefault());
                    if (mediaItem.IsExternal)
                    {
                        text += "\n" + mediaItem.MediaUrl;
                        result = await client.SendMessageAsync(text).ConfigureAwait(false);
                    }
                    else
                    {
                        var mediaType = EnumExtensions.FromValue<MediaTypeEnum>(mediaItem.Type);
                        var fileStream = mediaType.Equals(MediaTypeEnum.Image) ?
                            mediaItem.PublicUrl(false).ComposeSafeUrl(false).AddParameter("width", "1080").GetStreamFromUrl() :
                            mediaItem.GetStream(operation);
                        switch (mediaType)
                        {
                            case MediaTypeEnum.Image:
                                result = await client.SendPhotoAsync(fileStream, text).ConfigureAwait(false);
                                break;
                            case MediaTypeEnum.Video:
                                result = await client.SendVideoAsync(fileStream, text).ConfigureAwait(false);
                                break;
                            case MediaTypeEnum.Audio:
                                result = await client.SendAudioAsync(fileStream, text).ConfigureAwait(false);
                                break;
                            case MediaTypeEnum.File:
                                result = await client.SendFileAsync(fileStream, text).ConfigureAwait(false);
                                break;
                            default:
                                break;
                        }
                    }

                    results.Add(result);
                }
*/
                opResult.Success = results.NotEmpty() && results.Any(result => result?.MessageId != 0);
                opResult.Result = results.Serialize();
                results.ForEach(result =>
                {
                    if (result?.MessageId != 0)
                    {
                        var socialResult = new SocialMetaResult
                        {
                            Id = result.MessageId.ToString(),
                            NetworkId = channel.NetworkId,
                            OperationId = operation.Id
                        };
                        //socialResult.AddPostServiceMeta(SocialNetworkTypeEnum.Telegram.Key(), operation.PostId.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                opResult.Message = ex.FullMessage();
            }
            return opResult;
        }

        private static IEnumerable<object> List<T>()
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
