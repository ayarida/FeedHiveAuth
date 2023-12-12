using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data
{
    public static class Database
    {
        public static class Tables
        {
            public static string Subscription = "Subscription";
            public static string Role = "Role";
            public static string Operation = "Operation";
            public static string Post = "Post";
            public static string MediaItem = "MediaItem";
            public static string Channel = "Channel";
            public static string Users = "AspNetUsers";
            public static string Roles = "AspNetRoles";
            public static string RoleClaims = "AspNetRoleClaims";
            public static string UserTokens = "AspNetUserTokens";
            public static string UserRoles = "AspNetUserRoles";
            public static string UserLogins = "AspNetUserLogins";
        }
        public static class Columns
        {
            public static string Subscription = "Id,PublicId,Name,Code,Status,Master,CreationDate,ParentId,Hosts";

            public static string Users =
                "Id,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,SubscriptionId";

            public static string Roles = "Id,Name,NormalizedName,ConcurrencyStamp";

            public static string Operation =
                "Id,Status,CurrentState,Service,Action,Callback,PostId,MediaId,CreationDate,StartTime,EndTime,Progress,Parameters,Result,Messages,Info,PostId";

            /*            public static string Post = "Id,Title,ShortTitle,Summary,Content,Status,PostDate,CreationDate,LastModified,LastVisited,CreatedBy,ModifiedBy,PublishedBy,PublicLink,ExpireDate,SubscriptionId";
            */
            /*            public static string MediaItem = "Id,Caption,Description,Tags,Path,Extension,Type,ThumbnailUrl,CreationDate,Version,CreatedBy,Duration,Info,PostId";
            */
            public static string Post = "Id,Title,ShortTitle,Summary,Content,PublicLink,PostDate";
            public static string MediaItem = "Id,Caption,PostId,CreationDate, Path, CreatedBy";
            public static string Channel = "Id,Name,Description,Status,CreationDate,LastModified,Network,Account,NetworkId,Code,NetworkUrl,OriginalName,ProfileImageUrl,Credentials,SubscriptionId,Settings,Order,Configs";

        }
    }
}