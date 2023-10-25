namespace FeedHiveAuth.Models
{
    public class Subscription
    {
        public Subscription() { }

        public string Id {  get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? Hosts { get; set; }
        public bool Master { get; set; }
        //public MediaItem Logo { get; set; }
        public string? ParentId { get; set; }
    }
}
