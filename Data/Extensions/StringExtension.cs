using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FeedHiveAuth.Data.Extensions
{
    public static class StringExtensions
    {

        public static string CleanSql(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Trim().Replace("'", "''");
        }
        #region strings
        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            return s;
        }

        public static string ToIndianNumber(this string input)
        {
            UTF8Encoding utf8Encoder = new UTF8Encoding();
            Decoder utf8Decoder = utf8Encoder.GetDecoder();
            StringBuilder convertedChars = new StringBuilder();
            char[] convertedChar = new char[1];
            byte[] bytes = { 217, 160 };
            char[] inputCharArray = input.ToCharArray();
            foreach (char c in inputCharArray)
            {
                if (char.IsDigit(c))
                {
                    bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                    utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                    convertedChars.Append(convertedChar[0]);
                }
                else
                {
                    convertedChars.Append(c);
                }
            }
            return convertedChars.ToString();
        }
        public static int[] ToIntegers(this string str1)
        {
            if (string.IsNullOrEmpty(str1))
                return null;
            return str1.Split(',')
                        .Select(z => Convert.ToInt32(z)).ToArray();
        }
        public static Guid[] ToGuids(this string str)
        {
            return str.Split(',').Select(Guid.Parse).ToArray();
        }
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        public static string SplitCamelCase(this string source)
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z])"));
        }
        public static string Clean(this string text)
        {
            return text.Clean(false);
        }
        public static string Clean(this string text, bool removePunctuation)
        {
            return text.Clean(removePunctuation, true, false, false);
        }
        public static string KillEveryThingBeforeFirst(this string input, char c)
        {
            int index = input.IndexOf(c);
            if (index > 0)
                return input.Substring(input.IndexOf(c) + 1);
            return input;
        }
        public static string KillEveryThingBeforeLast(this string input, char c)
        {
            int index = input.LastIndexOf(c);
            if (index > 0)
                return input.Substring(input.IndexOf(c) + 1);
            return input;
        }
        public static string KillEveryThingAfterFirst(this string input, char c)
        {
            int index = input.IndexOf(c);
            if (index > 0)
                return input.Substring(0, index);
            return input;
        }
        public static string KillEveryThingAfterLast(this string input, char c)
        {
            int index = input.LastIndexOf(c);
            if (index > 0)
                return input.Substring(0, index);
            return input;
        }
        public static string Clean(this string text, bool removePunctuation, bool removeHtml, bool removeNumbers, bool removeSymbols)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            char[] chars = new char[text.Length];
            int index = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!char.IsControl(c) &&
                    !(removePunctuation && char.IsPunctuation(c)) &&
                    !(removeNumbers && char.IsNumber(c)) &&
                    !(removeSymbols && char.IsSymbol(c)))
                {
                    chars[index++] = c;
                }
            }
            var result = new string(chars, 0, index);
            result = result.RemoveLineEndings();
            if (removePunctuation)
                result = result.RemoveDiacritics();
            if (removeHtml)
            {
                result = result.CleanHtml();
                //result = HttpUtility.HtmlEncode(result);
            }

            return result;
        }
        public static string CleanCode(this string code)
        {
            if (string.IsNullOrEmpty(code)) return "";
            return code;//.ToSlug();
        }
        public static string RemoveLineEndings(this string value, string replace = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", replace).Replace("\n", replace).Replace("\r", replace).Replace(lineSeparator, replace).Replace(paragraphSeparator, replace);
        }
        public static string RemoveDiacritics(this string stIn, string replaceWith = "")
        {
            return stIn.RemoveArabicDiacritics(true, replaceWith);
        }
        public static string ToSlug(this string phrase, int maxLength = 100, string replaceDiacriticsWith = "")
        {
            phrase = phrase?.Trim() ?? "";
            if (string.IsNullOrEmpty(phrase)) return "";
            var tempResult = phrase.RemoveDiacritics(replaceDiacriticsWith).ToLower();
            //tempResult = Regex.Replace(tempResult, @"[^a-z0-9\s-]", "");
            tempResult = Regex.Replace(tempResult, @"[^\w\s-]", "-");
            tempResult = Regex.Replace(tempResult, @"\s+", " ").Trim();
            tempResult = tempResult.Substring(0,
                tempResult.Length <= maxLength ? tempResult.Length : maxLength).Trim();
            tempResult = Regex.Replace(tempResult, @"\s", "-");
            return tempResult;
        }
        public static string CleanHtml(this string html, string newline = "")
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            // remove html
            return Regex.Replace(html, "<[^>]+?>", "").Replace("\n\r", newline).Replace("&nbsp;", " ");
        }
        public static string RemoveArabicDiacritics(this string stIn, bool arabic, string replaceWith)
        {
            if (string.IsNullOrEmpty(stIn)) return "";
            string stFormD;
            if (arabic)
                stFormD = stIn.Normalize(NormalizationForm.FormKC);
            else
                stFormD = stIn.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
                else if (replaceWith.IsNotNullOrEmpty())
                {
                    sb.Append(replaceWith);
                }
            }
            return sb.ToString().RemoveLineEndings().Normalize(NormalizationForm.FormC);
        }
        public static bool IsGuid(this string str)
        {

            if (string.IsNullOrEmpty(str))
                return false;
            if (str.StartsWith("/"))
                str = str.Substring(1);
            Guid id;
            return Guid.TryParse(str, out id);
        }
        public static string Trim(this string text, int numOfChars, string breakingPhrase = " ", bool noWordCut = false)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length < numOfChars) return text;
            var finalNumOfChars = text.Substring(0, numOfChars).LastIndexOf(breakingPhrase, StringComparison.OrdinalIgnoreCase);

            if (finalNumOfChars == -1)
            {
                finalNumOfChars = numOfChars - 3;
                return string.Concat(text.Substring(0, finalNumOfChars), "...");
            }

            if (noWordCut && finalNumOfChars + 4 >= numOfChars)
            {
                finalNumOfChars = numOfChars - 3;
                return string.Concat(text.Substring(0, finalNumOfChars), "...");
            }

            return string.Concat(text.Substring(0, finalNumOfChars), " ...");
        }

        public static string DateParser(DateTime creationDate, string language, Guid subscriptionId)
        {
            //TODo migrate DateParser
            throw new NotImplementedException();
        }

        public static string ToTitleCase(this string original)
        {
            if (string.IsNullOrEmpty(original)) return "";
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(original);
        }

        public static string TrimParagraph(this string text, int? numOfChars, string breakingPhrase = " ", bool bytes = false)
        {
            int index = -1;

            if (string.IsNullOrEmpty(text)) return string.Empty;
            var textlength = bytes ? text.YoutubeLength() : text.Length;
            if (numOfChars == null || numOfChars.Value == 0 || textlength < numOfChars) return text;

            do
            {
                index = text.LastIndexOf(breakingPhrase, StringComparison.OrdinalIgnoreCase);
                if (index == -1) break;
                text = text.Substring(0, index);
            } while ((bytes ? text.YoutubeLength() : text.Length) + 4 > numOfChars);

            if (index == -1)
            {
                do
                {
                    text = text.Substring(0, text.Length - 1);

                } while ((bytes ? text.YoutubeLength() : text.Length) + 4 > numOfChars);
            }

            if (breakingPhrase == " ")
            {
                if (index == -1)
                    return string.Concat(text, "...");
                return string.Concat(text, "...");
            }

            return text;
        }

        public static bool IsBot(this string useragent)
        {
            if (string.IsNullOrEmpty(useragent)) return false;

            if (useragent.ToLower() == "Go-http-client/1.1".ToLower() ||
              useragent.ToLower() == "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0".ToLower())
                return true;


            var crawlers = new List<string>()
            {
                "bot","crawler","spider","80legs","baidu","yahoo! slurp","ia_archiver","mediapartners-google",
                "lwp-trivial","nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne",
                "atn_worldwide","atomz","bjaaland","ukonline","calif","combine","cosmos","cusco",
                "cyberspyder","digger","grabber","downloadexpress","ecollector","ebiness","esculapio",
                "esther","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
                "gcreep","golem","griffon","gromit","gulliver","gulper","whowhere","havindex","hotwired",
                "htdig","ingrid","informant","inspectorwww","iron33","teoma","ask jeeves","jeeves",
                "image.kapsi.net","kdd-explorer","label-grabber","larbin","linkidator","linkwalker",
                "lockon","marvin","mattie","mediafox","merzscope","nec-meshexplorer","udmsearch","moget",
                "motor","muncher","muninn","muscatferret","mwdsearch","sharp-info-agent","webmechanic",
                "netscoop","newscan-online","objectssearch","orbsearch","packrat","pageboy","parasite",
                "patric","pegasus","phpdig","piltdownman","pimptrain","plumtreewebaccessor","getterrobo-plus",
                "raven","roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au",
                "searchprocess","senrigan","shagseeker","site valet","skymob","slurp","snooper","speedy",
                "curl_image_client","suke","www.sygol.com","tach_bw","templeton","titin","topiclink","udmsearch",
                "urlck","valkyrie libwww-perl","verticrawl","victoria","webscout","voyager","crawlpaper",
                "webcatcher","t-h-u-n-d-e-r-s-t-o-n-e","webmoose","pagesinventory","webquest","webreaper",
                "webwalker","winona","occam","robi","fdse","jobo","rhcs","gazz","dwcp","yeti","fido","wlm",
                "wolp","wwwc","xget","legs","curl","webs","wget","sift","cmc", "twitterbot"
            };

            var iscrawler = crawlers.Any(x => useragent.ToLower().Contains(x));
            //Logger.Info(typeof(StringExtensions), "BOT: " + useragent);

            return iscrawler;
        }

        public static bool EqualsIgnoreCase(this string s1, string s2)
        {
            if (s1 == null && s2 != null || s1 != null && s2 == null)
                return false;
            if (s1 == null && s2 == null)
                return true;

            return s1.ToLower(CultureInfo.InvariantCulture) == s2.ToLower(CultureInfo.InvariantCulture);
        }
        public static string ReplaceIgnoreCase(this string original, string search, string replace)
        {
            return Regex.Replace(original, search, replace, RegexOptions.IgnoreCase);
        }
        public static bool ContainsIgnoreCase(this IEnumerable<string> strings, string s2)
        {
            if (strings == null) return false;
            return strings.Any(x => x.EqualsIgnoreCase(s2));
        }
        public static bool ContainsIgnoreCase(this string s1, string s2)
        {
            if (s1 == null) return false;
            return s1.ToLower().Contains(s2.ToLower());
        }

        public static string Base64Encode(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
        public static string Base64Decode(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            try
            {
                str = str.Replace(" ", "+");
                return Encoding.UTF8.GetString(Convert.FromBase64String(str));
            }
            catch (Exception ex)
            {
                return str;
            }
        }

        public static bool ContainsNonAlphabet(this string str)
        {
            foreach (char c in str.ToCharArray())
            {
                if (c > 127) // you probably don't want 127 either
                    return true;
                if (c < 32)  // I bet you don't want control characters 
                    return true;
            }
            return false;
        }

        public static string FontAwsome(this string icon, bool writefa = true)
        {
            if (string.IsNullOrEmpty(icon)) return "";
            var fa = writefa ? "fa " : "";
            if (icon.StartsWith("fa-") || icon.StartsWith("fas") || icon.StartsWith("fab")) return icon;
            return fa + "fa-" + icon;
        }
        public static string ReplaceLast(this string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        public static string NvalRatio(this string str, string altValue)
        {
            if (string.IsNullOrEmpty(str))
                return altValue;

            if (str == "-1" && (altValue == "-1" || string.IsNullOrEmpty(altValue)))
                return "1.7";//16/9

            return altValue;
        }

        public static string Nval(this string str, string altValue)
        {
            if (str.IsNotNullOrEmpty())
                return str;
            return altValue;
        }
        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
        #endregion


        #region urls
        public static string ExtractFirstIdFromUrl(this string url)
        {
            string id = "";
            if (url.IsNotNullOrEmpty())
            {
                int publicid = -1;
                foreach (var segment in url.Split('/'))
                {
                    if (int.TryParse(segment, out publicid)) break;
                }
                id = publicid > 0 ? publicid.ToString() : "";
            }
            return id;
        }
        public static string ComposeSafeUrl(this string url, bool? ssl)
        {
            if (!url.IsNotNullOrEmpty()) return string.Empty;
            if (!ssl.HasValue)
                return url.HtmlUrl();

            if (url.StartsWith("http"))
                url = url.Substring(url.IndexOf("://", StringComparison.Ordinal) + 3);
            if (url.StartsWith("//"))
                url = url.Substring(url.IndexOf("//", StringComparison.Ordinal) + 2);

            return string.Format("http{0}://{1}", ssl.Value ? "s" : "", url);


        }

        public static string GetFileNameExtension(this string filename)
        {
            if (filename.IsNotNullOrEmpty())
                return filename.Split('.').LastOrDefault();
            return "";
        }
        public static string HtmlUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            if (url.StartsWith("http") || url.StartsWith("//"))
                url = url.Substring(url.IndexOf("//", StringComparison.Ordinal) + 2);

            return string.Format("//{0}", url);


        }
        public static string SslUrl(this string url)
        {
            if (!url.StartsWith("http"))
                return "https:" + (url.StartsWith("//") ? "" : "//") + url;

            return url.Replace("http://", "https://");
        }

        public static string AddParameter(this string url, string key, string value, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            var ourl = url;
            if (url.StartsWith("//")) url = "http:" + url;


            Uri uri;
            bool validUrl = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);

            if (!validUrl)
                return url;

            //var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                var builder = new UriBuilder(uri);
                var newUrl = QueryHelpers.AddQueryString(url, key, value);
                // qs.Add();
                //builder.Query = qs.ToString();
                // var newurl = builder.Uri.ToString();
                return ourl.StartsWith("http:") ? newUrl : newUrl.Replace("http://", "//");
            }
            else
            {
                try
                {
                    var pre = url.Contains("?") ? "&" : "?";
                    url = url + pre + key + "=" + value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in AddParameter " + ex.Message + " " + url + "EXCEPTION:" + ex);

                }
                return url;
            }
        }
        public static string Resize(this string url, int width, int height)
        {
            return url.AddParameter("width", width.ToString()).AddParameter("height", height.ToString()).AddParameter("mode", "crop").AddParameter("scale", "both");
        }
        public static string Resize(this string url, int width)
        {
            return url.AddParameter("width", width.ToString());
        }
        public static string AddPreset(this string url, string value)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            if (url.Contains("preset="))
                return url;
            return string.Format("{0}{1}preset={2}", url, url.Contains("?") ? "&" : "?", value);
        }

        public static string RelativePath(this string url)
        {
            try
            {
                var uri = new Uri(url);
                return Uri.UnescapeDataString(uri.PathAndQuery);
            }
            catch (Exception ex)
            {
                return url;
            }
        }

        public static string GetFileExtension(this string url)
        {
            return Path.GetExtension(url)?.Replace(".", "").Split('?')[0];
        }

        public static string Decode(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            return WebUtility.UrlDecode(url);
        }

        public static string Encode(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            return Uri.EscapeDataString(url);
        }

        public static string ComposeUrl(this string host, params string[] segments)
        {
            host = host ?? "";
            var cleansengments = segments != null ? string.Join("/", segments.Where(x => x != null).Select(x => x.Trim('/'))) : "";
            return host.TrimEnd('/') + (cleansengments.IsNotNullOrEmpty() ? "/" + cleansengments : "");
        }

        public static string ValueOf(this NameValueCollection array, string key)
        {
            var result = "";
            var par = array.AllKeys.FirstOrDefault(x => x.EqualsIgnoreCase(key));
            if (par != null)
            {
                result = array[par];
            }
            return result;
        }
        #endregion

        #region exceptions
        public static string FullMessage(this Exception exception)
        {
            return exception.FullMessage("");
        }

        public static string FullMessage(this Exception exception, string message)
        {
            if (exception == null)
                return "";
            var builder = new StringBuilder(message);
            var inner = exception;
            while (inner != null)
            {
                builder.Append(inner.Message).Append(", ");
                inner = inner.InnerException;
            }
            if (builder.Length > message.Length) builder.Length = builder.Length - 2;
            return builder.ToString();

        }
        #endregion



        #region Hash
        public static string GenerateSalt()
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[32]; //256 bits
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
        public static string Hash(this string value, string salt)
        {
            if (value == null)
                return "";
            var hashed = Hash(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(salt));
            return Convert.ToBase64String(hashed);
        }
        public static byte[] Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();
            return new SHA256Managed().ComputeHash(saltedValue);
        }

        public static string Md5Hash(this string value)
        {
            var input = value;
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var c in hash) sb.Append(c.ToString("X2"));
            return sb.ToString();
        }
        public static string GetMd5(this string value, out byte[] hash)
        {
            var input = value;
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var c in hash) sb.Append(c.ToString("X2"));
            return sb.ToString();
        }

        public static byte[] Hmacsh1(this string data, string keysecret)
        {
            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(keysecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return hash;
        }

        #endregion

        #region Json

        public static IDictionary<string, object> JObjToDictionary(this JObject @object)
        {  //JObject comes from   Newtonsoft.Json.JsonConvert.DeserializeObject(formEntries);
            var result = @object.ToObject<Dictionary<string, object>>();

            var jObjectKeys = (from r in result
                               let key = r.Key
                               let value = r.Value
                               where value.GetType() == typeof(JObject)
                               select key).ToList();

            var jArrayKeys = (from r in result
                              let key = r.Key
                              let value = r.Value
                              where value.GetType() == typeof(JArray)
                              select key).ToList();

            jArrayKeys.ForEach(key => result[key] = ((JArray)result[key]).Values().Select(x => ((JValue)x).Value).ToArray());
            jObjectKeys.ForEach(key => result[key] = (result[key] as JObject).JObjToDictionary());

            return result;
        }
        public static T FromJson<T>(this string json, bool trycatch = true, bool logerror = true) where T : new()
        {
            if (string.IsNullOrEmpty(json) || !json.IsJson())
                return new T();

            if (trycatch)
            {
                try
                {

                    return JsonConvert.DeserializeObject<T>(json); // arabic attributes make obj NOT deserialized
                }
                catch (Exception ex)
                {
                    //if(logerror)
                    Console.WriteLine( "Unable to parse " + json + " to " + typeof(T).Name + ". " + ex.FullMessage(ex.StackTrace));
                    return new T();
                }
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static T FromJsonString<T>(this string json, bool logerror = true)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json); // arabic attributes make obj NOT deserialized
            }
            catch (Exception ex)
            {
                //if (logerror)
                //    Logger.Info(typeof(StringExtensions), "Error in FromJson " + json + ex.StackTrace, ex);
                return default;
            }
        }
        public static IEnumerable<T> ToArrayOf<T>(this string json, bool logerror = true)
        {
            if (string.IsNullOrEmpty(json))
                return Enumerable.Empty<T>();

            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
            }
            catch (Exception ex)
            {
                //if (logerror)
                //    Logger.Info(typeof(StringExtensions), "Error in ArrayFromJson " + json, ex);
            }
            return Enumerable.Empty<T>();
        }

        public static T ValueFromJson<T>(this string json, string key)
        {
            var jobject = json.FromJson<JObject>();
            return jobject.ValueFromJson(key, default(T));

        }
        public static T ValueFromJson<T>(this string json, string key, T defaultValue)
        {
            var jobject = json.FromJson<JObject>();
            return jobject.ValueFromJson(key, defaultValue);

        }
        public static T ValueFromJson<T>(this JObject jobject, string key, T defaultValue)
        {
            if (jobject == null || string.IsNullOrEmpty(key))
                return defaultValue;

            jobject.TryGetValue(key, StringComparison.InvariantCultureIgnoreCase, out JToken val);

            if (val.IsNullOrEmpty())
                return defaultValue;

            try
            {
                return val.Value<T>();
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }

        #endregion

        #region youtube
        public static string TrimYoutube(this string text, int numOfChars, string breakingPhrase = " ")
        {
            int index = -1;

            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length <= numOfChars) return text;

            do
            {
                index = text.LastIndexOf(breakingPhrase, StringComparison.OrdinalIgnoreCase);
                if (index == -1) break;
                text = text.Substring(0, index);
            } while (text.Length + 4 > numOfChars);

            if (index == -1)
            {
                do
                {
                    text = text.Substring(0, text.Length - 1);

                } while (text.Length + 4 > numOfChars);
            }

            if (breakingPhrase == " ")
            {
                if (index == -1)
                    return string.Concat(text, "...");
                return string.Concat(text, " ...");
            }

            return text;

        }

        public static int YoutubeLength(this string title)
        {
            if (string.IsNullOrEmpty(title)) return 0;
            var count = Encoding.UTF8.GetBytes(title).Count(x => x > 0);
            return count;
        }
        public static bool IsYoutubeTitleValid(this string title)
        {
            var count = Encoding.UTF8.GetBytes(title).Count(x => x > 0);
            return count <= 100;
        }

        public static string YoutubePlaylistUrl(this string playlist)
        {
            if (playlist.IsNotNullOrEmpty())
                return "https://www.youtube.com/playlist?list=" + playlist;
            return "";
        }
        public static string YoutubeId(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            try
            {
                return
                    Regex.Match(url,
                        @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&amp;]v=)|youtu\.be\/)([^""&amp;?\/ ]{11})")
                        .Groups[1].Value;
            }
            catch
            {
            }
            return "";
        }
        public static string ExtractYoutubeIdFromUri(this string url)
        {
            var YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
            var regexExtractId = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            var validAuthorities = new[] { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

            try
            {
                var authority = new UriBuilder(url).Uri.Authority.ToLower();

                //check if the url is a youtube url
                if (validAuthorities.Contains(authority))
                {
                    //and extract the id
                    var regRes = regexExtractId.Match(url);
                    if (regRes.Success)
                    {
                        return regRes.Groups[1].Value;
                    }
                }
            }
            catch { }


            return "";
        }
        #endregion

        #region cleanup
        private static Regex _tags = new Regex("<[^>]*(>|$)",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.Compiled);
        private static Regex _whitelist = new Regex(@"
        ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|mdnref|span|font|
        li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
        ^<(b|h)r\s?/?>$",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.Compiled |
            RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_refernce = new Regex(@"
        ^<span\sclass=""mdnrefernce""\s?>$|
        ^</span>$",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.Compiled |
            RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_a = new Regex(@"
        ^<a\s
        *(\stitle=""[^""<>]+"")?\s
        href=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
        (\starget=""[^""<>]*"")?
        (\srel=""nofollow"")?
        (\stitle=""[^""<>]+"")?\s?>$|
        ^</a>$",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.Compiled |
            RegexOptions.IgnorePatternWhitespace);

        private static Regex _whitelist_img = new Regex(@"
        ^<img\s
        src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
        (\swidth=""\d{1,3}"")?
        (\sheight=""\d{1,3}"")?
        (\salt=""[^""<>]*"")?
        (\stitle=""[^""<>]*"")?
        \s?/?>$",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.Compiled |
            RegexOptions.IgnorePatternWhitespace);
        private static Regex _whitelist_styled = new Regex(@"
            ^</?(a|p|span)\sstyle=""(font-weight:\s(normal|bold);\s?)?(font-style:\s(italic|normal|oblique);\s?)?(text-decoration:\s(underline|overline|none|line-through);\s?)?""\s?>$",
           RegexOptions.Singleline |
           RegexOptions.ExplicitCapture |
           RegexOptions.Compiled |
           RegexOptions.IgnorePatternWhitespace);
        private const string _blacklist_attribute = @"(?<=<)([^/>]+)(\s{0}=['""][^'""]+?['""])([^/>]*)(?=/?>|\s)";
        public static string AsSafeHtml(this string html)
        {

            if (string.IsNullOrEmpty(html)) return string.Empty;

            string tagname;
            Match tag;
            try
            {
                var style1 = new Regex("<style>", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                var style2 = new Regex("</style>", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                var tag1 = style1.Matches(html);
                var tag2 = style2.Matches(html);
                if (tag1.Count > 0 && tag2.Count > 0)
                {
                    //Logger.Info(typeof(HtmlSanitizerExtension), "tag1.Count:" + tag1.Count + " tag2.Count" + tag2.Count);
                    html = html.Remove(tag1[0].Index, tag2[0].Index + tag2[0].Length);
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(typeof(HtmlSanitizerExtension), "error in AsSafeHtml html:" + html, ex);
            }

            // match every HTML tag in the input
            MatchCollection tags = _tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();

                if (!(_whitelist.IsMatch(tagname) || _whitelist_a.IsMatch(tagname)
                    || _whitelist_img.IsMatch(tagname) || _whitelist_refernce.IsMatch(tagname)
                    || _whitelist_styled.IsMatch(tagname)
                    ))
                {
                    html = html.Remove(tag.Index, tag.Length);
                }
            }
            html.RemoveHtmlAttribute("style");

            return html;
        }
        public static string RemoveHtmlAttribute(this string input, string attributeName)
        {
            Regex reg = new Regex(string.Format(_blacklist_attribute, attributeName),
                   RegexOptions.IgnoreCase);
            return reg.Replace(input, item => item.Groups[1].Value + item.Groups[3].Value);
        }
        #endregion

        #region html
        //format line breaks of a browser to html syntax
        public static string ReplaceNewLineHtml(string content)
        {
            return Regex.Replace(content, @"\r\n?|\n", "<br />");
        }
        public static string NewLine(bool isHtmlContent)
        {
            if (isHtmlContent)
                return "<br/>";
            return "\n";
        }
        public static string DoubleBreak(bool isHtmlContent)
        {
            return NewLine(isHtmlContent) + NewLine(isHtmlContent);
        }
        public static string SubstringHtml(this string stringValue, int length)
        {
            var regexAllTags = new Regex(@"<[^>]*>");
            var regexIsTag = new Regex(@"<|>");
            var regexOpen = new Regex(@"<[^/][^>]*>");
            var regexClose = new Regex(@"</[^>]*>");
            var regexAttribute = new Regex(@"<[^ ]*");
            int necessaryCount = 0;
            if (regexAllTags.Replace(stringValue, "").Length <= length)
            {
                return stringValue;
            }
            string[] split = regexAllTags.Split(stringValue);
            string counter = "";
            foreach (string item in split)
            {
                if (counter.Length < length && counter.Length + item.Length >= length)
                {
                    necessaryCount = stringValue.IndexOf(item, counter.Length, StringComparison.Ordinal) + item.Substring(0, length - counter.Length).Length;
                    break;
                }
                counter += item;
            }
            var x = regexIsTag.Match(stringValue, necessaryCount);
            if (x.Value == ">")
            {
                necessaryCount = x.Index + 1;
            }
            string subs = stringValue.Substring(0, necessaryCount);
            var openTags = regexOpen.Matches(subs);
            var closeTags = regexClose.Matches(subs);
            List<string> openTagsList = new List<string>();
            foreach (var item in openTags)
            {
                string trans = regexAttribute.Match(item.ToString()).Value;
                if (trans.Last() == '>')
                {
                    trans = "</" + trans.Substring(1, trans.Length - 1);
                }
                else
                {
                    trans = "</" + trans.Substring(1, trans.Length - 1) + ">";
                }
                openTagsList.Add(trans);
            }
            foreach (Match close in closeTags)
            {
                openTagsList.Remove(close.Value);
            }
            for (int i = openTagsList.Count - 1; i >= 0; i--)
            {
                subs += openTagsList[i];
            }
            return subs;
        }
        #endregion html

        public static string AddHyperlinkTagToLinks(this string text)
        {
            Regex urlRx = new Regex(@"(?<url>(http:[/][/]|www.)([a-z]|[A-Z]|[0-9]|[/.]|[~])*)", RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(text);

            foreach (Match match in matches)
            {
                var url = match.Groups["url"].Value;
                text = text.Replace(url, string.Format("<a href=\"{0}\">{0}</a>", url));
            }

            return text;
        }

        public static string ExtractField(this string field)
        {
            if (!field.StartsWith("[") || !field.EndsWith("]")) return field;
            return field.TrimStart('[').TrimEnd(']');
        }

        public static string GetFieldName(this string param)
        {
            if (string.IsNullOrEmpty(param)) return "";
            var s = param.Split('.').LastOrDefault();
            if (s == null) return "";
            return s.FirstLetterToUpper();
        }

        public static bool IsJsonArray(this string json)
        {
            var jToken = JToken.Parse(json);

            if (jToken is JArray)
                return true;

            return false;
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null ||
                   token.Type == JTokenType.Array && !token.HasValues ||
                   token.Type == JTokenType.Object && !token.HasValues ||
                   token.Type == JTokenType.String && (token.ToString() == string.Empty || token.ToString() == "null") ||
                   token.Type == JTokenType.Null;
        }

        public static bool IsJson(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return false; }

            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        public static string AddTagAfterFirstParagraph(this string html, string tag)
        {
            var newHtml = html;
            if (newHtml == null) newHtml = "";
            var firstParagraphMatch = Regex.Matches(newHtml, "<p(s*[^>]*)>(.*?)</p>").Cast<Match>().FirstOrDefault();
            if (firstParagraphMatch != null)
            {
                var firstParagraph = firstParagraphMatch.ToString();
                var firstParagraphWithAd = firstParagraph + tag; //e.g tag = "<div id='gpt_mpu' style='margin: 20px;'></div>"
                newHtml = newHtml.ReplaceFirst(firstParagraph, firstParagraphWithAd);
                return newHtml;
            }
            return newHtml;
        }
    }
}
