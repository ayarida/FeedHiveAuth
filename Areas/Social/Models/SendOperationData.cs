using System;
using System.Collections.Generic;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class SendOperationData
    {
        public string NetworkId { get; set; }
        public string Text { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public IEnumerable<Guid> MediaItemIds { get; set; }
    }
}
