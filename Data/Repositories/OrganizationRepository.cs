using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
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

        public int GetOrganizationUsersCount(string organizationId)
        {
            const string sql = @"
        SELECT COUNT(1)
        FROM AspNetUsers
        WHERE OrganizationId = @OrgId";

            return connection.Query<int>(sql, new { OrgId = organizationId }).FirstOrDefault();
        }
        public DeleteResult TryDeleteOrganization(
    string organizationId,
    string deletedByUserId)
        {
            var usersCount = GetOrganizationUsersCount(organizationId);

            if (usersCount > 0)
            {
                return DeleteResult.Blocked(
                    $"This organization has {usersCount} linked user(s).");
            }

            const string sql = @"
        UPDATE Organization
        SET IsDeleted = 1,
            DeletedOn = @Now,
            DeletedBy = @DeletedBy
        WHERE Id = @Id";

            connection.Execute(sql, new
            {
                Id = organizationId,
                Now = DomainTime.Now(),
                DeletedBy = deletedByUserId
            });

            return DeleteResult.Success();
        }



    }
}
