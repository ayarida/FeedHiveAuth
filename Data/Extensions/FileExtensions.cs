namespace FeedHiveAuth.Data.Extensions
{
    public static class FileExtensions
    {
        public static T ReadFromJsonFile<T>(this string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return fileContents.FromJson<T>();
            }
            catch (Exception ex)
            {
                return default(T);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
