namespace FeedHiveAuth.Models
{
    public class MediaItem
    {
        public MediaItem() { }
        public string? Id { get; set; }
        public string?    Caption { get; set; }
        public string? Tags { get; set; }
        public string? Path { get; set; }
        public string? Extension { get; set; }
        public int Type { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Version { get; set; }
        public string? CreatedBy { get; set; }
        public string? Info { get; set; }

        public string? PostId {  get; set; }
        //public Post Post { get; set; }
        public IFormFile File { get; set; }


    }
}
