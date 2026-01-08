namespace FeedHiveAuth.Models
{
    public class Organization : BaseModel
    {
        public Organization() { }
        public Organization(string subscriptionId) : base(subscriptionId)
        {
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string SubscriptionType { get; set; } // Basic, Pro, Enterprise, etc.
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public ICollection<User> Users { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string? DeletedBy { get; set; }

    }
}
