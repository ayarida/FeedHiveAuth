using FeedHiveAuth.Areas.Social.Models;
using FeedHiveAuth.Models;
using FeedHiveAuth.Areas.Social.SocialTwitter.Clients;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Authorization;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Trends;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Tweet;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Data.Helpers;
using FeedHiveAuth.Models.Common;
using System.Threading.Channels;
using FeedHiveAuth.Areas.Social.SocialTwitter.Models.Account;
using FeedHiveAuth.Areas.Social.SocialFacebook.Models.Account;
using User = FeedHiveAuth.Models.User;

namespace FeedHiveAuth.Areas.Social.SocialTwitter.Handlers
{
    public static class TwitterScraper
    {
        
        #region trends

        public static IEnumerable<Trend> GetTrendsByPlace(ulong? placeId, string excludedTags, Guid subscriptionId)
        {
            var configs = SocialServiceHelper.getSocialConfigs().TwitterConfigs;
            if (!configs.TrendingTopics.Enable)
            {
                return null;
            }

            placeId = placeId ?? configs.TrendingTopics.PlaceId;
            var buildDir = AppDomain.CurrentDomain.BaseDirectory;
            var historyDir = Path.GetFullPath(Path.Combine(buildDir, @"..\")) + "ServicesData\\twitter\\trendingtopics\\" + placeId + "\\";
            if (!Directory.Exists(historyDir))
            {
                Directory.CreateDirectory(historyDir);
            }

            var files = Directory.GetFiles(historyDir);
            var filePath = historyDir + @"file" + (files.Length > 1 ? (files.Length - 1).ToString() : "") + ".json";

            if (!File.Exists(filePath))
            {
                var newFile = File.Create(filePath);
                newFile.Close();
            }
            var content = filePath.ReadFromJsonFile<TrendsList>();
            if (content?.Trends?.Count() > 0 && content.QueryDate.HasValue && content.QueryDate.Value.AddMinutes(30) > DomainTime.Now())
                return content.Trends;

            if (content != null && content.QueryDate.HasValue && content.QueryDate.Value.AddMinutes(30) <= DomainTime.Now())
            {
                filePath = historyDir + @"file" + files.Length.ToString() + ".json";
            }

            var client = new TrendsClient(configs, new TwitterCredentials { Token = configs.Application.Token, TokenSecret = configs.Application.TokenSecret });
            var trends = client.GetByPlace(placeId.Value, excludedTags ?? configs.TrendingTopics.ExcludedTags);
            if (!trends.IsSuccessful)
            {
                //Logger.Error(typeof(TwitterScraper), "Failed to get trending topics: " + string.Join(",", trends.Data.FirstOrDefault().Errors.Select(x => x.Message)));
                return null;
            }

            content = trends.Data.FirstOrDefault();
            content.QueryDate = DomainTime.Now();
            content.WriteToJsonFile(filePath, false);

            return content.Trends;
        }
        public static void WriteToJsonFile<T>(this T objectToWrite, string filePath, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = objectToWrite.Serialize();
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            catch (Exception ex)
            {
                var t = ex;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        #endregion

        #region tweet timelines

        public static IEnumerable<Tweet> GetUserTimeline(FeedHiveAuth.Models.Channel channel, IEnumerable<string> hashTags, int count = 10, bool allHashTags = false)
        {
            var token = channel.Credentials.FromJson<TwitterCredentials>();
            var configs = new TwitterConfigs();
            var username = "currentuser";
            //var subscription = Collections.Subscriptions.FirstOrDefault(x => x.Id.Equals(channel.SubscriptionId));
            var buildDir = AppDomain.CurrentDomain.BaseDirectory;
            var historyDir = Path.GetFullPath(Path.Combine(buildDir, @"..\")) + $"ServicesData\\twitter\\{username}\\timeline\\";
            if (!Directory.Exists(historyDir))
            {
                Directory.CreateDirectory(historyDir);
            }

            var files = Directory.GetFiles(historyDir);
            var filePath = historyDir + @"usertimeline.json";

            if (!File.Exists(filePath))
            {
                var newFile = File.Create(filePath);
                newFile.Close();
            }

            var history = filePath.ReadFromJsonFile<List<Tweet>>();
            var lastTweet = history.FirstOrDefault();

            var client = new TweetClient(configs, token);
            var timeline = client.GetUserTimeline(lastTweet?.Id);
            if (!timeline.IsSuccessful)
            {
                var errors = string.Join(",", timeline.Data.FirstOrDefault().Errors.Select(x => x.Message));
                //Logger.Error(typeof(TwitterScraper), "Failed to get trending topics: " + errors);
                throw new Exception(errors);
            }

            var results = timeline.Data.Where(x => allHashTags ? hashTags.ToList().All(hashtag => x.Text.ContainsIgnoreCase(hashtag)) : hashTags.ToList().Any(hashtag => x.Text.ContainsIgnoreCase(hashtag))).Take(count).ToList();

            if (results.Count() < count && history.Count() > 0)
            {
                var historyResults = history.Where(x => allHashTags ? hashTags.ToList().All(hashtag => x.Text.ContainsIgnoreCase(hashtag)) : hashTags.ToList().Any(hashtag => x.Text.ContainsIgnoreCase(hashtag))).Take(count - results.Count());
                results.AddRange(historyResults);
            }

            if (timeline.Data.NotEmpty())
            {
                history.InsertRange(0, timeline.Data);
                history.WriteToJsonFile(filePath, false);
            }

            return results.OrderByDescending(x => x.CreatedAt);
        }

        public static IEnumerable<Tweet> SearchTweets(string screenName, Guid subscriptionId, IEnumerable<string> hashTags, int count = 10, bool allHashTags = false)
        {
            var configs = new TwitterConfigs();
            var client = new SearchClient(configs, new TwitterCredentials { Token = configs.Application.Token, TokenSecret = configs.Application.TokenSecret });
            var searchResult = client.SearchTweets(screenName, hashTags, allHashTags, count);
            if (!searchResult.IsSuccessful)
            {
                var errors = string.Join(",", searchResult.Data.Errors.Select(x => x.Message));
                //Logger.Error(typeof(TwitterScraper), "Failed to get trending topics: " + errors);
                throw new Exception(errors);
            }

            return searchResult.Data.Statuses;
        }

        #endregion
    }
}
