using System.ComponentModel.DataAnnotations;

namespace FeedHiveAuth.Models.ViewModels
{
    public class OrganizationCreateViewModel
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Subscription Type")]
        public string SubscriptionType { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Description { get; set; }
    }
}
