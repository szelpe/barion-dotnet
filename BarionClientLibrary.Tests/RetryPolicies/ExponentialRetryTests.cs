using BarionClientLibrary.RetryPolicies;
using System;
using System.Net;
using Xunit;

namespace BarionClientLibrary.Tests.RetryPolicies
{
    public class ExponentialRetryTests
    {
        [Theory]
        [InlineData(HttpStatusCode.OK, false)]
        [InlineData(HttpStatusCode.NoContent, false)]
        [InlineData(HttpStatusCode.NotFound, false)]
        [InlineData(HttpStatusCode.NotImplemented, false)]
        [InlineData(HttpStatusCode.HttpVersionNotSupported, false)]
        [InlineData(HttpStatusCode.Redirect, false)]
        [InlineData(HttpStatusCode.GatewayTimeout, true)]
        [InlineData(HttpStatusCode.RequestTimeout, true)]
        [InlineData(HttpStatusCode.InternalServerError, true)]
        public void ShouldRetry_ShouldReturnFalse_OnNonRetriableStatusCodes(HttpStatusCode httpStatusCode, bool expectedRetryResult)
        {
            var retry = new ExponentialRetry();

            TimeSpan retryInterval;
            Assert.Equal(expectedRetryResult, retry.ShouldRetry(0, httpStatusCode, out retryInterval));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public void ShouldRetry_ShouldReturn_IfRetryCountIsTooHigh(int currentRetryCount, bool expectedResult)
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(currentRetryCount, HttpStatusCode.RequestTimeout, out retryInterval));
        }

        [Theory]
        [InlineData(0, 3, 3)]
        [InlineData(1, 6.2, 7.8)]
        [InlineData(2, 12.6, 17.4)]
        public void ShouldRetry_ShouldReturn_ExponentiallyIncreasingRetryInterval(int currentRetryCount, int min, int max)
        {
            var retry = new ExponentialRetry(TimeSpan.FromSeconds(4), 3);

            TimeSpan retryInterval;
            retry.ShouldRetry(currentRetryCount, HttpStatusCode.RequestTimeout, out retryInterval);

            Assert.InRange(retryInterval.Seconds, min, max);
        }
    }
}
