using FeedHiveAuth.Areas.Social.Models;

namespace FeedHiveAuth.Areas.Social.SocialWhatsapp.Models
{
    public class WhatsappOperationData : SendOperationData
    {
        public string NetwrokId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public string PhotoUrl { get; set; }
        public string VideoUrl { get; set; }
        public string AudioUrl { get; set; }
        public string DocumentUrl { get; set; }
        public bool SendToGroups { get; set; }
        public bool SendToContacts { get; set; }
        public bool UseTemplate { get; set; }
        public bool SetCaption { get; set; }
    }
}
