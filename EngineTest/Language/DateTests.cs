using System;
using Engine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Language;
using Engine.Utility;

namespace EngineTest.Language
{
    public class DateTests
    {
        [TestClass]
        public class WhenCalculatingSchoolYear
        {
            [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void Should_throw_exception_month_too_small()
            {
                var calculator = new DateCalculator(0, 1);
            }

            [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void Should_throw_exception_on_day_too_large()
            {
                var calculator = new DateCalculator(Month.January, 32);
            }

            [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void Should_throw_exception_on_leap_day_too_large()
            {
                var calculator = new DateCalculator(Month.February, 30);
            }

            [TestMethod]
            public void Should_return_next_year_after_cutoff()
            {
                // June first is next year when today is after the cutoff
                var calculator = new DateCalculator(Month.July, 1);
                var date = calculator.SchoolYearDate(Month.June, 1, new DateTime(2000, 7, 1));
                Assert.AreEqual(2001, date.Year);
            }

            [TestMethod]
            public void Should_return_current_year_before_cutoff()
            {
                // June first is this year when today is before the cutoff
                var calculator = new DateCalculator(Month.July, 1);
                var date = calculator.SchoolYearDate(Month.June, 1, new DateTime(2000, 6, 30));
                Assert.AreEqual(2000, date.Year);
            }

            [TestMethod]
            public void Should_return_previous_year_before_cutoff()
            {
                // July first is last year when today is before the cutoff
                var calculator = new DateCalculator(Month.July, 1);
                var date = calculator.SchoolYearDate(Month.July, 1, new DateTime(2000, 6, 30));
                Assert.AreEqual(1999, date.Year);
            }

            [TestMethod]
            public void Should_return_current_year_after_cutoff()
            {
                // July first is this year when today is after the cutoff
                var calculator = new DateCalculator(Month.July, 1);
                var date = calculator.SchoolYearDate(Month.July, 1, new DateTime(2000, 7, 1));
                Assert.AreEqual(2000, date.Year);
            }
        }

        [TestClass]
        public class WhenComputingWeekdayOfMonth
        {
            private readonly IDateCalculator _calculator = new DateCalculator();

            [TestMethod]
            public void ShouldFindLastDayOfMonthOnALeapDay()
            {
                var expected = new DateTime(2016, 2, 29);
                var actual = _calculator.CardinalWeekday(WeekdayOfMonth.last, DayOfWeek.Monday, Month.February, 2016);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ShouldFindLastDayOfMonthInDecember()
            {
                var expected = new DateTime(2015, 12, 31);
                var actual = _calculator.CardinalWeekday(WeekdayOfMonth.last, DayOfWeek.Thursday, Month.December, 2015);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ShouldFindFirstDayOfMonthInJanuary()
            {
                var expected = new DateTime(2016, 1, 1);
                var actual = _calculator.CardinalWeekday(WeekdayOfMonth.first, DayOfWeek.Friday, Month.January, 2016);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ShouldFindFourthDayOfMonthInJanuary()
            {
                var expected = new DateTime(2016, 1, 27);
                var actual = _calculator.CardinalWeekday(WeekdayOfMonth.fourth, DayOfWeek.Wednesday, Month.January, 2016);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestClass]
        public class WhenParsingDateWithYear
        {
            private Model _model;
            private readonly DateTime _date1 = DateTime.Today;
            private readonly DateTime _date2 = new DateTime(2000, 1, 1);

            [TestInitialize]
            public void Setup()
            {
                var date1 = _date1.ToString("d-MMM-yyyy");
                var date2 = _date2.ToString("d/MMM/yyyy");
                var text = $@"ruleset Foo rule 100.0 require {date1} > {date2} else 'error'";
                var stream = text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build();
            }

            [TestMethod]
            public void Should_contain_correct_dates()
            {
                var date1 = _date1.ToString("yyyy-MM-dd");
                var date2 = _date1.ToString("yyyy-MM-dd");
                Assert.IsTrue(_model.Rules[0].Sql.Contains(date1));
                Assert.IsTrue(_model.Rules[0].Sql.Contains(date2));
            }
        }

        [TestClass]
        public class WhenParsingDate
        {
            private Model _model;
            private readonly DateTime _date1;
            private readonly DateTime _date2;
            private readonly StaticDateProvider _dateProvider;

            public WhenParsingDate()
            {
                _dateProvider = new StaticDateProvider(new DateTime(2000, 1, 1));
                _date1 = _dateProvider.Today;
                _date2 = new DateTime(2000, 1, 2);
            }

            [TestInitialize]
            public void Setup()
            {
                var date1 = _date1.ToString("d-MMM");
                var date2 = _date2.ToString("d/MMM");
                var text = $@"ruleset Foo rule 100.0 require {date1} > {date2} else 'error'";
                var stream = text.ToStream();
                var builder = new ModelBuilder(stream);
                _model = builder.Build(_dateProvider);
            }

            [TestMethod]
            public void Should_contain_correct_dates()
            {
                var date1 = _date1.ToString("yyyy-MM-dd");
                var date2 = _date1.ToString("yyyy-MM-dd");
                Assert.IsTrue(_model.Rules[0].Sql.Contains(date1));
                Assert.IsTrue(_model.Rules[0].Sql.Contains(date2));
            }
        }

        [TestClass]
        public class WhenTodayIsBeforeEndOfSchoolYear
        {
            private Model _model;

            [TestInitialize]
            public void Setup()
            {
                var text = $@"ruleset Foo rule 100.0 require 1-Jul < 1-Jun  else 'error'";
                var stream = text.ToStream();
                var dateProvider = new StaticDateProvider(new DateTime(2000, 6, 30));
                var builder = new ModelBuilder(stream);
                _model = builder.Build(dateProvider);
            }

            [TestMethod]
            public void July_should_be_before_June()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains("1999-07-01"));
                Assert.IsTrue(_model.Rules[0].Sql.Contains("2000-06-01"));
            }
        }

        [TestClass]
        public class WhenCalculatingACardinalDate
        {
            private Model _model;

            [TestInitialize]
            public void Setup()
            {
                const string text = "ruleset Foo rule 100.0 require 1-Jul < third Wednesday in July else 'error'";
                var stream = text.ToStream();
                var dateProvider = new StaticDateProvider(new DateTime(2000, 7, 1));
                var builder = new ModelBuilder(stream);
                _model = builder.Build(dateProvider);
            }

            [TestMethod]
            public void Should_insert_properly_formatted_sql()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains("'2000-07-19'"));
            }
        }

        [TestClass]
        public class WhenCalculatingADifferentialDate
        {
            private Model _model;

            [TestInitialize]
            public void Setup()
            {
                const string text = "ruleset Foo rule 100.0 require 1-Jul = 11 months before 1-Jul else 'error'";
                var stream = text.ToStream();
                var dateProvider = new StaticDateProvider(new DateTime(2000, 7, 1));
                var builder = new ModelBuilder(stream);
                _model = builder.Build(dateProvider);
            }

            [TestMethod]
            public void Should_insert_properly_formatted_sql()
            {
                Assert.IsTrue(_model.Rules[0].Sql.Contains("1999-08-01'"));
            }
        }
    }
}
