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
        public MediaItemRepository() {
            TableName = Database.Tables.MediaItem;
            Columns = Database.Tables.MediaItem;
        }

        public List<MediaItem> MediasList(IFormFileCollection files)
        {
            //convert post.medias list to objects 
            //convert objects to mediaItem objects 
            //save mediaItem objects in Db with post.Id foreign key
            List<MediaItem> medias = new List<MediaItem>();
            foreach(var file in files)
            {
                MediaItem mediaItem = new MediaItem();
                mediaItem.ThumbnailUrl = file.FileName;
                mediaItem.Caption = file.FileName;
                medias.Add(mediaItem);
            }
            return medias;

        }

       public void InsertPostMedia(List<MediaItem> postMedias, string postId)
        {
             //= "INSERT INTO AspNetRoles(Id,Name,NormalizedName) values ('1234','" + role.Name + "','" + role.NormalizedName + "')";
            foreach(var mediaItem in postMedias) {
                string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15})",
                                    TableName,
                                    Columns.AddBraces(),
                                    Guid.NewGuid().EscapeForSql(),

                                    mediaItem.Caption.EscapeForSql(),
                                    mediaItem.Description.EscapeForSql(),
                                    mediaItem.Tags.EscapeForSql(),
                                    mediaItem.Path.EscapeForSql(),
                                    mediaItem.Extension.EscapeForSql(),
                                    mediaItem.Type,
                                    mediaItem.ThumbnailUrl.EscapeForSql(),
                                    mediaItem.CreationDate, 
                                    mediaItem.CreatedBy.EscapeForSql(),
                                    mediaItem.Version,
                                    mediaItem.Duration, 
                                    mediaItem.Info.EscapeForSql(),
                                    postId.EscapeForSql()
                                    );
                ExecuteQuery(query);
            }
        }
    }
}
