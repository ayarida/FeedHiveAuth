using System.ComponentModel.DataAnnotations;

namespace FeedHiveAuth.Models.ViewModels
{
    public class CreateOrgAdminViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Display(Name = "Organization")]
        public string OrganizationId { get; set; }

        public bool isActive { get; set; } = true;
    }
}
