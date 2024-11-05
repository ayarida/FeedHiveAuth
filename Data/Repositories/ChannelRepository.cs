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
        protected string SqlUpdate => Columns.GenerateUpdateQuery(TableName, "Id", "Id,Name,NetworkId,PublicId,CreationDate");


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

        public Channel GetById(string Id, out ChannelErrorEnum error)
        {
            var channel = Collections.Channels().FirstOrDefault(x => x.Id.ToString().EqualsIgnoreCase(Id));

            if (channel == null)
            {
                error = ChannelErrorEnum.NOT_FOUND;
                return channel;
            }
            error = ChannelErrorEnum.NO_ERROR;
            return channel;
        }

        public string GetNetworkUrl(string networkType, string networkId)
        {
            var network = EnumExtension.FromKey<SocialNetworkTypeEnum>(networkType);
            switch (network)
            {
                case SocialNetworkTypeEnum.Facebook:
                    return "https://facebook.com/" + networkId;
                case SocialNetworkTypeEnum.Twitter:
                    return "https://twitter.com/" + networkId;
                case SocialNetworkTypeEnum.Telegram:
                    return "https://t.me/" + networkId;
                case SocialNetworkTypeEnum.Youtube:
                    return "https://www.youtube.com/channel/" + networkId;
                default:
                    return "";
            }
        }

        public Channel GetByNetwork(string network, string networkId, out ChannelErrorEnum error)
        {
            var oldChannel = Collections.Channels().FirstOrDefault(c => c.NetworkId.EqualsIgnoreCase(networkId) && c.Network.EqualsIgnoreCase(network));
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
                try
                {
                    ch.Status = StatusEnum.Active.Value();
                    ch.Id = GuidExtension.GenerateGuid().ToString();
                    ch.CreationDate = DomainTime.Now();
                    this.Insert(ch);
                }
                catch (Exception ex)
                {
                }
            }
            //this.Insert(channels);
            //Collections.RefreshChannels();
            return channels;
        }

        public Channel Create(string network, out ChannelErrorEnum error)
        {
            //dailymotion network create
            var type = EnumExtension.FromKey<SocialNetworkTypeEnum>(network);
            var channel = new Channel()
            {
                Network = type.Key(),
                Account = SocialAccountTypeEnum.Profile.Key(),
            };
            error = ChannelErrorEnum.NO_ERROR;
            return channel;
        }
        public new void Update(Channel channel)
        {
            channel.LastModified = DomainTime.Now();
            var sql = SqlUpdate;
            Execute(sql, channel);
        }

    }
}
