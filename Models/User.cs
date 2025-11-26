namespace FeedHiveAuth.Models
{
    public class User
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public string? TwoFactorAuthKey { get; set; }
        public string? VerificationToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Language { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? RoleId { get; set; }
        public string? AvatarId { get; set; }
        public string? OrganizationId { get; set; }
        //public MediaItem Avatar { get; set; }
        //public Role RoleProperties { get; set; }
    }
}
