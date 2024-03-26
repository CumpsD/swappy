namespace SwappyBot.Infrastructure
{
    using System;

    public static class IsEmptyDateExtension
    {
        public static bool IsEmptyDate(this DateTimeOffset? date)
        {
            if (date == null)
                return true;

            return date == DateTimeOffset.MinValue;
        }
    }
}