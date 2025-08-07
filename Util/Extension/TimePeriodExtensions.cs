using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;

namespace PubQuizBackend.Util.Extension
{
    public static class TimePeriodExtensions
    {
        public static DateTime GetStartDateUtc(this TimePeriod period)
        {
            var now = DateTime.UtcNow;

            return period switch
            {
                TimePeriod.Today => now.Date,
                TimePeriod.Week => now.Date.AddDays(-(int)now.DayOfWeek),
                TimePeriod.Month => DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc),
                TimePeriod.Year => DateTime.SpecifyKind(new DateTime(now.Year, 1, 1), DateTimeKind.Utc),
                TimePeriod.AllTime => DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
                _ => throw new DivineException()
            };
        }
    }
}
