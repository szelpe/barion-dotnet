using System;

namespace BarionClientLibrary.RetryPolicies
{
    /// <summary>
    /// Defines a retry policy with fixed time intervals between retries.
    /// </summary>
    public class LinearRetry : RetryPolicy
    {
        private const uint DefaultClientRetryCount = 3;
        private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromMilliseconds(500);

        private readonly TimeSpan _deltaBackoff;

        /// <summary>
        /// Initializes a new instance of the BarionClientLibrary.RetryPolicies.LinearRetry class.
        /// </summary>
        public LinearRetry()
            : this(DefaultClientBackoff, DefaultClientRetryCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BarionClientLibrary.RetryPolicies.LinearRetry class.
        /// </summary>
        /// <param name="deltaBackoff">A <see cref="TimeSpan"/> specifying the back-off interval between retries.</param>
        /// <param name="maxAttempts">The maximum number of retry attempts.</param>
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
