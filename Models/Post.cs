namespace FeedHiveAuth.Models
{
    public class Post : BaseModel
    {
        public Post(string subscriptionId) : base(subscriptionId) { }

        public Post() : this(string.Empty) { }
        public string? Title { get; set; }
        public string? ShortTitle { get; set; }
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public DateTime? PostDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? PublishedBy { get; set; }
        public string? PublicLink { get; set; }

        public DateTime? ExpireDate { get; set; }

    }
}
