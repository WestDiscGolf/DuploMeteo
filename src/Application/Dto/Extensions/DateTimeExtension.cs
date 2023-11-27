using Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// The API does not return the timezone with any specific markings as to what its timezone is or isn't
        /// Ex: 2023-11-07T00:00. However, it does return the TimeZone for which the time was calculated. As such, we can convert this
        /// to a UTC time instead per best practice of storing dates in a DB.
        /// </summary>
        /// <param name="times"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public static List<DateTime> ConvertStringTimesUsingTimezone(this IEnumerable<string> times, string timezone)
        {
            if(times == null)
                return new List<DateTime>();

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var convertedTimes = times.Select(x =>
            {
                var dt = DateTime.Parse(x, CultureInfo.InvariantCulture);

                var date = TimeZoneInfo.ConvertTimeToUtc(dt, tz);
                return date;

            }).ToList();

            return convertedTimes;

        }
    }
}
