using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models.Enums;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using File = Telegram.Bot.Types.File;
using FeedHiveAuth.Areas.Social.SocialFacebook;
using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Areas.Social.SocialTelegram.Mdels;

namespace FeedHiveAuth.Areas.Social.SocialTelegram.Clients
{
    public class TelegramClient : RestClientService
    {
        private readonly TelegramConfigs Configs;
        private readonly TelegramBotClient Bot;
        private readonly string ChannelUsername;
        private const ParseMode parseMode = ParseMode.Html;

        public TelegramClient(TelegramConfigs configs, string username = null) : base("https://api.telegram.org")
        {
            Configs = configs;
            Bot = new TelegramBotClient(configs.Bot.Token);
            if (username.IsNotNullOrEmpty())
            {
                ChannelUsername = username.StartsWith("@") || username.StartsWith("-") ? username : "@" + username;
                //ChannelUsername = username;
            }
        }
        public async Task<Message> SendMessageAsync(string text)
        {
            return await Bot.SendTextMessageAsync(chatId: ChannelUsername,
                                                  text: text,
                                                  parseMode: parseMode).ConfigureAwait(false);
        }

        public async Task<Message> SendPhotoAsync(Stream photoStream, string text)
        {
            var inputFile = new InputOnlineFile(photoStream);
            return await Bot.SendPhotoAsync(chatId: ChannelUsername,
                                            photo: inputFile,
                                            caption: text,
                                            parseMode: parseMode).ConfigureAwait(false);
        }

        public async Task<Message> SendVideoAsync(Stream videoStream, string text)
        {
            var inputFile = new InputOnlineFile(videoStream);
            return await Bot.SendVideoAsync(chatId: ChannelUsername,
                                    video: inputFile,
                                    caption: text,
                                    parseMode: parseMode).ConfigureAwait(false);
        }

        public async Task<Message> SendAudioAsync(Stream audioStream, string text)
        {
            var inputFile = new InputOnlineFile(audioStream);
            return await Bot.SendAudioAsync(chatId: ChannelUsername,
                                            audio: inputFile,
                                            caption: text,
                                            parseMode: parseMode).ConfigureAwait(false);
        }

        public async Task<Message> SendFileAsync(Stream fileStream, string text)
        {
            var inputFile = new InputOnlineFile(fileStream);
            return await Bot.SendDocumentAsync(chatId: ChannelUsername,
                                            document: inputFile,
                                            caption: text,
                                            parseMode: parseMode).ConfigureAwait(false);
        }

        public async Task<Message[]> SendMediaGroup(IEnumerable<TelegramGroupMedia> streams)
        {
            var media = new List<IAlbumInputMedia>();
            foreach (var stream in streams)
            {
                var inputMedia = new InputMedia(stream.Stream, stream.FileName);
                IAlbumInputMedia albumInputMedia;
                switch (stream.MediaType)
                {
                    case MediaTypeEnum.Image:
                        albumInputMedia = new InputMediaPhoto(inputMedia)
                        {
                            Caption = stream.Text,
                            ParseMode = parseMode
                        };
                        media.Add(albumInputMedia);
                        break;
                    case MediaTypeEnum.Audio:
                        albumInputMedia = new InputMediaAudio(inputMedia)
                        {
                            Caption = stream.Text,
                            Thumb = new InputMedia(stream.Thumbnail),
                            ParseMode = parseMode
                        };
                        media.Add(albumInputMedia);
                        break;
                    case MediaTypeEnum.Video:
                        albumInputMedia = new InputMediaVideo(inputMedia)
                        {
                            Caption = stream.Text,
                            Thumb = new InputMedia(stream.Thumbnail),
                            ParseMode = parseMode
                        };
                        media.Add(albumInputMedia);
                        break;
                    case MediaTypeEnum.File:
                        albumInputMedia = new InputMediaDocument(inputMedia)
                        {
                            Caption = stream.Text,
                            Thumb = new InputMedia(stream.Thumbnail),
                            ParseMode = parseMode
                        };
                        media.Add(albumInputMedia);
                        break;
                    default:
                        break;
                }
            }
            return await Bot.SendMediaGroupAsync(chatId: ChannelUsername,
                                                 media: media);
        }


        public IRestResponse<TelegramWebResponse<ExtendedChat>> GetCurrentChat()
        {
            var parms = new Dictionary<string, object>
            {
                { "chat_id", ChannelUsername }
            };

            return Get<TelegramWebResponse<ExtendedChat>>(client.BaseUrl.OriginalString + "/bot" + Configs.Bot.Token + "/getChat", query: parms);
        }

        public User GetCurrentUser()
        {
            return Bot.GetMeAsync().Result;
        }

        public File GetFile(string fileId)
        {
            return Bot.GetFileAsync(fileId).Result;
        }

        public IRestResponse<TelegramWebResponse<bool>> DeleteMessage(string messageId)
        {
            var parms = new Dictionary<string, object>
            {
                { "chat_id", ChannelUsername },
                { "message_id", messageId }
            };

            return Post<TelegramWebResponse<bool>>(client.BaseUrl.OriginalString + "/bot" + Configs.Bot.Token + "/deleteMessage", parms: parms);
        }

        public async Task<Message> EditMessageText(string messageId, string text, bool mediaCaption = false)
        {
            if (mediaCaption)
            {
                return await Bot.EditMessageCaptionAsync(ChannelUsername, Convert.ToInt32(messageId), text).ConfigureAwait(false);
            }

            return await Bot.EditMessageTextAsync(ChannelUsername, Convert.ToInt32(messageId), text).ConfigureAwait(false);
        }


    }
}
