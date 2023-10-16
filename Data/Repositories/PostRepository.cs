using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository() {
            TableName = Database.Tables.Post;
            Columns = Database.Columns.Post;
        }

        public static void SavePost(Post post)
        {

        }

        
    }
}
