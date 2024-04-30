namespace FeedHiveAuth.Models
{
    public class Resources :BaseModel
    {
        public Resources(string subscriptionId) : base(subscriptionId)
        {
        }
        public Resources() : this(string.Empty)
        {
        }
        public string Key { get; set; }
        public string Language { get; set; }
        public string Value { get; set; }
    }
}
