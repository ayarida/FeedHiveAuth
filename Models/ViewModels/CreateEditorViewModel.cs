using System.ComponentModel.DataAnnotations;

namespace FeedHiveAuth.Models.ViewModels
{
    public class CreateEditorViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public string OrganizationId { get; set; }

        public string ParentId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
