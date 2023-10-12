namespace FeedHiveAuth.Models
{
    public class BaseModel
    {
        public BaseModel(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public string? Id { get; set; }
        public int Status { get; set; }
        public DateTime? CreationDate { get; set; } = DateTime.MinValue;
        public DateTime? LastModified { get; set; }
        public string? SubscriptionId { get; set; }
    }
}
