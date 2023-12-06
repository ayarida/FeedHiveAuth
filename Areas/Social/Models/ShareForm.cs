namespace FeedHiveAuth.Areas.Social.Models
{
    public class ShareForm
    {
        public string? PostId {  get; set; }
        public IEnumerable<string> Channels { get; set; }
        public string? MediaId {  get; set; }
        public IEnumerable<ShareFormData> Data { get; set; }
    }

    public class ShareFormData
    {
        public string ChannelId { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public IEnumerable<Guid> MediaItemIds { get; set; }
        public DateTime? ScheduleTime { get; set; }

        //for push only
        public string Title { get; set; }
        public string Topics { get; set; }
        public bool ForMobile { get; set; }
        public bool ForWeb { get; set; }

        //for youtube
        public string Privacy { get; set; }
        public string Category { get; set; }
        public string Playlist { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Labels { get; set; }

        //for whatsapp only
        public bool SendToContacts { get; set; }
        public bool SendToGroups { get; set; }
        public bool UseTemplate { get; set; }
        public bool SetCaption { get; set; }

        //for instant article 
        public string HTMLSource { get; set; }

        //todo: add all other possible entries
    }
}
