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

    }
}
