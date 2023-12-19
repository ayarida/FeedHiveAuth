using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using FeedHiveAuth.Models.Common;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Data.Repositories
{
    public class ChannelRepository : BaseRepository<Channel>
    {
        public static string _connectionString = DatabaseConnection.GetConnectionStrings();
        public static CustomSqlConnection connection = DatabaseConnection.GetConnection(_connectionString);
        public ChannelRepository()
        {
            TableName = Database.Tables.Channel;
            Columns = Database.Columns.Channel;
        }
        public new IEnumerable<Channel> GlobalGetAll(string o = "")
        {
            var query =
                $"SELECT * " +
                $"FROM {Database.Tables.Channel}";

            using (connection)
            {
                var channels = connection.Query<Channel>(query).ToList();
                return channels;

            }
        }

        public Channel GetByNetwork(string subscriptionId, string network, string networkId, out ChannelErrorEnum error)
        {
            var oldChannel = Collections.ChannelsOf(subscriptionId).FirstOrDefault(c => c.NetworkId.EqualsIgnoreCase(networkId) && c.Network.EqualsIgnoreCase(network));
            if (oldChannel == null)
                error = ChannelErrorEnum.NOT_FOUND;
            else
                error = ChannelErrorEnum.NO_ERROR;
            return oldChannel;
        }

        public Channel AddChannel(Channel channel)
        {
            return Instances.Repositories.ChannelRepository.AddChannel1(channel);
        }
        public Channel AddChannel1(Channel channel)
        {
            return AddChannels(new[] { channel }).FirstOrDefault();
        }

        public IEnumerable<Channel> AddChannels(IEnumerable<Channel> channels)
        {
            foreach (var ch in channels)
            {
                ch.Status = StatusEnum.Active.Value();
                ch.Id = GuidExtension.GenerateGuid().ToString();
                ch.CreationDate = DomainTime.Now();
                this.Insert(ch);
            }
            //this.Insert(channels);
            //Collections.RefreshChannels();
            return channels;
        }

        /*public void RefreshChannels()
        {
            Collections.RefreshChannels();
        }*/

    }
}
