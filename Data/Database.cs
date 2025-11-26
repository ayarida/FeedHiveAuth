namespace FeedHiveAuth.Data
{
    public static class Database
    {
        public static class Tables
        {
            public static string Role = "Role";
            public static string Operation = "Operation";
            public static string Post = "Post";
            public static string MediaItem = "MediaItem";
            public static string Channel = "Channel";
            public static string Users = "AspNetUsers";
            public static string Roles = "AspNetRoles";
            public static string RoleClaims = "AspNetRoleClaims";
            public static string Subscription = "Subscription";

            public static string UserTokens = "AspNetUserTokens";
            public static string UserRoles = "AspNetUserRoles";
            public static string UserLogins = "AspNetUserLogins";
            public static string Resources = "Resources";
            public static string PostMedias = "PostMedias";
            public static string Configs = "Configs";

            public static string Organization = "Organization";
        }
        public static class Columns
        {
            public static string Users =
                "Id,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,SubscriptionId,OrganizationId";

            public static string Roles = "Id,Name,NormalizedName,ConcurrencyStamp";

            public static string Operation =
                "Id,Status,CurrentState,Service,Action,Callback,PostId,MediaId,CreationDate,StartTime,EndTime,Progress,Parameters,Result,Messages,Info";
            public static string Post = "Id,Title,ShortTitle,Summary,Content,PublicLink,PostDate,CreatedBy,ModifiedBy,Status,OrganizationId";
            public static string MediaItem = "Id,Caption,PostId,CreationDate, Path, CreatedBy,Extension,Type";
            public static string Channel = "Id,Name,Description,Status,CreationDate,LastModified,Network,Account,NetworkId,Code,NetworkUrl,OriginalName,ProfileImageUrl,Credentials,Settings,Order,Configs,ParentId";
            public static string Resources = "Id,Key,Value,Language";
            public static string PostMedias = "MediaItemId,PostId";
            public static string Configs = "Id,EnumKey,Name,JsonValue,MasterId,CreationDate";
            public static string Organization = "Id,Name,SubscriptionType,IsActive";
        }
        public static class ExcludedColumns
        {
            public static string Post = "PublicId,Status";
            public static string Configs = "";

        }
    }
}