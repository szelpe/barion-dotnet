using BarionClientLibrary.RetryPolicies;
using System;
using System.Net;

namespace BarionClientLibrary.Tests
{
    public class TestRetryPolicy : IRetryPolicy
    {
        public IRetryPolicy CreateInstance()
        {
            return this;
        }

        public Func<uint, HttpStatusCode, bool> ShouldRetryMock { get; set; }

        public Func<uint, Exception, bool> ShouldRetryExceptionMock { get; set; }

        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(2);

        public bool ShouldRetry(uint currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            retryInterval = RetryInterval;

            if (ShouldRetryExceptionMock == null)
                return false;

            return ShouldRetryExceptionMock(currentRetryCount, lastException);
        }

        public bool ShouldRetry(uint currentRetryCount, HttpStatusCode statusCode, out TimeSpan retryInterval)
        {
            retryInterval = RetryInterval;

            if (ShouldRetryMock == null)
                return false;

            return ShouldRetryMock(currentRetryCount, statusCode);
        }
    }
}
