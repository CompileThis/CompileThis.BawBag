namespace CompileThis.BawBag.Jabbr
{
    using System;

    public interface IDateTimeProvider
    {
        DateTimeOffset GetNow();

        DateTimeOffset GetUtcNow();

        DateTime GetToday();

        TimeSpan GetTimeOfDay();
    }
}