using System;

namespace BarionClientLibrary.RetryPolicies
{
    public class ExponentialRetry : RetryPolicy
    {
        private const uint DefaultClientRetryCount = 3;
        private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(120);
        private static readonly TimeSpan MinBackoff = TimeSpan.FromSeconds(3);

        private readonly TimeSpan _deltaBackoff;

        public ExponentialRetry()
            : this(DefaultClientBackoff, DefaultClientRetryCount)
        {
        }

        public ExponentialRetry(TimeSpan deltaBackoff, uint maxAttempts) : base(maxAttempts)
        {
            if (deltaBackoff.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(deltaBackoff), "deltaBackoff should not be negative.");

            _deltaBackoff = deltaBackoff;
        }

        protected override TimeSpan CalculateRetryInterval(uint currentRetryCount)
        {
            Random r = new Random();
            double increment = (Math.Pow(2.0, currentRetryCount) - 1.0) * r.Next((int)(_deltaBackoff.TotalMilliseconds * 0.8), (int)(_deltaBackoff.TotalMilliseconds * 1.2));
            var retryInterval = Math.Min(MaxBackoff.TotalMilliseconds, checked(MinBackoff.TotalMilliseconds + increment));

            return TimeSpan.FromMilliseconds(retryInterval);
        }

        /// <summary>
        /// Generates a new retry policy for the current request attempt.
        /// </summary>
        /// <returns>An <see cref="IRetryPolicy"/> object that represents the retry policy for the current request attempt.</returns>
        public override IRetryPolicy CreateInstance()
        {
            return new ExponentialRetry(_deltaBackoff, _maximumAttempts);
        }
    }
}
