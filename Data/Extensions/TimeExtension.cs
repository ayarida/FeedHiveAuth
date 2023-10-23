using System.Globalization;
using System.Xml;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Humanizer;

namespace Mangox.Data.Extensions
{
    public static class TimeExtensions
    {
        public static TimeSpan TimeSpanFromNow(this DateTime current)
        {
            var domaintime = DomainTime.Now();
            var difference = domaintime.Subtract(current);
            return difference;
        }

        public static string ToStandardFormat(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToStandardFormat() : "";
        }
        public static string ToStandardFormat(this DateTime date)
        {
            return date.ToStandardFormat(false, false);
        }
        public static string ToStandardFormat(this DateTime date, bool withTime)
        {
            return date.ToStandardFormat(withTime, false);
        }
        public static string ToDateRangeFormat(this DateTime date, bool withTime)
        {
            return date.ToDateRangeformat(withTime, withTime);
        }
        /// <summary>
        /// Returns yyyy-MM-dd format string value with time as HH:mm:ss format
        /// </summary>
        public static string ToStandardFormat(this DateTime? date, bool withTime)
        {
            return date.HasValue ? date.Value.ToStandardFormat(withTime, false) : "";
        }

        /// <summary>
        /// Returns yyyy-MM-dd format string value with time as HH:mm:ss format
        /// </summary>
        public static string ToStandardFormat(this DateTime? date, bool withTime, bool withSeconds)
        {
            return date.HasValue ? date.Value.ToStandardFormat(withTime, withSeconds) : "";
        }
        public static string ToStandardFormat(this DateTime date, bool withTime, bool withSeconds, bool withDate = true)
        {
            var time = withTime ? withSeconds ? " HH:mm:ss" : " HH:mm" : "";
            var format = withDate ? string.Concat("yyyy-MM-dd", time) : time;
            return date.ToString(format, CultureInfo.InvariantCulture);
        }
        public static string ToDateRangeformat(this DateTime date, bool withTime, bool withSeconds)
        {
            var time = withTime ? withSeconds ? " HH:mm:ss" : " HH:mm" : "";
            var format = string.Concat("yyyy/MM/dd", time);
            return date.ToString(format, CultureInfo.InvariantCulture);
        }

        public static string ToDayOfWeekFormat(this DateTime? date, string lang)
        {
            return date.HasValue ? date.Value.ToDayOfWeekFormat(lang) : "";
        }
        public static string ToDayOfWeekFormat(this DateTime date, string lang)
        {
            return date.ToDayOfWeekFormat(false, lang);
        }
        public static string ToDayOfWeekFormat(this DateTime? date, bool withTime, string lang)
        {
            return date.HasValue ? date.Value.ToDayOfWeekFormat(withTime, lang) : "";
        }

        public static string ToDayOfWeekFormat(this DateTime date, bool withTime, string lang)
        {
            var time = withTime ? "HH:mm" : "";
            var format = string.Concat("dddd ", time);
            return date.ToString(format, new CultureInfo(lang));
        }


        public static DateTime? AddTimezoneHours(this DateTime? utcdate, string timezone)
        {
            if (!utcdate.HasValue)
                return null;
            return utcdate.Value.AddTimezoneHours(timezone);
        }
        public static DateTime AddTimezoneHours(this DateTime utcdate, string timezone)
        {
            if (string.IsNullOrEmpty(timezone))
                return utcdate;

            var cst = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var offset = cst.GetUtcOffset(utcdate);
            return utcdate.AddHours(offset.Hours);
        }

        public static string ToFormat(this DateTime date, string format)
        {
            return date.ToString(format, CultureInfo.InvariantCulture);
        }
        public static long ToUnixTimespans(this DateTime date)
        {
            long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }
        public static string ToStructuredDataDate(this DateTime date)
        {
            if (!date.HasValue())
                return null;

            return date.ToIso8601Date();
        }

        public static string ToStructuredDataDuration(int? seconds)
        {
            if (seconds.HasValue())
                return ToIso8601Duration(seconds.Value);
            return null;
        }
        public static string ToStructuredDataDuration(this TimeSpan timeSpan)
        {
            return timeSpan.ToIso8601Duration();
        }
        public static string ToIso8601Date(this DateTime date)
        {
            return date.ToString("O");
        }

        public static string ToIso8601Duration(int seconds)
        {
            var timeSpan = new TimeSpan(0, 0, seconds);
            return timeSpan.ToIso8601Duration();
        }
        public static string ToIso8601Duration(this TimeSpan timeSpan)
        {
            return XmlConvert.ToString(timeSpan);
        }
    }
}
