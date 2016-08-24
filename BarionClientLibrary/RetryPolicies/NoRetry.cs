using System;
using System.Net;

namespace BarionClientLibrary.RetryPolicies
{
    public class NoRetry : IRetryPolicy
    {
        public bool ShouldRetry(uint currentRetryCount, HttpStatusCode statusCode, out TimeSpan retryInterval)
        {
            retryInterval = TimeSpan.Zero;
            return false;
        }

        public bool ShouldRetry(uint currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            retryInterval = TimeSpan.Zero;
            return false;
        }

        public IRetryPolicy CreateInstance()
        {
            return new NoRetry();
        }
    }
}
