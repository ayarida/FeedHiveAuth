using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Repositories;
using FeedHiveAuth.Models.Enums;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace FeedHiveAuth.Models.Common
{
    public class TasksBgService : BackgroundService
    {
        public PostRepository _postService = new PostRepository();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*while (!stoppingToken.IsCancellationRequested)
            {
                // Perform your database checks or any other tasks here
                Debug.WriteLine("Checking database at: " + DateTime.Now);

                //Get all the scheduled posts from database

                var scheduledPosts = _postService.GetScheduledPosts();

                //Check the date of the scheduled posts if pDate <= currDate , make its status publish
                DateTime currentDateTime = DateTime.Now;
                if (scheduledPosts.Count > 0)
                {
                    foreach (var scheduledPost in scheduledPosts)
                    {
                        int comp = DateTime.Compare(scheduledPost.PostDate.Value, currentDateTime);
                        if (comp < 0)
                        {
                            scheduledPost.Status = StatusEnum.Published.Value();
                            _postService.UpdateColumn("Status", scheduledPost.Status, scheduledPost.Id);
                        }
                        else
                        {
                            scheduledPost.Status = StatusEnum.Scheduled.Value();
                            _postService.UpdateColumn("Status", scheduledPost.Status, scheduledPost.Id);
                        }
                    }
                }

                // Adjust the interval as needed
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }*/
        }

    }
}
