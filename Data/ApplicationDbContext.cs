using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FeedHiveAuth.Models;
using Microsoft.AspNetCore.Identity;

namespace FeedHiveAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.ParentId)
                    .HasColumnName("ParentId")
                    .HasMaxLength(128); // Adjust as necessary

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleId")
                    .HasMaxLength(128);

                entity.Property(e => e.Status)
                    .HasColumnName("Status");
            });
        }
        public DbSet<FeedHiveAuth.Models.Post>? Post { get; set; }
        public DbSet<FeedHiveAuth.Models.Subscription>? Subscription { get; set; }
        public DbSet<FeedHiveAuth.Models.Channel>? Channel { get; set; }
        public DbSet<FeedHiveAuth.Models.MediaItem>? MediaItem { get; set; }
    }
}