using FeedHiveAuth.Areas.Social.Models.Helpers;
using FeedHiveAuth.Areas.Social.SocialDailymotion.Handlers;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using Microsoft.Extensions.Logging;

namespace FeedHiveAuth.Areas.Social.Models.Services
{
    public static class OperationsService
    {
        public static void ProcessOperations(IEnumerable<Operation> operations)
        {
            foreach (var operation in operations)
            {
                ProcessOperation(operation);
            }
        }

        public static void ProcessOperation(Operation operation)
        {
            if(operation == null)
            {
                return;
            }
            try
            {
                switch (operation.Action.ToLower())
                {
                    case "send": 
                        ProcessShareOperation(operation);break;

                    /*case "delete":
                        ProcessDeleteOperation(operation);break;

                    case "update": 
                        ProcessUpdateOperation(operation);break;*/

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to process operation " + operation.Id + "\n" + ex.StackTrace, ex);
            }
        }

        public static void ProcessShareOperation(Operation operation)
        {
            //OperationHelper.StartShareOperation(operation);
            var operationData = operation.Parameters.FromJson<SendOperationData>();
            var channel = Collections.ChannelsOf("1f59028d-15d0-4bf6-a61b-28f33b895310").FirstOrDefault(x => x.Id.Equals("ad23186b-bb6a-11ee-9421-d027889076a7"));
            if(channel == null)
            {
                OperationHelper.EndShareOperation(operation, StatusEnum.Failed, "No channel");
                return;
            }
            var service = EnumExtension.FromKey<SocialNetworkTypeEnum>(operation.Service);
            switch (service)
            {
                case SocialNetworkTypeEnum.Telegram:
                    Task.Run(() =>
                    {
                        var result = TelegramService.Send(operation, channel);
                    });
                    break;
                case SocialNetworkTypeEnum.DailyMotion:
                    Task.Run(() =>
                    {
                        DailymotionService.Send(operation, channel).ContinueWith((result) =>
                        {
                            OperationHelper.EndShareOperation(operation,result.Result.Success ? StatusEnum.Success : StatusEnum.Failed, result.Result.Message, result.Result.Result);
                        });
                    });
                    break;
                default:
                    OperationHelper.EndShareOperation(operation,StatusEnum.Failed, "Channel type doesn't support sharing");
                    break;
            }
        }
    }
}
