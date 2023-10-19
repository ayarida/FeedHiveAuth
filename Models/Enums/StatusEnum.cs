namespace FeedHiveAuth.Models.Enums
{
    public enum StatusEnum
    {
        New = 10,
        Published = 20,
        Unpublished = 30,
        Processing = 40,
        Waiting = 45,
        Processed = 50,
        ProcessedFailed = 55,
        Success = 60,
        Failed = 70,
        Expired = -150,
        Scheduled = 150,
        Deleted = -100,
        Blocked = -200
    }
}
