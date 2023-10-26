using Dapper;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;

namespace FeedHiveAuth.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository()
        {
            TableName = Database.Tables.Post;
            Columns = Database.Columns.Post;
        }


        public int Publish(string postId , string userId=null)
        {
            //change status 
            //save publishedBy 
            //check permission before
            var post = Get(postId);
            int comp = DateTime.Compare(post.PostDate.Value, DomainTime.Now());
            var published = StatusEnum.Published.Value();
            if (post.Status == published)
            {
                return PostErrorEnum.POST_ALREADY_PUBLISHED.Value();
            }
            //check postdate on publish
            if (post.PostDate == null)
            {
                post.PostDate = DomainTime.Now();
                //UpdateColumn("PostDate", post.PostDate, post.Id);
            }
            else
            {
                //SCHEDULED
                if(post.PostDate.Value > DomainTime.Now())
                {
                    post.Status = StatusEnum.Scheduled.Value();
                }
                else
                    post.Status = published;
            }
            UpdateColumn("Status", post.Status, post.Id);
            //get current user and update PublishedBy to current user id
            UpdateColumn("PublishedBy",userId, post.Id);
            return 0;
        }

        public List<Post> GetPublishedPosts()
        {
            List<Post> posts  = new List<Post>();
            var query = SqlSelect + " WHERE STATUS = 20";
            posts = connection.Query<Post>(query).ToList();
            return posts;
        }

        public List<Post> GetPosts()
        {
            List<Post> posts = new List<Post>();
            var query = SqlSelect;
            posts = _connection.globalSqlConnection.Query<Post>(query).ToList();
            return posts;
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
                                    "Insert Into {0} ({1}) Values ({2},{3},{4},{5},{6},{7},{8})",
                                    TableName,
                                    Columns.AddBraces(),
                                    post.Id.EscapeForSql(),
                                    post.Title.EscapeForSql(),
                                    post.ShortTitle.EscapeForSql(),
                                    post.Summary.EscapeForSql(),
                                    post.Content.EscapeForSql(),
                                    post.PublicLink.EscapeForSql(),
                                    post.PostDate.EscapeForSql(true)
                                    );
            ExecuteQuery(query);
        }
    }
}
