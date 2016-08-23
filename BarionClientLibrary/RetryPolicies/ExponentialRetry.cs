using System;
using System.Net;

namespace BarionClientLibrary.RetryPolicies
{
    public class ExponentialRetry : IRetryPolicy
    {
        private const int DefaultClientRetryCount = 3;
        private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(120);
        private static readonly TimeSpan MinBackoff = TimeSpan.FromSeconds(3);

        private readonly TimeSpan _deltaBackoff;
        private readonly int _maximumAttempts;

        public ExponentialRetry()
            : this(DefaultClientBackoff, DefaultClientRetryCount)
        {
        }

        public ExponentialRetry(TimeSpan deltaBackoff, int maxAttempts)
        {
            _deltaBackoff = deltaBackoff;
            _maximumAttempts = maxAttempts;
        }

        /// <summary>
        /// Determines whether the operation should be retried and the interval until the next retry.
        /// </summary>
        /// <param name="currentRetryCount">An integer specifying the number of retries for the given operation. A value of zero signifies this is the first error encountered.</param>
        /// <param name="statusCode">The status code for the last operation.</param>
        /// <param name="retryInterval">A <see cref="TimeSpan"/> indicating the interval to wait until the next retry.</param>
        /// <returns><c>true</c> if the operation should be retried; otherwise, <c>false</c>.</returns>
        public bool ShouldRetry(int currentRetryCount, HttpStatusCode statusCode, out TimeSpan retryInterval)
        {
            retryInterval = TimeSpan.Zero;

            if (((int)statusCode >= 200 && (int)statusCode < 500 && statusCode != HttpStatusCode.RequestTimeout)
                  || statusCode == HttpStatusCode.NotImplemented
                    || statusCode == HttpStatusCode.HttpVersionNotSupported)
            {
                return false;
            }

            if (currentRetryCount < _maximumAttempts)
            {
                Random r = new Random();
                double increment = (Math.Pow(2, currentRetryCount) - 1) * r.Next((int)(_deltaBackoff.TotalMilliseconds * 0.8), (int)(_deltaBackoff.TotalMilliseconds * 1.2));
                retryInterval = (increment < 0) ?
                                    MaxBackoff :
                                    TimeSpan.FromMilliseconds(Math.Min(MaxBackoff.TotalMilliseconds, MinBackoff.TotalMilliseconds + increment));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the operation should be retried and the interval until the next retry.
        /// </summary>
        /// <param name="currentRetryCount">An integer specifying the number of retries for the given operation. A value of zero signifies this is the first error encountered.</param>
        /// <param name="lastException">An <see cref="Exception"/> object that represents the last exception encountered.</param>
        /// <param name="retryInterval">A <see cref="TimeSpan"/> indicating the interval to wait until the next retry.</param>
        /// <returns><c>true</c> if the operation should be retried; otherwise, <c>false</c>.</returns>
        public bool ShouldRetry(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a new retry policy for the current request attempt.
        /// </summary>
        /// <returns>An <see cref="IRetryPolicy"/> object that represents the retry policy for the current request attempt.</returns>
        public IRetryPolicy CreateInstance()
        {
            return new ExponentialRetry(_deltaBackoff, _maximumAttempts);
        }
    }
}
