namespace CompileThis.BawBag.Jabbr
{
    using System;

    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now;
        }

        public DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }

        public DateTime GetToday()
        {
            return DateTime.Today;
        }

        public TimeSpan GetTimeOfDay()
        {
            return DateTimeOffset.Now.TimeOfDay;
        }
    }
}
