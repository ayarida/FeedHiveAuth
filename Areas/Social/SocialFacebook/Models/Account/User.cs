using Newtonsoft.Json;

namespace FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        //public Location Address { get; set; }
        //public IEnumerable<PageAdminNotes> AdminNotes { get; set; }
        //public AgeRange AgeRange { get; set; }

        [JsonProperty("auth_method")]
        public string AuthMethod { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [JsonProperty("Can_review_measurement_request")]
        public bool CanReviewMeasurementRequest { get; set; }
        //public IEnumerable<EducationExperience> Education { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        //public IEnumerable<Experience> FavoriteAthletes { get; set; }
        //public IEnumerable<Experience> FavoriteTeams { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }
        //public Page Hometown { get; set; }
        //public IEnumerable<Experience> InspirationalPeople { get; set; }

        [JsonProperty("install_type")]
        public string InstallType { get; set; }

        [JsonProperty("installed")]
        public bool Installed { get; set; }

        [JsonProperty("interested_in")]
        public IEnumerable<string> InterestedIn { get; set; }

        [JsonProperty("is_famedeeplinkinguser")]
        public bool IsFamedeeplinkinguser { get; set; }

        [JsonProperty("is_shared_login")]
        public bool IsSharedLogin { get; set; }
        //public IEnumerable<PageLabel> Labels { get; set; }
        //public IEnumerable<Experience> Languages { get; set; }        

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
        //public Page Location { get; set; }

        [JsonProperty("meeting_for")]
        public IEnumerable<string> MeetingFor { get; set; }

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_format")]
        public string NameFormat { get; set; }
        //public PaymentPricepoints PaymentPricepoints { get; set; }

        [JsonProperty("political")]
        public string Political { get; set; }

        [JsonProperty("profile_pic")]
        public string ProfilePic { get; set; }

        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        [JsonProperty("quotes")]
        public string Quotes { get; set; }

        [JsonProperty("relationship_status")]
        public string RelationshipStatus { get; set; }

        [JsonProperty("religion")]
        public string Religion { get; set; }
        //public SecuritySettings SecuritySettings { get; set; }

        [JsonProperty("shared_login_upgrade_required_by")]
        public DateTime SharedLoginUpgradeRequiredBy { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("significant_other")]
        public User SignificantOther { get; set; }
        //public IEnumerable<Experience> Sports { get; set; }

        [JsonProperty("test_group")]
        public long TestGroup { get; set; }

        [JsonProperty("token_for_business")]
        public string TokenForBusiness { get; set; }
        //public VideoUploadLimits VideoUploadLimits { get; set; }

        [JsonProperty("viewer_can_send_gift")]
        public bool ViewerCanSendGift { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }
    }
}
