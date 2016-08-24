using BarionClientLibrary.RetryPolicies;
using System;
using System.Net;
using Xunit;

namespace BarionClientLibrary.Tests.RetryPolicies
{
    public class LinearRetryTests
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
            var retry = new LinearRetry();

            TimeSpan retryInterval;
            Assert.Equal(expectedRetryResult, retry.ShouldRetry(0, httpStatusCode, out retryInterval));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public void ShouldRetry_ShouldReturn_IfRetryCountIsTooHigh(uint currentRetryCount, bool expectedResult)
        {
            var retry = new LinearRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(currentRetryCount, HttpStatusCode.RequestTimeout, out retryInterval));
        }

        [Theory]
        [InlineData(0, 4)]
        [InlineData(1, 4)]
        [InlineData(2, 4)]
        public void ShouldRetry_ShouldReturn_LinearRetryInterval(uint currentRetryCount, int expectedRetryInterval)
        {
            var retry = new LinearRetry(TimeSpan.FromSeconds(4), 3);

            TimeSpan retryInterval;
            retry.ShouldRetry(currentRetryCount, HttpStatusCode.RequestTimeout, out retryInterval);

            Assert.Equal(expectedRetryInterval, retryInterval.Seconds);
        }

        [Fact]
        public void ShouldThrowException_OnNegativeDeltaBackoff()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LinearRetry(TimeSpan.FromSeconds(-4), 3));
        }
    }
}
