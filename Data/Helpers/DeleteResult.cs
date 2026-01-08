namespace FeedHiveAuth.Data.Helpers
{
    public class DeleteResult
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }

        public static DeleteResult Success()
            => new DeleteResult { IsSuccess = true };

        public static DeleteResult Blocked(string message)
            => new DeleteResult { IsSuccess = false, Message = message };
    }

}
