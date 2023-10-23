using System.ComponentModel.DataAnnotations.Schema;

namespace FeedHiveAuth.Models
{
    public class MediaItem : BaseModel
    {
        public MediaItem(string subscriptionId) : base(subscriptionId)
        {
           
        }

        public MediaItem() : this(string.Empty)
        {
        }
        //public string? Id { get; set; }
        public string?    Caption { get; set; }
        public string? Description { get; set; }

        public int? Duration { get; set; }
        public string? Tags { get; set; }
        public string? Path { get; set; }
        public string? Extension { get; set; }
        public int Type { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Version { get; set; }
        public string? CreatedBy { get; set; }
        public string? Info { get; set; }

        public string? PostId {  get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        [NotMapped]

        public List<IFormFile> Files { get; set; }
    }
}
