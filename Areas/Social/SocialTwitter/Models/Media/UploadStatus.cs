namespace FeedHiveAuth.Areas.Social.SocialTwitter.Models.Media
{
    public class UploadStatus : TwitterWebResponse
    {
        public ulong MediaId { get; set; }
        public string MediaIdString { get; set; }
        public long ExpiresAfterSecs { get; set; }
        public ProcessingInfo ProcessingInfo { get; set; }
    }

    public class ProcessingInfo
    {
        public string State { get; set; }
        public int ProgressPercent { get; set; }
        public int? CheckAfterSecs { get; set; }
        public ProcessingError Error { get; set; }
    }

    public class ProcessingError
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
