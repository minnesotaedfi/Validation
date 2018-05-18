using System;

namespace Engine.Language
{
    public interface IDateProvider
    {
        DateTime Today { get; }
    }

    public class DateProvider : IDateProvider
    {
        public DateTime Today => DateTime.Today;
    }

    public class StaticDateProvider : IDateProvider
    {
        public DateTime Today { get; }

        public StaticDateProvider(DateTime today)
        {
            Today = today.Date;
        }
    }
}