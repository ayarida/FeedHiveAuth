/*using System.Globalization;
using System.Xml;
using FeedHiveAuth.Data.Extensions;
using FeedHiveAuth.Models;
using Humanizer;

namespace Mangox.Data.Extensions
{
    public static class TimeExtensions
    {
        public static bool IsToday(this DateTime current)
        {
            return current.Midnight().CompareTo(DomainTime.Now().Midnight()) == 0;
        }
        public static bool IsToday(this DateTime? current)
        {
            return current.HasValue && current.Value.Midnight().CompareTo(DomainTime.Now().Midnight()) == 0;
        }

        public static int CountDaysFromNow(this DateTime current)
        {
            return (DomainTime.Now().BeginningOfDay() - current.BeginningOfDay()).Days;
        }
        private static string IncludeFromCluase(this string date, string lang, Guid subscriptionId)
        {
            return lang == "ar" ? TimeLabelsEnum.From.DisplayName(lang, subscriptionId) + " " + date : date + " " + TimeLabelsEnum.Ago.DisplayName(lang, subscriptionId); ;
        }
        public static string MangopulseDisplayFormat(this DateTime date, string lang, string timeZonestring, int timezoneAdditionalDays, Guid subscriptionId, string splitter = " ")
        {
            if (string.IsNullOrEmpty(timeZonestring))
                return date.ToString();
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZonestring);
            var offset = timezone.GetUtcOffset(date.AddDays(timezoneAdditionalDays));

            var monthName = GetMonthName(date.Month, lang, subscriptionId);
            var timePeriod = date.GetTimePeriod(lang, subscriptionId);
            var timespan = date.TimeSpanFromNow();
            if (date.IsToday())
            {
                if (timespan.TotalSeconds < 60)
                {
                    return timespan.Seconds.GetNbSecondsDisplay(lang, subscriptionId).IncludeFromCluase(lang, subscriptionId);
                }
                if (timespan.TotalMinutes < 60)
                    return timespan.Minutes.GetNbOfMinutesDisplay(lang, subscriptionId).IncludeFromCluase(lang, subscriptionId);
                if (timespan.Hours <= 6)
                    return timespan.Hours.GetNbOfHoursDisplay(lang, subscriptionId).IncludeFromCluase(lang, subscriptionId);
                return string.Concat(date.AddHours(offset.TotalHours).ToString("hh:mm "), splitter, " ", timePeriod);
            }
            if (timespan.TotalDays <= 7)
            {
                return date.CountDaysFromNow().GetNbOfDaysDisplay(lang, subscriptionId).IncludeFromCluase(lang, subscriptionId); ;//ok
            }
            if (date.Year != DomainTime.Now().Year)
            {
                return date.Day + " " + monthName + splitter + date.ToString("yyyy");
            }
            return date.Day + splitter + monthName;
        }
        public static TimeSpan TimeSpanFromNow(this DateTime current)
        {
            var domaintime = DomainTime.Now();
            var difference = domaintime.Subtract(current);
            return difference;
        }

        public static string ToNamedFormat(this DateTime? date, string lang, Guid subscriptionId, bool showAll = true, string splitter = " ")
        {
            var emtpyDate = "---------";
            if (date.HasValue)
                return date.Value.ToNamedFormat(lang, subscriptionId, showAll, splitter);
            return emtpyDate;
        }

        public static string MinimalNamedDate(this DateTime? date, string lang, Guid subscriptionId, bool showTime = false, bool showDate = true)
        {
            if (date == null) return "";
            return date.Value.MinimalNamedDate(lang, subscriptionId, showTime, showDate);
        }
        public static string MinimalNamedDate(this DateTime date, string lang, Guid subscriptionId, bool showTime = false, bool showDate = true)
        {
            var utcdate = DomainTime.Now();
            var year = date.Year != utcdate.Year;
            var month = date.Day != utcdate.Day || date.Month != utcdate.Month || date.Year != utcdate.Year;
            var day = date.Day != utcdate.Day || date.Month != utcdate.Month || date.Year != utcdate.Year;
            var today = date.Day == utcdate.Day && date.Month == utcdate.Month && date.Year == utcdate.Year;

            //var time = showTime || today;
            if (today)
            {
                return showTime ? date.ToString("HH:mm") : "";// TimeLabelsEnum.Today.DisplayName(lang);
            }
            var pattern = showDate ? "{0}{1}{2}{3}" : "{3}";
            var format = string.Format(pattern, day ? "dd " : "", month ? "[]" + " " : "", year ? " yyyy" : "", showTime ? " HH:mm" : "");
            return date.ToString(format).Replace("[]", GetMonthName(date.Month, lang, subscriptionId));
        }
        public static string ToNamedFormat(this DateTime date, string lang, Guid subscriptionId, bool showDay = true, string splitter = " ", bool onlydate = false, bool onlytime = false, bool showToday = true, bool alwaysshowyear = false, bool timeOfTodayOnly = false)
        {
            var emtpyDate = "";
            var utcdate = DomainTime.Now();
            var today = date.Day == utcdate.Day && date.Month == utcdate.Month && date.Year == utcdate.Year;
            var monthName = GetMonthName(date.Month, lang, subscriptionId);
            var timePeriod = date.ToString("HH:mm ");// + GetTimePeriod(date, lang);
            var formatted = string.Concat(date.Day, splitter, monthName);

            if (timeOfTodayOnly && !today)
            {
                timePeriod = "";
            }

            if (date.Date == DomainTime.Now().Date && showToday)
            {
                if (onlydate)
                    return TimeLabelsEnum.Today.DisplayName(lang, subscriptionId);

                if (onlytime)
                    return timePeriod;

                //    return string.Concat(Strings.Today, splitter, date.ToString("hh:mm:ss "), timePeriod);
                return string.Concat(TimeLabelsEnum.Today.DisplayName(lang, subscriptionId), splitter, timePeriod);
            }

            if (showDay)
            {
                formatted = date.DayOfWeek.DisplayName(lang, subscriptionId) + " " + formatted;
            }
            if (date.Year != DomainTime.Now().Year || alwaysshowyear)
            {
                //format = string.Concat(format, splitter, "yyyy");
                formatted = string.Concat(formatted, splitter, date.ToString("yyyy"));
            }

            if (date != date.Midnight())
            {
                if (onlytime)
                    formatted = timePeriod;
                else if (!onlydate)
                    //format = string.Concat(format, ",", splitter, "hh:mm:ss ", timePeriod);
                    // formatted = string.Concat(formatted, ",", splitter, date.ToString("hh:mm:ss "), timePeriod);
                    formatted = string.Concat(formatted, splitter, timePeriod);

            }

            return date == DateTime.MinValue ? emtpyDate :
                    //date.ToString(format, System.Globalization.CultureInfo.InvariantCulture)
                    formatted
                ;
        }


        private static string GetMonthName(int month, string lang, Guid subscriptionId)
        {
            var name = "MMM";
            switch (month)
            {
                case 1:
                    name = MonthEnum.Jan.DisplayName(lang, subscriptionId);
                    break;
                case 2:
                    name = MonthEnum.Feb.DisplayName(lang, subscriptionId);
                    break;
                case 3:
                    name = MonthEnum.Mar.DisplayName(lang, subscriptionId);
                    break;
                case 4:
                    name = MonthEnum.Apr.DisplayName(lang, subscriptionId);
                    break;
                case 5:
                    name = MonthEnum.May.DisplayName(lang, subscriptionId);
                    break;
                case 6:
                    name = MonthEnum.Jun.DisplayName(lang, subscriptionId);
                    break;
                case 7:
                    name = MonthEnum.Jul.DisplayName(lang, subscriptionId);
                    break;
                case 8:
                    name = MonthEnum.Aug.DisplayName(lang, subscriptionId);
                    break;
                case 9:
                    name = MonthEnum.Sep.DisplayName(lang, subscriptionId);
                    break;
                case 10:
                    name = MonthEnum.Oct.DisplayName(lang, subscriptionId);
                    break;
                case 11:
                    name = MonthEnum.Nov.DisplayName(lang, subscriptionId);
                    break;
                case 12:
                    name = MonthEnum.Dec.DisplayName(lang, subscriptionId);
                    break;
            }
            return name;
        }

        public static string GetTimePeriod(this DateTime date, string lang, Guid subscriptionId)
        {
            return date.IsBeforeNoon() ? TimeLabelsEnum.AM.DisplayName(lang, subscriptionId) : TimeLabelsEnum.PM.DisplayName(lang, subscriptionId);
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
        public static string ToSmartFormat(this DateTime? date, string lang, Guid subscriptionId, int numOfDays = 1)
        {
            var emtpyDate = "#";// "Unknown";
            if (date.HasValue && date.Value < DomainTime.Now())
                return date.Value.ToSmartFormat(lang, subscriptionId, numOfDays);
            return emtpyDate;
        }
        public static string ToSmartFormat(this DateTime date, string lang, Guid subscriptionId, int numOfDays = 1, bool showDay = false, bool showYear = false, bool showTime = true)
        {
            var emtpyDate = "---------";
            var splitter = " ";

            //var format = string.Concat("d", splitter, "MMM");

            var monthName = "MMM";
            monthName = GetMonthName(date.Month, lang, subscriptionId);
            var timePeriod = date.GetTimePeriod(lang, subscriptionId);
            //var format = string.Concat("d", splitter, monthName);
            var formatted = string.Concat(date.Day, splitter, monthName);
            if (date.Date >= DomainTime.Now().Date.AddDays(-numOfDays))
            {
                var timespan = date.Ago();
                return timespan.GetTimeInfo(lang, subscriptionId);
            }
            if (showDay)
            {
                formatted = string.Concat(((DayOfWeekEnum)DomainTime.Now().DayOfWeek).DisplayName("ar", subscriptionId), splitter, formatted);
            }
            if (date.Year != DomainTime.Now().Year)
            {
                //format = string.Concat(format, splitter, "yyyy");
                formatted = string.Concat(formatted, splitter, date.ToString("yyyy"));
            }
            if (showYear)
            {
                formatted = string.Concat(formatted, splitter, date.ToString("yyyy"));
            }

            if (showTime && date != date.Midnight())
            {
                //format = string.Concat(format, ",", splitter, "hh:mm ", timePeriod);
                formatted = string.Concat(formatted, ",", splitter, date.ToString("hh:mm "), timePeriod);
            }

            return date == DateTime.MinValue
                       ? emtpyDate
                       : //date.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
                   formatted;
        }

        public static string GetTimeInfo(this TimeSpan timespan, string lang, Guid subscriptionId)
        {
            var message = "";
            if (timespan.TotalMinutes <= 1)
                message = TimeLabelsEnum.FewSeconds.DisplayName(lang, subscriptionId);// "Few seconds";
            else if (timespan.TotalMinutes <= 5)
                message = "5 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//"min.";
            else if (timespan.TotalMinutes <= 10)
                message = "10 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 15)
                message = "15 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 20)
                message = "20 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 25)
                message = "25 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 30)
                message = "30 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 40)
                message = "40 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 50)
                message = "50 " + TimeLabelsEnum.MinuteShort.DisplayName(lang, subscriptionId);//min.";
            else if (timespan.TotalMinutes <= 60)
                message = "1 " + TimeLabelsEnum.HourShort.DisplayName(lang, subscriptionId);
            else if (timespan.TotalMinutes <= 90)
                message = "1.5 " + TimeLabelsEnum.HourShort.DisplayName(lang, subscriptionId);
            else if (timespan.TotalMinutes <= 120)
                message = "2 " + TimeLabelsEnum.HourShort.DisplayName(lang, subscriptionId);
            else if (timespan.TotalMinutes <= 150)
                message = "2.5 " + TimeLabelsEnum.HourShort.DisplayName(lang, subscriptionId);
            else if (timespan.TotalMinutes <= 180)
                message = "3 " + TimeLabelsEnum.HourShort.DisplayName(lang, subscriptionId);
            else if (timespan.TotalHours >= 4 && timespan.TotalHours <= 24)
                message = string.Concat(Math.Ceiling(timespan.TotalHours), " " + TimeLabelsEnum.HourShort.DisplayName(lang, subscriptionId));

            else if (timespan.TotalHours >= 24)
                message = timespan.TotalDays.FormatDays(lang, subscriptionId);

            return message;
        }
        public static string FormatDays(this int value, string lang, Guid subscriptionId)
        {
            return ((double)value).FormatDays(lang, subscriptionId);
        }


        public static DateTime Round(this DateTime dateTime, RoundTo rt)
        {
            DateTime rounded;

            switch (rt)
            {
                case RoundTo.Second:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                        if (dateTime.Millisecond >= 500)
                        {
                            rounded = rounded.AddSeconds(1);
                        }
                        break;
                    }
                case RoundTo.Minute:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
                        if (dateTime.Second >= 30)
                        {
                            rounded = rounded.AddMinutes(1);
                        }
                        break;
                    }
                case RoundTo.Hour:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
                        if (dateTime.Minute >= 30)
                        {
                            rounded = rounded.AddHours(1);
                        }
                        break;
                    }
                case RoundTo.Day:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                        if (dateTime.Hour >= 12)
                        {
                            rounded = rounded.AddDays(1);
                        }
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            return rounded;
        }

        public static TimeSpan Round(this TimeSpan timeSpan, RoundTo rt)
        {
            TimeSpan rounded;

            switch (rt)
            {
                case RoundTo.Second:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                        if (timeSpan.Milliseconds >= 500)
                        {
                            rounded = rounded + 1.Seconds();
                        }
                        break;
                    }
                case RoundTo.Minute:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
                        if (timeSpan.Seconds >= 30)
                        {
                            rounded = rounded + 1.Minutes();
                        }
                        break;
                    }
                case RoundTo.Hour:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
                        if (timeSpan.Minutes >= 30)
                        {
                            rounded = rounded + 1.Hours();
                        }
                        break;
                    }
                case RoundTo.Day:
                    {
                        rounded = new TimeSpan(timeSpan.Days, 0, 0, 0);
                        if (timeSpan.Hours >= 12)
                        {
                            rounded = rounded + 1.Days();
                        }
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            return rounded;
        }

        public enum RoundTo
        {
            Second, Minute, Hour, Day
        }
        /// <summary>
        /// Convert a <see cref="TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>
        public static string ToDisplayString(this FluentTimeSpan timeSpan, string lang, Guid subscriptionId)
        {
            return ((TimeSpan)timeSpan).ToDisplayString(lang, subscriptionId);
        }



        /// <summary>
        /// Convert a <see cref="TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>
        public static string ToDisplayString(this TimeSpan timeSpan, string lang, Guid subscriptionId)
        {
            if (timeSpan.TotalDays > 1)
            {
                var round = timeSpan.Round(RoundTo.Hour);

                return string.Format("{0} {1} {2} {3}", round.Days.FormatDays(lang, subscriptionId), TimeLabelsEnum.And, round.Hours, TimeLabelsEnum.HourShort);
            }
            if (timeSpan.TotalHours > 1)
            {
                var round = timeSpan.Round(RoundTo.Minute);
                return string.Format("{0} {1} {2} {3} {4}", round.Hours, TimeLabelsEnum.HourShort, TimeLabelsEnum.And, round.Minutes, TimeLabelsEnum.MinuteShort);
            }
            if (timeSpan.TotalMinutes > 1)
            {
                var round = timeSpan.Round(RoundTo.Second);
                return string.Format("{0} {1} {2} {3} {4}", round.Minutes, TimeLabelsEnum.MinuteShort, TimeLabelsEnum.And, round.Seconds, TimeLabelsEnum.SecondsShort);
            }
            if (timeSpan.TotalSeconds > 1)
            {
                return string.Format("{0} {1}", timeSpan.TotalSeconds, TimeLabelsEnum.SecondsShort);
            }
            return string.Format("{0} {1}", timeSpan.Milliseconds, TimeLabelsEnum.Milliseconds);
        }
        public static string FormatDays(this double value, string lang, Guid subscriptionId)
        {
            value = Math.Ceiling(value);
            var message = "";
            if (value >= 1 && value < 2)
                message = TimeLabelsEnum.About.DisplayName(lang, subscriptionId) + " " + value + " " + TimeLabelsEnum.Day.DisplayName(lang, subscriptionId);
            else if (value >= 2 && value < 3)
                message = TimeLabelsEnum.About.DisplayName(lang, subscriptionId) + " " + value + " " + TimeLabelsEnum.TwoDays.DisplayName(lang, subscriptionId);
            else if (value >= 3 && value < 11)
                message = TimeLabelsEnum.About.DisplayName(lang, subscriptionId) + " " + value + " " + TimeLabelsEnum.Days.DisplayName(lang, subscriptionId);
            else if (value >= 11)
                message = TimeLabelsEnum.About.DisplayName(lang, subscriptionId) + " " + value + " " + TimeLabelsEnum.Day.DisplayName(lang, subscriptionId);

            return message;
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

        public static DateTime ConvertUnixToDateTime(this double unixTimestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(unixTimestamp);

            return dateTime;
        }

        public static DateTime? FromDate(this string daterange)
        {
            if (daterange.IsNotNullOrEmpty())
            {
                var dates = daterange.Split('-').Select(x => x.Trim());
                return DateTime.Parse(dates.ElementAt(0)).BeginningOfDay();
            }
            return null;
        }
        public static DateTime? ToDate(this string daterange)
        {
            if (daterange.IsNotNullOrEmpty())
            {
                var dates = daterange.Split('-').Select(x => x.Trim());
                return DateTime.Parse(dates.ElementAt(1)).EndOfDay();
            }
            return null;
        }
        public static string UtcFormat(this DateTime? date)
        {
            if (!date.HasValue) return "";
            return date.Value.UtcFormat();
        }
        public static string UtcFormat(this DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);
        }


        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static DateTime StartOfYear(this DateTime dt)
        {
            if (dt.Month == 1)
                dt.SetYear(dt.Year - 1);
            dt = dt.SetDay(1);
            dt = dt.SetMonth(1);
            return dt;
        }


        //todo: support client's timezone
        public static string IsoDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }


        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime thru)
        {
            for (var day = from; day.Date <= thru.EndOfMonth(); day = day.AddMonths(1))
                yield return day;
        }

        public static string DisplayFormatRevamp(DateTime? date, Guid subscriptionId, string dateFormat, bool showAll = false)
        {
            if (!date.HasValue) return "";

            Subscription subscription = Collections.Subscriptions.SingleOrDefault(x => x.Id == subscriptionId);
            return FormatDate(date, dateFormat, AppConfigs.InstanceOf(subscriptionId).LanguageConfigs.DefaultLanguage, subscriptionId);
        }

        public static string DisplayFormat(this DateTime? date, Guid subscriptionId, bool showAll = false)
        {
            if (!date.HasValue) return "";
            return date.Value.DisplayFormat(subscriptionId);
        }

        public static string DisplayFormat(this DateTime date, Guid subscriptionId)
        {
            var configs = AppConfigs.InstanceOf(subscriptionId);
            return date.MangopulseDisplayFormat(AppConfigs.InstanceOf(subscriptionId).LanguageConfigs.DefaultLanguage.Key, configs.Misc.Timezone, configs.Misc.TimezoneAdditionalDays, subscriptionId, ", ");
        }

        private static string FormatDate(DateTime? date, string format, Language lang, Guid subscriptionId, Subscription subscription = null)
        {
            if (subscription?.Code == "alitihad")
            {
                if (format == "details")
                {
                    format = "namedfull-datetimeyear";
                }
                else
                {
                    format = "named-datetimetodyear";
                }

            }

            //format: style-data. style: standard, named, minimal, long. data: datetime, date, time
            //examples: standard -datetime, standard-date, standard-time
            format = format.Nval("named-date");
            if (!date.HasValue)
            {
                return "";
            }

            var data = format.Split('-');
            if (data.Length < 2 && format.IsNotNullOrEmpty())
            {
                try
                {
                    return date.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    return date.MinimalNamedDate(lang.Key, subscriptionId, false);
                }
            }
            var includeDate = data[1].Contains("date");
            var includeTime = data[1].Contains("time");
            var includeday = data[1].Contains("day");
            var includeyear = data[1].Contains("year");
            var timeOfTodayOnly = data[1].Contains("tod");

            var time = includeTime ? "HH:mm" : "";
            var stdFormat = includeDate ? string.Concat("yyyy-MM-dd", " ", time) : time;
            var style = data[0].Nval("minimal");
            switch (style)
            {
                case "minimal":
                    return date.MinimalNamedDate(lang.Key, subscriptionId, includeTime, includeDate);
                case "standard":

                    return date.Value.ToString(stdFormat, System.Globalization.CultureInfo.InvariantCulture);
                case "named":
                    return date.Value.ToNamedFormat(lang.Key, subscriptionId, includeday, " ", !includeTime, !includeDate, true, includeyear, timeOfTodayOnly);
                case "namedfull":
                    return date.Value.ToNamedFormat(lang.Key, subscriptionId, includeday, " ", !includeTime, !includeDate, false, includeyear);
                case "now":
                    return DomainTime.Now().BeginningOfDay().ToSmartFormat(lang.Key, subscriptionId, -1, true, true, true);
            }
            return date.MinimalNamedDate(lang.Key, subscriptionId, false);
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
*/