using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.Models.Helpers
{
    public static class OperationHelper
    {
        public static void StartShareOperation(Operation operation)
        {
            operation.StartOperation("The post is being shared to " + operation.Service);
        }

        public static void EndShareOperation(Operation operation, StatusEnum status, string message = null, string result = null)
        {
            operation.EndOperation(status, message, result, "The post share operation to " + operation.Service + " has ended");
        }
        private static void StartOperation(this Operation operation, string currentState)
        {
            operation.Status = StatusEnum.Processing.Value();
            operation.CurrentState = currentState;
            operation.Progress = 0;
            operation.StartTime = DomainTime.Now();
            operation.EndTime = null;
            operation.Messages += "************************************************\r\n" + DomainTime.Now() + ": " + "The operation has started\r\n";
            Instances.Repositories.OperationRepository.Update(operation);
        }

        private static void EndOperation(this Operation operation, StatusEnum status, string message = null, string result = null, string currentState = null)
        {
            operation.Status = status.Value();
            operation.CurrentState = currentState;
            operation.EndTime = DomainTime.Now();
            if (result.IsNotNullOrEmpty())
            { //operation.Result = operation.AddResult(result); 
            }
            if (message.IsNotNullOrEmpty())
            { operation.Messages += DomainTime.Now() + ": " + message + "\r\n"; }
            operation.Messages += DomainTime.Now() + ": " + "The operation has ended\r\n************************************************\r\n";
            Instances.Repositories.OperationRepository.Update(operation);
            if (operation.Callback.IsNotNullOrEmpty())
            {
                //operation.Callback(status);
            }
        }
    }
}
