using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.JSON;
using FeedHiveAuth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using Telegram.Bot.Types;

namespace FeedHiveAuth.Data.Repositories
{
    public class MediaItemRepository : BaseRepository<MediaItem>
    {
        public MediaItemRepository()
        {
            TableName = Database.Tables.MediaItem;
            Columns = Database.Columns.MediaItem;
        }

        public List<MediaItem> MediasList(IFormFileCollection files, string currentuser)
        {
            //convert post.medias list to objects 
            //convert objects to mediaItem objects 
            //save mediaItem objects in Db with post.Id foreign key
            List<MediaItem> medias = new List<MediaItem>();
            var baseUrl = "";
            foreach (var file in files)
            {
                MediaItem mediaItem = new MediaItem();
                mediaItem.ThumbnailUrl = file.FileName;
                mediaItem.CreatedBy = currentuser;
                mediaItem.Caption = file.FileName;
#if DEBUG

                baseUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs?.appTechnicalConfigs?.LocalUrl;
#else
                baseUrl = SocialServiceHelper.getSocialConfigs().TechnicalConfigs?.appTechnicalConfigs?.PublicUrl;
#endif                
                mediaItem.Path = baseUrl + "uploads/" + file.FileName;
                //mediaItem.CreatedBy = GlobalContext.UserConfigs?.UserData?.Id;
                medias.Add(mediaItem);

            }
            return medias;

        }
        public void CreateMedia(List<MediaItem> postMedias)
        {
            foreach (var mediaItem in postMedias)
            {
                //Id, Caption, postId, creationDate, path, createdBy
                var extension = mediaItem.Caption.Split('.')[1];
                switch (extension)
                {
                    case "mp3":
                        mediaItem.Type = 20;
                        break;
                    case "mp4":
                        mediaItem.Type = 30;
                        break;
                    case "pdf":
                        mediaItem.Type = 50;
                        break;
                    default:
                        mediaItem.Type = 10;
                        break;
                }
                string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},{3},{4},{5},{6},{7},{8},{9})",
                                    TableName,
                                    Columns.AddBraces(),
                                    Guid.NewGuid().EscapeForSql(),
                                    mediaItem.Caption.EscapeForSql(),
                                    "null",
                                    DateTime.Now.EscapeForSql(),
                                    mediaItem.Path.EscapeForSql(),
                                    mediaItem.CreatedBy.EscapeForSql(),
                                    extension.EscapeForSql(),
                                    mediaItem.Type
                                    );
                ExecuteQuery(query);
            }
        }
        //create the media in mediaitem table 
        public List<MediaItem> InsertPostMedia(List<MediaItem> postMedias, string postId)
        {
            foreach (var mediaItem in postMedias)
            {
                //Id, Caption, postId, creationDate, path, createdBy
                var extension = mediaItem.Caption.Split('.')[1];
                switch (extension)
                {
                    case "mp3":
                        mediaItem.Type = 20;
                        break;
                    case "mp4":
                        mediaItem.Type = 30;
                        break;
                    case "pdf":
                        mediaItem.Type = 50;
                        break;
                    default:
                        mediaItem.Type = 10;
                        break;
                }
                var mid = Guid.NewGuid();
                string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},{3},{4},{5},{6},{7},{8},{9})",
                                    TableName,
                                    Columns.AddBraces(),
                                    mid.EscapeForSql(),
                                    mediaItem.Caption.EscapeForSql(),
                                    postId.EscapeForSql(),
                                    DateTime.Now.EscapeForSql(),
                                    mediaItem.Path.EscapeForSql(),
                                    mediaItem.CreatedBy.EscapeForSql(),
                                    extension.EscapeForSql(),
                                    mediaItem.Type
                                    );
                mediaItem.Id = mid.ToString();
                ExecuteQuery(query);
            }
            return postMedias;
        }

        public IEnumerable<MediaItem> GetMediaList()
        {

            var query = SqlSelectWhole;
            IEnumerable<MediaItem> mediaItems = connection.Query<MediaItem>(query).ToList();
            return mediaItems;
        }

        public List<MediaItem> GetMediaByPostId(string id)
        {
            var query = "select * from MediaItem where Id in  (select MediaItemId from PostMedias where PostId='" + @id + "')";
            List<MediaItem> postMediaItem = connection.Query<MediaItem>(query).ToList();
            return postMediaItem;
        }
        public List<PostMedia> GetPostMedias(string id)
        {
            var query = "SELECT * FROM PostMedias Where PostId='" + @id + "' ";
            List<PostMedia> postmedias = connection.Query<PostMedia>(query).ToList();
            return postmedias;
        }
        public List<MediaItem> GetMediasByPostId(string id)
        {
            var queryMediaIds = "SELECT MediaItemId FROM PostMedias Where PostId IN ('" + @id + "')";
            var mediaIds = connection.Query<string>(queryMediaIds, new { PostId = id }).ToList();
            if (!mediaIds.Any())
            {
                return new List<MediaItem>();
            }
            var queryMediaItems = "SELECT * FROM MediaItem WHERE Id IN @MediaIds";
            var postMediaItems = connection.Query<MediaItem>(queryMediaItems, new { MediaIds = mediaIds }).ToList();
            //List<MediaItem> postMediaItems = connection.Query<MediaItem>(query).ToList();
            return postMediaItems;
        }
        public MediaItem GetMediaById(string id)
        {
            var query = "SELECT * FROM MediaItem Where Id='" + @id + "' ";
            MediaItem mediaItem = connection.Query<MediaItem>(query).FirstOrDefault();
            return mediaItem;
        }
        public List<MediaItem> GetMediasByUser(string id)
        {
            var query = "SELECT * FROM MediaItem Where CreatedBy='" + @id + "' ";
            List<MediaItem> postMediaItems = connection.Query<MediaItem>(query).ToList();
            return postMediaItems;
        }

        public IEnumerable<PostMedia> GetPostMediaRelations(string mediaId)
        {
            using (connection)
            {
                var query = "SELECT * FROM PostMedias WHERE MediaItemId = @mediaId";
                return connection.Query<PostMedia>(query, new { mediaId = mediaId });
            }
        }
        public void DeletePostMedia(string mediaId, string postId)
        {
            var query = "Delete from PostMedias Where postId='" + postId + "' and mediaItemId='" + mediaId + "' ";
            var result = connection.Query<int>(query).FirstOrDefault();
        }
        public string GetPath(string id)
        {
            var query = "Select Path from MediaItem Where Id='" + id + "' ";
            var result = connection.Query<string>(query).FirstOrDefault();
            return result;
        }
    }
}
