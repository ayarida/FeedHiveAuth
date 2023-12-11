using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
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
            foreach (var file in files)
            {
                MediaItem mediaItem = new MediaItem();
                mediaItem.ThumbnailUrl = file.FileName;
                mediaItem.Caption = file.FileName;
                mediaItem.Path = "~/wwwroot/uploads/" + file.FileName;
                medias.Add(mediaItem);
            }
            return medias;

        }

        public void InsertPostMedia(List<MediaItem> postMedias, string postId)
        {
            foreach(var mediaItem in postMedias) {
                //Id, Caption, postId, creationDate, path

                string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},{3},{4},{5},{6})",
                                    TableName,
                                    Columns.AddBraces(),
                                    Guid.NewGuid().EscapeForSql(),
                                    mediaItem.Caption.EscapeForSql(),
                                    postId.EscapeForSql(),
                                    DateTime.Now.EscapeForSql(), 
                                    mediaItem.Path.EscapeForSql()
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
    }
}
