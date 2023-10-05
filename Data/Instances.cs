using FeedHiveAuth.Data.Repositories;

namespace FeedHiveAuth.Data
{
    public class Instances
    {
        public static readonly Instances All = new Instances();
        public static AllRepositories Repositories;

        public Instances()
        {
            Repositories = new AllRepositories();
            Repositories.Fill();
        }

        public class AllRepositories
        {
            public SubscriptionRepository SubscriptionRepository { get; set; }
            public PostRepository PostRepository { get; set; }
            public ChannelRepository ChannelRepository { get; set; }
            public MediaItemRepository MediaItemRepository { get; set; }
            public RoleRepository RoleRepository { get; set; }
            public UserRepository UserRepository { get; set;}
            public OperationRepository OperationRepository { get; set; }
            public void Fill()
            {
                SubscriptionRepository = new SubscriptionRepository();
                PostRepository = new PostRepository();
                ChannelRepository = new ChannelRepository();
                MediaItemRepository = new MediaItemRepository();
                RoleRepository = new RoleRepository();
                UserRepository = new UserRepository();
                OperationRepository = new OperationRepository();
            }
        }
        

    }
}
