using FeedHiveAuth.Models;
using System.Collections.Generic;

namespace FeedHiveAuth.Data.Repositories
{
    public class OperationRepository : BaseRepository<Operation>
    {
        public OperationRepository() {
            Columns = Database.Columns.Operation;
            TableName = Database.Tables.Operation;
        }
        public List<Operation> getPostOperationsById(string postId)
        {
            var query = SqlSelect + $" WHERE PostId = '{postId}'";
            List <Operation> ops = connection.Query<Operation>(query).ToList();
            return ops;
        }
    }
}
