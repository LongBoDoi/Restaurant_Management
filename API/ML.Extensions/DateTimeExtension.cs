using API.ML.Common;

namespace API.ML.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime GetFromDateByTimeFilter(this DateTime date, EnumTimeFilter timeFilter)
        {
            switch (timeFilter)
            {
                case EnumTimeFilter.ByDay:
                    return date.AddDays(-1);
                case EnumTimeFilter.ByWeek:
                    return date.AddDays(-7);
                case EnumTimeFilter.ByMonth:
                    return date.AddMonths(-1);
                case EnumTimeFilter.ByYear:
                    return date.AddYears(-1);
                default:
                    return date;
            }
        }

        public static DateTime GetToDateByTimeFilter(this DateTime date, EnumTimeFilter timeFilter)
        {
            switch (timeFilter)
            {
                case EnumTimeFilter.ByDay:
                    return date.AddDays(1);
                case EnumTimeFilter.ByWeek:
                    int dayOfWeek = (int)date.DayOfWeek;
                    return date.AddDays(dayOfWeek == 0 ? 1 : 8 - dayOfWeek);
                case EnumTimeFilter.ByMonth:
                    DateTime nextMonthDate = date.AddMonths(1);
                    return new DateTime(nextMonthDate.Year, nextMonthDate.Month, 1, 0, 0, 0);
                case EnumTimeFilter.ByYear:
                    return new DateTime(date.Year + 1, 1, 1, 0, 0, 0);
                default:
                    return date;
            }
        }

        public static DateTime ApplyTimeZone(this DateTime date, int timeZone)
        {
            return date.AddHours(timeZone * -1);
        }
    }
}
