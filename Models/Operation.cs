using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models.JSON;

namespace FeedHiveAuth.Models
{
    public class Operation : BaseModel
    {
        public Operation(string subscriptionId) : base(subscriptionId)
        {
            Messages = "";
        }

        public Operation() : this(string.Empty)
        {
        }
        public string? Service { get; set; }
        public string? Action { get; set; }
        public string? Callback { get; set; }
        public string? PostId { get; set; }
        public string? MediaId { get; set; }
        //public Post Post { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? CurrentState { get; set; }
        public decimal? Progress { get; set; }
        public string? Parameters { get; set; }
        public string? Result { get; set; }
        public string? Messages { get; set; }
        public string? Info { get; set; }

        public Post _post { get; set; }
        public MediaItem _mediaItem { get; set; }

        private OperationInfo _info;

        public OperationInfo OperationInfo
        {
            get { return _info ?? (_info = Info.FromJson<OperationInfo>()); }
        }
    }
}
