using FeedHiveAuth.Models;

namespace FeedHiveAuth.Data.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>
    {
        public OrganizationRepository()
        {
            TableName = Database.Tables.Organization;
            Columns = Database.Columns.Organization;
        }        

        public Organization GetById(string id)
        {
            var sql = SqlSelect + " WHERE Id = @Id";
            var org = connection.Query<Organization>(sql, new { Id = id }).FirstOrDefault();
            return org;
        }

        public void Save(Organization organization)
        {
            Insert(organization);
        }

        public void UpdateOrganization(Organization organization)
        {
            Update(organization);
        }

        public void DeleteOrganization(string id)
        {
            Delete(id);
        }
    }
}
