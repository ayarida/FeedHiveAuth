using Elfie.Serialization;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using System.Text;

namespace FeedHiveAuth.Data.Repositories
{
    public class ResourcesRepository:BaseRepository<Resources>
    {
        public static string _connectionString = DatabaseConnection.GetConnectionStrings();
        public static CustomSqlConnection connection = DatabaseConnection.GetConnection(_connectionString);
        public ResourcesRepository()
        {
            TableName = Database.Tables.Resources;
            Columns = Database.Columns.Resources;
        }
        public ResourceModel GetById(string id)
        {
            using (connection)
            {
                return connection.Query<ResourceModel>("select [Id],[Key],[Language],[Value] from Resources" + " WHERE Id=@id", new { Id = id }).FirstOrDefault();

            }
        }
        public IEnumerable<ResourceModel> GetAll()
        {
            var query = SqlSelectWhole;
            List<ResourceModel> resources = connection.Query<ResourceModel>(query).ToList();
            return resources;
        }
        public void SaveResource(Resources resource)
        {
            resource.Id = Guid.NewGuid().ToString();
            string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},N{3},N{4},N{5})",
                                    TableName,
                                    Columns.AddBraces(ExcludedColumns),
                                    resource.Id.EscapeForSql(),
                                    resource.Key.EscapeForSql(),
                                    resource.Value.EscapeForSql(),
                                   
                                    resource.Language.EscapeForSql()
                                    
                                    

                                    );
            ExecuteQuery(query);
        }
        public void UpdateResource(Resources resource)
        {
   
            string query = string.Format(
                                    "Update {0} Set [Key]={2},[Value]=N{3},[Language]={4} where Id = {1}",
                                    TableName,
                                    resource.Id.EscapeForSql(),
                                    resource.Key.EscapeForSql(),
                                    resource.Value.EscapeForSql(),
                                    resource.Language.EscapeForSql()



                                    );
            ExecuteQuery(query);
        }

    }
}
