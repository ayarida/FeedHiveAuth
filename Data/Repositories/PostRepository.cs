using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository()
        {
            TableName = Database.Tables.Post;
            Columns = Database.Columns.Post;
        }

        public void Save(Post post)
        {
            if(post.Id == null)
            {
                SavePost(post);
            }
            else
            {
                //Update(post);
            }
            
        }

        public void SavePost(Post post)
        {
            post.Id = Guid.NewGuid().ToString();
            string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},{3},{4},{5},{6},{7})",
                                    TableName,
                                    Columns.AddBraces(),
                                    post.Id.EscapeForSql(),
                                    post.Title.EscapeForSql(),
                                    post.ShortTitle.EscapeForSql(),
                                    post.Summary.EscapeForSql(),
                                    post.Content.EscapeForSql(),
                                    post.PublicLink.EscapeForSql()
                                    );
            ExecuteQuery(query);
        }
    }
}
