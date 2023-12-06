using System.Net;

namespace FeedHiveAuth.Data.Extensions
{
    public static class MediaExtension
    {
        public static Stream GetStream(string url)
        {
            if(url == null) { return null; }
            return new FileStream(url, FileMode.Open, FileAccess.Read);
        }
        public static Stream GetStreamFromUrl(this string url)
        {
            byte[] imageData = null;

            using (var wc = new WebClient())
                imageData = wc.DownloadData(url.ComposeSafeUrl(url.StartsWith("https:")));

            return new MemoryStream(imageData);
        }
    }
}
