using FeedHiveAuth.Data.Extensions;

namespace FeedHiveAuth.Data.Helpers
{
    

        public static class AppResources

    {
            private static Dictionary<string, string> Entries = new Dictionary<string, string>();
            public static string Label(string key, string lang = null)
            {
                return GetByKey(key, lang);
            }
            public static string GetByKey(string key, string lang = null, string defaultt = null)
            {
                if (string.IsNullOrEmpty(key))
                    return "";
                string value;
                var keylower = key.ToLowerInvariant();
                var keylang = lang.IsNotNullOrEmpty() ? keylower + "." + lang : keylower;
                if (Entries.TryGetValue(keylang, out value))
                    return value;

                if (Entries.TryGetValue(key, out value))
                    return value;

                //insert resource Warning
                var val = key.Split('.').LastOrDefault() ?? key;
                return val == val.ToUpper() ? val : defaultt ?? val.SplitCamelCase();
            }
        public static void AssignResources()
       {
            Entries = new  Dictionary<string, string>();
            var resources = Instances.Repositories.ResourcesRepository.GetAll();
            foreach (var resource in resources)
            {
                var key = resource.Language.IsNotNullOrEmpty() ? resource.Key + "." + resource.Language : resource.Key;
                key = key.ToLowerInvariant();
                Dictionary<string, string> subscriptionEntries;
                
                    if (!Entries.ContainsKey(key))
                    {
                        try
                        {
                            Entries.Add(key, resource.Value);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                
            }
        }
    }
    

