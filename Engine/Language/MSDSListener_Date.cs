using System;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace Engine.Language
{
    public partial class MsdsListener
    {
        public const string SqlDateFormat = "yyyy-MM-dd";

        public override void ExitToday([NotNull] MsdsParser.TodayContext context)
        {
            var date = DateProvider.Today;
            _value.Put(context, date);
            var strDate = date.ToString(SqlDateFormat);
            _sql.Put(context, $"'{strDate}'");
        }

        /// <summary>
        /// A month and day of the current school year (i.e. July 1 through June 30)
        /// </summary>
        public override void ExitDayMonth([NotNull] MsdsParser.DayMonthContext context)
        {
            DateTime tmpDate;
            if (!DateTime.TryParse(context.DATE1().GetText(), out tmpDate)) return;
            var date = DateCalculator.SchoolYearDate((Month)tmpDate.Month, tmpDate.Day, DateProvider.Today);
            _value.Put(context, date);
            var strDate = date.ToString(SqlDateFormat);
            _sql.Put(context, $"'{strDate}'");
        }

        /// <summary>
        /// A fully defined date (i.e. July 1, 2000)
        /// </summary>
        public override void ExitDayMonthYear([NotNull] MsdsParser.DayMonthYearContext context)
        {
            DateTime date;
            if (!DateTime.TryParse(context.DATE2().GetText(), out date)) return;
            _value.Put(context, date);
            var strDate = date.ToString(SqlDateFormat);
            _sql.Put(context, $"'{strDate}'");
        }

        /// <summary>
        /// a value representing a date that must occur on a given day (i.e. the third Thursday in November)
        /// </summary>
        public override void ExitCardinalDate([NotNull] MsdsParser.CardinalDateContext context)
        {
            var dayOfWeek = context.CARDINAL().GetText();
            var weekday = context.WEEKDAY().GetText();
            var month = context.MONTH().GetText();
            var year = DateProvider.Today.Year;
            var date = DateCalculator.CardinalWeekday(dayOfWeek, weekday, month, year);
            _value.Put(context, date);
            var strDate = date.ToString(SqlDateFormat);
            _sql.Put(context, $"'{strDate}'");
        }

        /// <summary>
        /// basic SQL-based additive or negative date arithmetic (i.e. 5 days before 1-March)
        /// </summary>
        public override void ExitDifferentialDate([NotNull] MsdsParser.DifferentialDateContext context)
        {
            var date = (DateTime)_value.Get(context.date());
            var before = context.BEFORE() != null;
            var value = int.Parse(context.@int().GetText()) * (before ? -1 : 1);
            var datepart = context.TIMEUNIT().GetText().TrimEnd('s');
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (datepart)
            {
                case "day":
                    date = date.AddDays(value);
                    break;
                case "week":
                    date = date.AddDays(7 * value);
                    break;
                case "month":
                    date = date.AddMonths(value);
                    break;
                case "year":
                    date = date.AddYears(value);
                    break;
            }
            _value.Put(context.parent, date);
            var strDate = date.ToString(SqlDateFormat);
            _sql.Put(context, $"'{strDate}'");
        }

        public override void ExitDateoperation([NotNull] MsdsParser.DateoperationContext context)
        {
            var data = new
            {
                operation = context.DATEOP().GetText() == "earliest" ? "MIN" : "MAX",
                values = context.date().Select(x => _sql.Get(x))
            };
            var ruleSql = _engine.Generate("DateOperation", data);
            _sql.Put(context, ruleSql);
        }
    }
}
