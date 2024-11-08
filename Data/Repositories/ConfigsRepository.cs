using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Hosting;
using FeedHiveAuth.Areas.Social.Models;
using static FeedHiveAuth.Data.Database;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Data.Repositories
{
    public class ConfigsRepository : BaseRepository<Configs>
    {
        public ConfigsRepository()
        {
            TableName = Database.Tables.Configs;
            Columns = Database.Columns.Configs;
            ExcludedColumns = Database.ExcludedColumns.Configs;
        }
        protected string SqlUpdate => Columns.GenerateUpdateQuery(TableName, "Id", "Id,EnumKey,Name,CreationDate,MasterId");

        public void InsertConfigs(int enumKey, string Name, string jsonValue)
        {
            //var query = string.Format("INSERT INTO {0} ({1}) VALUES({2});", TableName, Columns.AddBraces(), (enumKey,Name,jsonValue));
            // "Id,EnumKey,Name,JsonValue,MasterId,CreationDate"
            string query = string.Format(
                                    "Insert Into {0} ({1}) Values ({2},{3},N{4},N{5},N{6},{7})",
                                    TableName,
                                    Columns.AddBraces(),
                                    Guid.NewGuid().ToString().EscapeForSql(),
                                    enumKey,
                                    Name.EscapeForSql(),
                                    jsonValue.EscapeForSql(),
                                    DomainTime.Now().EscapeForSql(true)
                                    );
            ExecuteQuery(query);
        }
        public new void UpdateConfigs(int enumKey, string id ,string newJsonValue)
        {
            var oldConfigs = GetConfigsByKey(enumKey);
            //var query = string.Format("INSERT INTO {0} ({1}) VALUES({2});", TableName, Columns.AddBraces(), (enumKey,Name,jsonValue));
            //"Id,EnumKey,Name,JsonValue,MasterId,CreationDate"
            try {
                UpdateColumn("JsonValue", newJsonValue, id);
            }catch(Exception exception){};    
        }
        public List<Configs> GetAllConfigs()
        {
            var query = SqlSelectWhole;
            List<Configs> configs = connection.Query<Configs>(query).ToList();
            return configs;
        }
        public Configs GetConfigsByKey(int enumKey)
        {
            var configsKey = GetAllConfigs().Where(c => c.EnumKey == enumKey).FirstOrDefault();
            return configsKey;
        }

    }
}
