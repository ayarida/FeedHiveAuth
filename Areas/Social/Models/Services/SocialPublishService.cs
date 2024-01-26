using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models.Enums;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Areas.Social.SocialTelegram.Mdels;
using FeedHiveAuth.Areas.Social.SocialDailymotion.Models;

namespace FeedHiveAuth.Areas.Social.Models.Services
{
    public static class SocialPublishService
    {
        public static string Send(string postId, string mediaId, ShareFormData data, Channel channel, DateTime creationDate)
        {
            var sendData = GetSendParameters(channel, data);
            if (sendData.MediaItemIds.Empty() && mediaId.HasValue())
                sendData.MediaItemIds = new List<string> { mediaId };

            var operation = new Operation()
            {
                Id = Guid.NewGuid().ToString(),
                Status = StatusEnum.New.Value(),
                CreationDate = creationDate,
                LastModified = creationDate,
                Service = channel.Network,
                Action = "send",
                PostId = postId,
                MediaId = mediaId,
                Parameters = sendData.Serialize()
            };
            //Instances.Repositories.OperationRepository.Insert(operation);
            OperationsService.ProcessOperation(operation);
            return operation.Id;
        }

        private static SendOperationData GetSendParameters(Channel channel, ShareFormData data)
        {
            var type = EnumExtension.FromKey<SocialNetworkTypeEnum>(channel.Network);
            var sendData = new SendOperationData();
            switch (type)
            {
                /*case SocialNetworkTypeEnum.Facebook:
                    sendData = new FacebookOperationData
                    {
                        Text = data.Text,
                        Title = data.Title,
                        Link = data.Link,
                        HTMLSource = data.HTMLSource
                    };
                    break;*/
                case SocialNetworkTypeEnum.Telegram:
                    sendData = new TelegramOperationData
                    {
                        Text = data.Text,
                        Link = data.Link
                    };
                    break;
                case SocialNetworkTypeEnum.DailyMotion:
                    sendData = new DailymotionSendOperationData
                    {
                        Title = data.Title,
                        Text = data.Text,
                    };
                    break;
                default:
                    sendData.Text = data.Text;
                    break;
            }

            sendData.NetworkId = channel.NetworkId;
            sendData.MediaItemIds = data.MediaItemIds;
            sendData.ScheduleTime = data.ScheduleTime;
            return sendData;
        }

    }
}
