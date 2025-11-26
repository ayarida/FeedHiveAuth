using FeedHiveAuth.Areas.Social.Models.Helpers;
using FeedHiveAuth.Areas.Social.SocialDailymotion.Handlers;
using FeedHiveAuth.Areas.Social.SocialFacebook.Handlers;
using FeedHiveAuth.Areas.Social.SocialTwitter.Handlers;
using FeedHiveAuth.Data;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;
using Newtonsoft.Json;
using System.Text;

namespace FeedHiveAuth.Areas.Social.Models.Services
{
    public static class OperationsService
    {
        public static MediaItemRepository _mediaItemService = Instances.Repositories.MediaItemRepository;

        public static HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };

        public static HttpClient client = new HttpClient(handler);
        public static void ProcessOperations(IEnumerable<Operation> operations)
        {
            foreach (var operation in operations)
            {
                ProcessOperation(operation);
            }
        }

        public static void ProcessOperation(Operation operation)
        {
            if (operation == null)
            {
                return;
            }
            try
            {
                switch (operation.Action.ToLower())
                {
                    case "send":
                        ProcessShareOperation(operation); break;

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

        public static async void ProcessShareOperation(Operation operation)
        {
            OperationHelper.StartShareOperation(operation);
            var operationData = operation.Parameters.FromJson<SendOperationData>();
            operationData.mediaLink = _mediaItemService.GetPath(operation.MediaId);
            
            var channel = Collections.Channels().FirstOrDefault(x => x.Id.Equals(operation.ChannelId));
            if (channel == null)
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
                        OperationHelper.EndShareOperation(operation, result.Result.Success ? StatusEnum.Success : StatusEnum.Failed, result.Result.Message, result.Result.Result);

                    });
                    break;
                case SocialNetworkTypeEnum.DailyMotion:
                    Task.Run(() =>
                    {
                        DailymotionService.Send(operation, channel).ContinueWith((result) =>
                        {
                            OperationHelper.EndShareOperation(operation, result.Result.Success ? StatusEnum.Success : StatusEnum.Failed, result.Result.Message, result.Result.Result);
                        });
                    });
                    break;
                case SocialNetworkTypeEnum.Facebook:

                        using (client)
                        {
                            var requestData = new
                            {
                                operationData = operationData,
                                channel = channel, 
                                
                            };
                            var jsonContent = JsonConvert.SerializeObject(requestData);
                            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                            var response = await client.PostAsync("https://localhost:44352/Home/SendFacebook", content);
                            
                            // Read the response from the second app
                            var responseContent = response.Content.ReadAsStringAsync();
                            OperationHelper.EndShareOperation(operation, response.IsSuccessStatusCode ? StatusEnum.Success : StatusEnum.Failed, response.Content.ReadAsStringAsync().ToString());
                            

                        }
                        /*var result = FacebookService.Send(operation, channel);
                        OperationHelper.EndShareOperation(operation,result.Success ? StatusEnum.Success : StatusEnum.Failed, result.Message, result.Result);*/
                    break;
                case SocialNetworkTypeEnum.Twitter:
                    using (client)
                    {
                        var requestData = new
                        {
                            operationData = operationData,
                            channel = channel,
                            credentials = channel.Credentials
                        };
                        var jsonContent = JsonConvert.SerializeObject(requestData);
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("https://localhost:44352/Home/SendTwitter", content);
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the response from the second app
                            var responseContent = response.Content.ReadAsStringAsync();
                            Console.WriteLine("Response from server: " + responseContent);
                        }

                    }
                    /*Task.Run(() =>
                    {
                        var result = TwitterService.Send(operation, channel);
                        OperationHelper.EndShareOperation(operation, result.Success ? StatusEnum.Success : StatusEnum.Failed, result.Message, result.Result);
                    });*/
                    break;
                default:
                    OperationHelper.EndShareOperation(operation, StatusEnum.Failed, "Channel type doesn't support sharing");
                    break;
            }
        }
    }
}
