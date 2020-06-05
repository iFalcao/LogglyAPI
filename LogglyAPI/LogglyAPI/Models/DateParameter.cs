using System;

namespace LogglyAPI.Models
{
    public class DateParameter
    {
        public static DateParameter DEFAULT_FROM = new DateParameter("-24h");
        public static DateParameter DEFAULT_UNTIL = new DateParameter("now");

        private DateParameter(string query)
        {
            this.Query = query;
        }

        public string Query { get; private set; }


        public DateParameter FromTimeInterval(TimeInterval interval, int timeValue)
        {
            var absoluteValue = Math.Abs(timeValue);
            return new DateParameter($"-{absoluteValue}{(char)interval}");
        }
    }

    public enum TimeInterval
    {
        Seconds = 's',
        Minutes = 'm',
        Hours = 'h',
        Days = 'd',
        Weeks = 'w'
    }
}
