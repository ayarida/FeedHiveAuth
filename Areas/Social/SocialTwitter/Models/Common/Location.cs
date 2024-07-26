namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Common
{
    public class Location
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string FullName { get; set; }
        public Geometry Geo { get; set; }
    }

    public class Geometry
    {
        public IEnumerable<float> Coordinates { get; set; }
        public string Type { get; set; }
    }
}
