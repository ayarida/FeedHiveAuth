namespace FeedHiveAuth.Models.ViewModels
{
    public class UserDataViewModel
    {
        public List<Post> userPosts {  get; set; }

        public List<Post> allPosts { get; set; }

        public int userPostsCount { get; set; }
    }
}
