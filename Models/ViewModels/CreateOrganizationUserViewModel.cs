using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FeedHiveAuth.Models.ViewModels
{
    public class CreateOrganizationUserViewModel
    {
        public string OrganizationId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string SelectedRole { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();
    }
}
