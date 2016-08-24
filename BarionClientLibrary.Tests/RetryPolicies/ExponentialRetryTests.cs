using BarionClientLibrary.RetryPolicies;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
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
        public void ShouldRetry_ShouldReturn_IfRetryCountIsTooHigh(uint currentRetryCount, bool expectedResult)
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(currentRetryCount, HttpStatusCode.RequestTimeout, out retryInterval));
        }

        [Theory]
        [InlineData(WebExceptionStatus.Timeout, true)]
        [InlineData(WebExceptionStatus.ConnectionClosed, true)]
        [InlineData(WebExceptionStatus.SendFailure, true)]
        [InlineData(WebExceptionStatus.UnknownError, false)]
        public void ShouldRetry_ShouldReturn_OnWebExceptions(WebExceptionStatus status, bool expectedResult)
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(0, new WebException("", status), out retryInterval));
        }

        [Theory]
        [InlineData(WebExceptionStatus.Timeout, true)]
        [InlineData(WebExceptionStatus.ConnectionClosed, true)]
        [InlineData(WebExceptionStatus.SendFailure, true)]
        [InlineData(WebExceptionStatus.UnknownError, false)]
        public void ShouldRetry_ShouldReturn_OnHttpRequestExceptions(WebExceptionStatus status, bool expectedResult)
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(0, new HttpRequestException("", new WebException("", status)), out retryInterval));
        }

        [Theory]
        [InlineData(SocketError.ConnectionRefused, true)]
        [InlineData(SocketError.TimedOut, true)]
        [InlineData(SocketError.AccessDenied, false)]
        public void ShouldRetry_ShouldReturn_OnSocketException(int errorCode, bool expectedResult)
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(0, new SocketException(errorCode), out retryInterval));
        }

        [Theory]
        [InlineData(SocketError.ConnectionRefused, true)]
        [InlineData(SocketError.TimedOut, true)]
        [InlineData(SocketError.AccessDenied, false)]
        public void ShouldRetry_ShouldReturn_OnSocketException_InHttpRequestException(int errorCode, bool expectedResult)
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(expectedResult, retry.ShouldRetry(0, new HttpRequestException("", new SocketException(errorCode)), out retryInterval));
        }

        [Fact]
        public void ShouldRetry_ShouldReturn_OnTimeoutException()
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(true, retry.ShouldRetry(0, new TimeoutException(), out retryInterval));
        }

        [Fact]
        public void ShouldRetry_ShouldReturn_OnIOException()
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(true, retry.ShouldRetry(0, new IOException(), out retryInterval));
        }

        [Fact]
        public void ShouldRetry_ShouldReturn_OnJsonException()
        {
            var retry = new ExponentialRetry(default(TimeSpan), 3);

            TimeSpan retryInterval;
            Assert.Equal(false, retry.ShouldRetry(0, new JsonSerializationException(), out retryInterval));
        }

        [Theory]
        [InlineData(0, 3, 3)]
        [InlineData(1, 6.2, 7.8)]
        [InlineData(2, 12.6, 17.4)]
        public void ShouldRetry_ShouldReturn_ExponentiallyIncreasingRetryInterval(uint currentRetryCount, int min, int max)
        {
            var retry = new ExponentialRetry(TimeSpan.FromSeconds(4), 3);

            TimeSpan retryInterval;
            retry.ShouldRetry(currentRetryCount, HttpStatusCode.RequestTimeout, out retryInterval);

            Assert.InRange(retryInterval.Seconds, min, max);
        }

        [Fact]
        public void ShouldThrowException_OnNegativeDeltaBackoff()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExponentialRetry(TimeSpan.FromSeconds(-4), 3));
        }
    }
}
