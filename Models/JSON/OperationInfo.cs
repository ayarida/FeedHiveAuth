namespace FeedHiveAuth.Models.JSON
{
    public class OperationInfo
    {
        public string Rule { get; set; }
        public int Relevance { get; set; }
        public string Action { get; set; }
        public bool IgnoreGeneric { get; set; }
    }
}
