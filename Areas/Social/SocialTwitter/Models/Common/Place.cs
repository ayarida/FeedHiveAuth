namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common
{
    public class Place
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string PlaceType { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public BoundingBox BoundingBox { get; set; }
        //public Attributes Attributes { get; set; }
    }

    public class BoundingBox
    {
        public IEnumerable<IEnumerable<IEnumerable<float>>> Coordinates { get; set; }
        public string Type { get; set; }
    }
}
