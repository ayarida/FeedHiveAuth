namespace FeedHiveAuth.Data.Extensions
{
    public class GuidExtension
    {
        public static Guid GenerateGuid()
        {
            return OperatingSystem.IsWindows() ? SequentialGuid.Generate() : Guid.NewGuid();
        }
    }
}
