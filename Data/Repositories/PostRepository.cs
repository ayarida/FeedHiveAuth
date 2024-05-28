using Dapper;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FeedHiveAuth.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository()
        {
            TableName = Database.Tables.Post;
            Columns = Database.Columns.Post;
            ExcludedColumns = Database.ExcludedColumns.Post;
        }

        public int Publish(string postId, string userId = null)
        {
            //change status 
            //save publishedBy 
            //check permission before           
            var post = Get(postId);
            //If it has no postdate so by default current and give new status
            if (post.PostDate == null)
            {
                post.PostDate = DateTime.Now;
                post.Status = StatusEnum.Published.Value();
                UpdateColumn("PostDate", post.PostDate, post.Id);
                //UpdateColumn("PostDate", post.PostDate, post.Id);
            }
            else
            {
                DateTime currentDateTime = DateTime.Now;
                int comp = DateTime.Compare(post.PostDate.Value, currentDateTime);
                if (comp > 0)
                {
                    post.Status = StatusEnum.Scheduled.Value();
                }
                else
                {
                    post.Status = StatusEnum.Published.Value();
                }
            }
            /*            if (post.Status == published)
                        {
                            return PostErrorEnum.POST_ALREADY_PUBLISHED.Value();
                        }
                        */
            UpdateColumn("Status", post.Status, post.Id);
            UpdateColumn("PublishedBy", userId, post.Id);
            return 0;
        }

        public List<Post> GetPublishedPosts()
        {
            List<Post> posts = new List<Post>();
            var query = SqlSelect + " WHERE STATUS = 20";
            posts = connection.Query<Post>(query).ToList();
            return posts;
        }

        public List<Post> GetPosts()
        {
            var query = SqlSelectWhole;
            List<Post> posts = connection.Query<Post>(query).ToList();
            return posts;
        }
        public List<Post> GetAdminUsersPosts(List<string> adminUsersIds)
        {
            var paramNames = adminUsersIds.Select((id, index) => $"@userId{index}");
            var query = SqlSelectWhole + $" WHERE CreatedBy IN ({string.Join(",", paramNames)})";
            //var query = SqlSelectWhole + $" WHERE CreatedBy IN (" + adminUsersIds + ")";
            var parameters = new DynamicParameters();
            for (var i = 0; i < adminUsersIds.Count; i++)
            {
                parameters.Add($"userId{i}", adminUsersIds[i]);
            }

            // Execute the query with parameters
            List<Post> posts = connection.Query<Post>(query, parameters).ToList();

            //List<Post> posts = connection.Query<Post>(query).ToList();
            return posts;
        }

        public List<Post> GetScheduledPosts()
        {
            var query = SqlSelect + $" WHERE Status = 150";
            List<Post> scheduledPosts = connection.Query<Post>(query).ToList();

            return scheduledPosts;
        }

        public Post GetPostById(string id)
        {
            var query = SqlSelect + $" WHERE ID = '{id}'";
            Post posts = connection.Query<Post>(query).ToList().FirstOrDefault();
            //posts = SqlUpdate + $"WHERE ID = '{id}'";
            return posts;
        }

        public int Update(Post post)
        {
            try
            {
                connection.Query<Post>(SqlUpdate, post);
            }catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return 0;
            }
            return 1;
        }


        public List<Post> GetPostsByIds(List<string> idsArray)
        {
            var posts = Query<Post>(SqlSelect + " WHERE Id in @Ids", new { Ids = idsArray }).ToList();

            return posts;
        }
        public void Save(Post post)
        {
            if (post.Id == null)
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
                                    "Insert Into {0} ({1}) Values ({2},N{3},N{4},N{5},N{6},N{7},{8},{9},{10})",
                                    TableName,
                                    Columns.AddBraces(ExcludedColumns),
                                    post.Id.EscapeForSql(),
                                    post.Title.EscapeForSql(),
                                    post.ShortTitle.EscapeForSql(),
                                    post.Summary.EscapeForSql(),
                                    post.Content.EscapeForSql(),
                                    post.PublicLink.EscapeForSql(),
                                    post.PostDate.EscapeForSql(true), 
                                    post.CreatedBy.EscapeForSql(), 
                                    post.ModifiedBy.EscapeForSql()
                                    );
            ExecuteQuery(query);
        }

        public MediaItem GetPostMedia(string postId, string mediaId, out PublishErrorEnum error)
        {
            var media = postId.HasValue() && mediaId.HasValue() ? Instances.Repositories.MediaItemRepository.Get(mediaId) : null;
            if (media == null)
                error = PublishErrorEnum.NO_MEDIA;
            else
                error = PublishErrorEnum.NO_ERROR;
            return media;
        }

        public List<Operation> GetPostOperations(string postId)
        {
            var operations = postId.HasValue() ? Instances.Repositories.OperationRepository.getPostOperationsById(postId) : null;
            return operations;
        }

        public List<Post> GetCurrentUserPosts(string userId)
        {
            var query = SqlSelect + $" WHERE CreatedBy='{userId}'";
            List<Post> scheduledPosts = connection.Query<Post>(query).ToList();

            return scheduledPosts;
        }
        public List<Post> GetPostByTitle(string title)
        {
            var query = SqlSelect + $" WHERE Content LIKE N'%{title}%' or Title LIKE N'%{title}%' or ShortTitle LIKE N'%{title}%'";
            List<Post> listPosts = connection.Query<Post>(query).ToList();
            return listPosts;
        }
        public List<Post> GetByDate(DateTime date, string userId)
        {
            string formattedDate = date.ToString("yyyy-MM-dd");
            var query = $"{SqlSelect} WHERE CONVERT(date, PostDate) = @Date AND CreatedBy= @userId";
            List<Post> posts = connection.Query<Post>(query, new { Date = formattedDate , userId = userId } ).ToList();

            return posts;
        }
    }
}
