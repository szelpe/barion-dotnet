using System;

namespace BarionClientLibrary.RetryPolicies
{
    public class LinearRetry : RetryPolicy
    {
        private const uint DefaultClientRetryCount = 3;
        private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromMilliseconds(500);

        private readonly TimeSpan _deltaBackoff;

        public LinearRetry()
            : this(DefaultClientBackoff, DefaultClientRetryCount)
        {
        }

        public LinearRetry(TimeSpan deltaBackoff, uint maxAttempts) : base(maxAttempts)
        {
            if (deltaBackoff.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(deltaBackoff), "deltaBackoff should not be negative.");

            _deltaBackoff = deltaBackoff;
        }

        protected override TimeSpan CalculateRetryInterval(uint currentRetryCount)
        {
            return _deltaBackoff;
        }

        /// <summary>
        /// Generates a new retry policy for the current request attempt.
        /// </summary>
        /// <returns>An <see cref="IRetryPolicy"/> object that represents the retry policy for the current request attempt.</returns>
        public override IRetryPolicy CreateInstance()
        {
            return new LinearRetry(_deltaBackoff, _maximumAttempts);
        }
    }
}
