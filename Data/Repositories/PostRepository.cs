using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository()
        {
            TableName = Database.Tables.Post;
            Columns = Database.Columns.Post;
        }


        public Post Publish(string id)
        {
            //change status 
            //save publishedBy 
            //check permission before 
            var post = Get(id);
            var published = StatusEnum.Published.Value();
            if (post.Status == published)
            {
                return null;
            }
            int status = -1;
            var updateStatus = false;
            post.Status = published;
            UpdateColumn("Status", post.Status, post.Id);
            //check postdate on publish
            if (post.PostDate == null)
            {
                post.PostDate = DomainTime.Now();
                UpdateColumn("PostDate", post.PostDate, post.Id);
            }
            //add PublishedBy current user id
            return post;
        }


        

        /*public List<Post> PublishedPosts()
        {

        }*/

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
