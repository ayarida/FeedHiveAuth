using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class OperationRepository : BaseRepository<Operation>
    {
        public OperationRepository() {
            Columns = Database.Columns.Operation;
            TableName = Database.Tables.Operation;
        }
    }
}
