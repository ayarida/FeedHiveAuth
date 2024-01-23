namespace FeedHiveAuth.Areas.Social.SocialWhatsapp.Models
{
    public class WhatsappViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public string Document { get; set; }
        public string Youtube { get; set; }
        public bool SendToGroups { get; set; }
        public bool SendToContacts { get; set; }
        public bool UseTemplate { get; set; }
        public bool SetCaption { get; set; }
    }
}
