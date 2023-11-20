using System.Globalization;
using System.Collections.Specialized;
using FeedHiveAuth.Models;
using FeedHiveAuth.Data.Extensions;
using System.Dynamic;
using System.Collections;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Data.Extensions
{
    public static class SharedExtensions
    {

        public static TimeSpan ParseTimeSpan(this string time)
        {
            if (time.Length < 8)
                time = "00:" + time;

            TimeSpan.TryParse(time, CultureInfo.InvariantCulture, out var span);
            return span;
        }
        public static bool HasValue(this Guid guid)
        {
            return guid != Guid.Empty;
        }

        public static bool HasValue(this Guid? guid)
        {
            return guid.HasValue && guid.Value != Guid.Empty;
        }

        public static bool HasValue(this int? integer)
        {
            return integer.HasValue && integer.Value > 0;
        }

        public static bool HasValue(this int integer)
        {
            return integer > 0;
        }

        public static bool HasValue(this double? integer)
        {
            return integer.HasValue && integer.Value > 0;
        }

        public static bool HasValue(this double integer)
        {
            return integer > 0;
        }

        public static bool HasValue(this DateTime? date)
        {
            return date.HasValue;
        }

        public static bool HasValue(this DateTime date)
        {
            return date != DateTime.MinValue;
        }

        public static void AddRange<T>(this ICollection<T> destination,
            IEnumerable<T> source)
        {
            List<T> list = destination as List<T>;

            if (list != null)
            {
                list.AddRange(source);
            }
            else
            {
                foreach (T item in source)
                {
                    destination.Add(item);
                }
            }
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive || type == typeof(decimal) || type == typeof(string) || type == typeof(Guid);
        }

        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }


        public static bool In<T>(this T value, IEnumerable<T> array)
        {
            if (value == null)
            {
                return false;
            }

            return array.Any(x => x.Equals(value));
        }

        public static bool StartsWith(this string value, IEnumerable<string> array)
        {
            if (value == null)
            {
                return false;
            }

            return array.Any(x => x.StartsWith(value));
        }

        public static bool In(this string value, string arrayStr)
        {
            if (value == null || arrayStr == null)
            {
                return false;
            }

            return value.ToLower().In(arrayStr.ToLower().Split(',').Select(x => x.Trim()));
        }

        public static bool NotIn(this string value, string arrayStr)
        {
            if (value == null || arrayStr == null)
            {
                return false;
            }

            return value.NotIn(arrayStr.Split(',').Select(x => x.Trim()));
        }

        public static bool NotIn<T>(this T value, IEnumerable<T> array)
        {
            if (value == null)
            {
                return false;
            }

            return array.All(x => !x.Equals(value));
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static bool None<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool Empty<T>(this IEnumerable<T> source)
        {
            return source == null || source.None();
        }

        public static bool NotEmpty<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        public static bool ContainsAny(this string str, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (value.ContainsIgnoreCase(str))
                {
                    return true;
                }
            }

            return false;
        }

        public static string Display<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (source == null || source.ToList() == null)
            {
                return "";
            }

            return string.Join(separator, source);
        }

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            // note: creating a Random instance each call may not be correct for you,
            // consider a thread-safe static instance
            var r = new Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            var ix = r.Next(0, list.Count);
            return list.Count == 0 ? list.FirstOrDefault() : list[ix];
        }

        public static string CreateTempFile(this string original)
        {
            return Path.GetTempFileName().Replace(".tmp", Path.GetExtension(original));
        }


        public static T GetValue<P, T>(this Dictionary<P, T> dictionary, P key, T defaultValue)
        {
            if (dictionary.TryGetValue(key, out T value))
            {
                return value;
            }

            return defaultValue;
        }

        public static T Duplicate<T>(this T original) where T : ICloneable
        {
            return (T)original.Clone();
        }

        //public static T DeepClone<T>(this T original) where T : ISerializable
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new MemoryStream();
        //    using (stream)
        //    {
        //        formatter.Serialize(stream, original);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        return (T)formatter.Deserialize(stream);
        //    }
        //}
        //public static int GetTimeZoneOffset(this DateTime d)
        //{
        //    TimeZoneInfo cst = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        //    TimeSpan offset;
        //    if (d != null)
        //    {
        //        offset = cst.GetUtcOffset(d);
        //        return offset.Hours;
        //    }
        //    return 0;
        //}
        public static string ComputeHashForLongFile(this string filePath)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    byte[] buffer = new byte[32768];
                    long amount = stream.Length;

                    using (var hashAlgorithm = SHA1.Create())
                    {
                        while (amount > 0)
                        {
                            int bytesRead = stream.Read(buffer, 0,
                                (int)Math.Min(buffer.Length, amount));

                            if (bytesRead > 0)
                            {
                                amount -= bytesRead;
                                if (amount > 0)
                                {
                                    hashAlgorithm.TransformBlock(buffer, 0, bytesRead,
                                        buffer, 0);
                                }
                                else
                                {
                                    hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        }

                        byte[] hash = hashAlgorithm.Hash;

                        string hashValue = Convert.ToBase64String(hash);

                        return hashValue;
                    }
                }
            }
            catch (Exception exception)
            {
                throw;
                //TODO migrate Logger.Error(typeof(SharedExtensions), "Error while computing file hash", exception);
            }

            return string.Empty;
        }

        public static Guid ToGuid(this int value)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static Guid ToGuid(this long value)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static Guid ToGuid(this string value)
        {
            var bytes = new byte[16];
            if (!int.TryParse(value, out int intvalue))
            {
                return Guid.Empty;
            }

            BitConverter.GetBytes(intvalue).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static int ToInt(this Guid value)
        {
            var bytes = value.ToByteArray();
            return BitConverter.ToInt32(bytes, 0);
        }

        private static readonly List<string> BootstrapSizes = new List<string> { "lg", "md", "sm", "xs" };

        public static string BootstrapClass(this string settings, string defaultRatio)
        {
            if (string.IsNullOrEmpty(settings))
            {
                return "col-md-" + defaultRatio;
            }

            return
                settings.Split(',')
                    .Select((x, n) =>
                        x.IsNotNullOrEmpty() ? "col-" + BootstrapSizes[n] + "-" + x : "hidden-" + BootstrapSizes[n])
                    .Display(" ");
        }


        //TODo???
        public static string Serialize(this object obj, bool ignoreNull = true)
        {
            try
            {
                if (obj == null) return "";
                return JsonConvert.SerializeObject(obj,
                    new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include
                    });
            }
            catch (Exception ex)
            {

            }
            return "";
        }



        public static T TryCast<T>(this object obj) where T : new()
        {
            if (obj is T)
            {
                return (T)obj;
            }

            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (InvalidCastException)
            {
                return obj.Serialize().FromJson<T>();
            }
        }


        public static T EvaluateField<T>(this object obj, string fieldName)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(fieldName))
                {
                    return default;
                }

                var field = fieldName.ExtractField();
                var type = obj.GetType();

                var fields = field.Split('.');
                if (fields.Length > 2)
                {
                    field = fields.LastOrDefault();
                    var subfields = fields.ToList().SkipLast(1).Skip(1).ToList();
                    if (subfields.Any(x => x.Contains("[") && !x.EndsWith("]")))
                    {
                        var newSubfield = new StringBuilder();
                        var subfieldIndex = 0;
                        var newSubfieldIndex = subfieldIndex;
                        foreach (var subfield in fields.SkipLast(1).Skip(1).ToList())
                        {
                            if (subfield.Contains("[") && !subfield.EndsWith("]"))
                            {
                                newSubfieldIndex = subfieldIndex;
                                newSubfield.Append(subfield).Append(".");
                                subfields.Remove(subfield);
                            }
                            else if (!subfield.Contains("[") && subfield.EndsWith("]"))
                            {
                                newSubfield.Append(subfield);
                                subfields.Remove(subfield);
                                subfields.Insert(newSubfieldIndex, newSubfield.ToString());
                                newSubfield.Clear();
                            }
                            else if (newSubfield.Length > 0)
                            {
                                newSubfield.Append(subfield).Append(".");
                                subfields.Remove(subfield);
                            }
                            subfieldIndex++;
                        }
                    }
                    foreach (var subfield in subfields)
                    {
                        var subfieldname = subfield;
                        if (subfield.EndsWith("]"))
                        {
                            subfieldname = subfield.Split('[').FirstOrDefault();
                        }
                        var subproperty = type.GetProperty(subfieldname);
                        if (subproperty != null)
                        {
                            var subvalue = subproperty.GetValue(obj);
                            if (subfield.EndsWith("]"))
                            {
                                var itemcondition = subfield.Split('[').Skip(1).FirstOrDefault()?.TrimEnd(']');
                                if (int.TryParse(itemcondition, out var index))
                                {
                                    subvalue = (subvalue as IEnumerable<object>).ElementAt(index);
                                }
                                else
                                {
                                    var conditions = itemcondition.Split('=');
                                    var key = "Key";
                                    var value = itemcondition;
                                    if (conditions.Count() == 2)
                                    {
                                        key = conditions[0];
                                        value = conditions[1];
                                    }

                                    var a = (subvalue as IEnumerable<object>).FirstOrDefault(x =>
                                    {
                                        // todo: remove "Network"
                                        if (key.Contains("."))
                                        {
                                            var keyValue = x.EvaluateField<string>(subfieldname + "." + key);
                                            if (keyValue.IsNotNullOrEmpty())
                                            {
                                                return keyValue == value;
                                            }
                                        }
                                        else
                                        {
                                            var p = x.GetType().GetProperty(key) ?? x.GetType().GetProperty("Network");
                                            if (p != null)
                                            {
                                                return (string)p.GetValue(x) == value;
                                            }
                                        }

                                        return false;
                                    });
                                    subvalue = a;
                                }
                            }
                            if (subvalue != null)
                            {
                                obj = subvalue;
                                type = obj.GetType();
                            }
                        }
                    }
                }

                var fieldKey = field.GetFieldName();
                var property = type.GetProperty(fieldKey);
                if (property != null)
                {
                    return (T)property.GetValue(obj);
                }

                if (type == typeof(JObject))
                {
                    return ((JObject)obj).ValueFromJson(fieldKey, default(T));
                }
            }
            catch (Exception ex)
            {
                throw;
                //@TODO migrate
                // Logger.Error(typeof(SharedExtensions), "Failed to get field " + fieldName + " from object", ex);
            }
            return default;
        }

        //public static string ServiceUrl(this ApplicationEnum app, Guid subscriptionId, bool? privatee = null,
        //   bool addprotocol = true, bool debugging = false)
        //{
        //    var configs = AppConfigs.InstanceOf(subscriptionId)?.Services;
        //    if (configs == null)
        //    {
        //        return "";
        //    }

        //    var service = configs.Services.FirstOrDefault(x => x.Type == app.Value().ToString() && x.Canonical == true) ?? configs.Services.FirstOrDefault(x => x.Type == app.Value().ToString());
        //    if (service == null)
        //    {
        //        return "";
        //    }

        //    if (privatee == null)
        //    {
        //        privatee = debugging;
        //    }

        //    //return privatee.Value
        //    //    ? service.PrivateUrl.ComposeSafeUrl(!addprotocol ? (bool?)null : service.EnableSsl && !debugging) + "/"
        //    //    : service.PublicUrl.ComposeSafeUrl(!addprotocol ? (bool?)null : service.EnableSsl && !debugging) + "/";


        //    return privatee.Value
        //        ? service.PrivateUrl.ComposeSafeUrl(!addprotocol ? null : service.EnableSsl) + "/"
        //        : service.PublicUrl.ComposeSafeUrl(!addprotocol ? null : service.EnableSsl) + "/";
        //}
    }
}
