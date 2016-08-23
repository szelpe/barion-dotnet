using System;
using System.Net;

namespace BarionClientLibrary.RetryPolicies
{
    public class LinearRetry : IRetryPolicy
    {
        private const int DefaultClientRetryCount = 3;
        private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromSeconds(30);

        private readonly TimeSpan _deltaBackoff;
        private readonly int _maximumAttempts;

        public LinearRetry()
            : this(DefaultClientBackoff, DefaultClientRetryCount)
        {
        }

        public LinearRetry(TimeSpan deltaBackoff, int maxAttempts)
        {
            _deltaBackoff = deltaBackoff;
            _maximumAttempts = maxAttempts;
        }

        public bool ShouldRetry(int currentRetryCount, HttpStatusCode statusCode, out TimeSpan retryInterval)
        {
            retryInterval = TimeSpan.Zero;

            if (((int)statusCode >= 200 && (int)statusCode < 500 && statusCode != HttpStatusCode.RequestTimeout)
                  || statusCode == HttpStatusCode.NotImplemented
                    || statusCode == HttpStatusCode.HttpVersionNotSupported)
            {
                return false;
            }

            retryInterval = _deltaBackoff;
            return currentRetryCount < _maximumAttempts;
        }

        public bool ShouldRetry(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            throw new NotImplementedException();
        }

        public IRetryPolicy CreateInstance()
        {
            return new LinearRetry(_deltaBackoff, _maximumAttempts);
        }
    }
}
