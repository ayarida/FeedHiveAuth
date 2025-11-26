using System;
using System.Collections.Generic;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class SendOperationData
    {
        public string NetworkId { get; set; }

        public string postSummary { get; set; }
        public string postContent { get; set; }
        public string Text { get; set; } // Title
        public DateTime? ScheduleTime { get; set; }
        public IEnumerable<string> MediaItemIds { get; set; }
        public string mediaLink { get; set; }
    }
}
