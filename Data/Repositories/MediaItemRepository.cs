using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace FeedHiveAuth.Data.Repositories
{
    public class MediaItemRepository : BaseRepository<MediaItem>
    {
        public MediaItemRepository()
        {
            TableName = Database.Tables.MediaItem;
            Columns = Database.Columns.MediaItem;
        }

        public List<MediaItem> MediasList(IFormFileCollection files)
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
                mediaItem.Caption = file.FileName;
#if DEBUG

                baseUrl = SocialConfigs.Construct().TechnicalConfigs.LocalUrl;
#else
                baseUrl = SocialConfigs.Construct().TechnicalConfigs.PublicUrl;
#endif                
                mediaItem.Path = baseUrl + "uploads/" + file.FileName;
                mediaItem.CreatedBy = GlobalContext.UserConfigs?.UserData?.Id;
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
        public void InsertPostMedia(List<MediaItem> postMedias, string postId)
        {
            foreach(var mediaItem in postMedias) {
                //Id, Caption, postId, creationDate, path, createdBy
                var extension = mediaItem.Caption.Split('.')[1];
                switch(extension)
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
                                    postId.EscapeForSql(),
                                    DateTime.Now.EscapeForSql(), 
                                    mediaItem.Path.EscapeForSql(), 
                                    mediaItem.CreatedBy.EscapeForSql(), 
                                    extension.EscapeForSql(), 
                                    mediaItem.Type
                                    );
                ExecuteQuery(query);
            }
        }

        public IEnumerable<MediaItem> GetMediaList()
        {
            var query = SqlSelectWhole;
            IEnumerable<MediaItem> mediaItems = connection.Query<MediaItem>(query).ToList(); ;
            return mediaItems;
        }

        public MediaItem GetMediaByPostId(string id) {
            var query = "SELECT * FROM MediaItem Where PostId='" + @id + "' "; 
            MediaItem postMediaItem =  connection.Query<MediaItem>(query).FirstOrDefault();
            return postMediaItem;
        }

        public List<MediaItem> GetMediasByPostId(string id)
        {
            var query = "SELECT * FROM MediaItem Where PostId='" + @id + "' ";
            List<MediaItem> postMediaItems = connection.Query<MediaItem>(query).ToList();
            return postMediaItems;
        }
    }
}
