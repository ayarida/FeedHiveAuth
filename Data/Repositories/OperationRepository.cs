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
        public void UpdateStatus(string id, int status)
        {
            Execute("UPDATE " + TableName + " SET Status=@Status WHERE Id=@Id ", new { Id = id, Status = status });
        }
        public void UpdateCurrentState(Operation operation)
        {
            UpdateColumn("CurrentState", operation.CurrentState, operation.Id.ToString());
        }
    }
}
