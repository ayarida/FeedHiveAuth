namespace FeedHiveAuth.Models
{
    public class Subscription : BaseModel
    {
        public Subscription() : base(string.Empty) { }

        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Hosts { get; set; }
        public bool Master { get; set; }
        //public MediaItem Logo { get; set; }
        public string? ParentId { get; set; }
    }
}
