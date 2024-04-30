using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Areas.Social.Models
{
    public class EventResult
    {
        public EventResult()
        {
            Exceptions = new List<Exception>();
            Operations = new List<Operation>();
        }
        public ActionResultEnum ActionResult { get; set; }
        public List<Exception> Exceptions { get; set; }
        public IEnumerable<Operation> Operations { get; set; }
    }
}
