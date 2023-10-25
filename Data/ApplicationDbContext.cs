using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FeedHiveAuth.Models.Post>? Post { get; set; }
        public DbSet<FeedHiveAuth.Models.Subscription>? Subscription { get; set; }
        public DbSet<FeedHiveAuth.Models.Channel>? Channel { get; set; }
    }
}