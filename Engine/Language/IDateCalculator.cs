using System;

namespace Engine.Language
{
    public enum WeekdayOfMonth
    {
        // ReSharper disable InconsistentNaming
        last = 0,
        first,
        second,
        third,
        fourth,
        // ReSharper restore InconsistentNaming
    }

    public enum Month
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public interface IDateCalculator
    {
        DateTime SchoolYearDate(Month month, int day, DateTime asOfDate);
        DateTime CardinalWeekday(WeekdayOfMonth weekdayOfMonth, DayOfWeek dayOfWeek, Month month, int year);
        DateTime CardinalWeekday(string weekdayOfMonth, string dayOfWeek, string month, int year);
    }

    public class DateCalculator : IDateCalculator
    {
        private readonly Month _rolloverMonth;
        private readonly int _rolloverDay;

        public DateCalculator() : this(Month.July, 1)
        {
        }

        /// <param name="month">The rollover month</param>
        /// <param name="day">The rollover day</param>
        public DateCalculator(Month month, int day)
        {
            switch (month)
            {
                case Month.January:
                case Month.March:
                case Month.May:
                case Month.July:
                case Month.August:
                case Month.October:
                case Month.December:
                    if (day < 0 || day > 31)
                        throw new ArgumentOutOfRangeException(nameof(day), "value should be between 1 and 31");
                    break;
                case Month.February:
                    if (day < 0 || day > 29)
                        throw new ArgumentOutOfRangeException(nameof(day), "value should be between 1 and 29");
                    break;
                case Month.April:
                case Month.June:
                case Month.September:
                case Month.November:
                    if (day < 0 || day > 30)
                        throw new ArgumentOutOfRangeException(nameof(day), "value should be between 1 and 30");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(month), month, null);
            }
            _rolloverMonth = month;
            _rolloverDay = day;
        }

        public DateTime SchoolYearDate(Month month, int day, DateTime asOfDate)
        {
            var year = asOfDate.Year;
            if ((Month)asOfDate.Month > _rolloverMonth || ((Month)asOfDate.Month == _rolloverMonth && asOfDate.Day >= _rolloverDay))
            {
                if (month < _rolloverMonth || (month == _rolloverMonth && day < _rolloverDay)) year++;
            }
            else
            {
                if (month > _rolloverMonth || (month == _rolloverMonth && day >= _rolloverDay)) year--;
            }
            return new DateTime(year, (int)month, day);
        }

        public DateTime CardinalWeekday(WeekdayOfMonth weekdayOfMonth, DayOfWeek dayOfWeek, Month month, int year)
        {
            var date = new DateTime(year, (int)month, 1);
            switch (weekdayOfMonth)
            {
                case WeekdayOfMonth.last:
                    date = date.AddMonths(1);
                    do
                    {
                        date = date.AddDays(-1);
                    }
                    while (date.DayOfWeek != dayOfWeek);
                    return date;
                case WeekdayOfMonth.first:
                case WeekdayOfMonth.second:
                case WeekdayOfMonth.third:
                case WeekdayOfMonth.fourth:
                    while (date.DayOfWeek != dayOfWeek)
                    {
                        date = date.AddDays(1);
                    }
                    return date.AddDays(7 * ((int)weekdayOfMonth - 1));
                default:
                    throw new ArgumentOutOfRangeException(nameof(weekdayOfMonth), weekdayOfMonth, null);
            }
        }

        public DateTime CardinalWeekday(string strWeekdayOfMonth, string strDayOfWeek, string strMonth, int year)
        {
            var weekdayOfMonth = (WeekdayOfMonth)Enum.Parse(typeof(WeekdayOfMonth), strWeekdayOfMonth);
            var dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), strDayOfWeek);
            var month = (Month)Enum.Parse(typeof(Month), strMonth);
            return CardinalWeekday(weekdayOfMonth, dayOfWeek, month, year);
        }
    }
}
