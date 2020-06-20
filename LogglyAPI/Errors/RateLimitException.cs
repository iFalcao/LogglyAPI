using System;

namespace LogglyAPI.Errors
{
    public class RateLimitException : Exception
    {
        public const string ERR_DEFAULT_MSG = "You may have hit the rate limit of the Loggly API.";

        public RateLimitException()
            : base(ERR_DEFAULT_MSG) { }
    }
}
