using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace BarionClientLibrary.RetryPolicies
{
    /// <summary>
    /// Defines a base class for retry policies.
    /// </summary>
    public abstract class RetryPolicy : IRetryPolicy
    {
        protected readonly uint _maximumAttempts;

        public RetryPolicy(uint maxAttempts)
        {
            _maximumAttempts = maxAttempts;
        }

        /// <summary>
        /// Determines whether the operation should be retried and the interval until the next retry.
        /// </summary>
        /// <param name="currentRetryCount">An integer specifying the number of retries for the given operation. A value of zero signifies this is the first error encountered.</param>
        /// <param name="statusCode">The status code for the last operation.</param>
        /// <param name="retryInterval">A <see cref="TimeSpan"/> indicating the interval to wait until the next retry.</param>
        /// <returns><c>true</c> if the operation should be retried; otherwise, <c>false</c>.</returns>
        public bool ShouldRetry(uint currentRetryCount, HttpStatusCode statusCode, out TimeSpan retryInterval)
        {
            retryInterval = TimeSpan.Zero;

            if (IsNotTransientStatusCode(statusCode))
            {
                return false;
            }

            if (currentRetryCount < _maximumAttempts)
            {
                retryInterval = CalculateRetryInterval(currentRetryCount);
                return true;
            }

            return false;
        }

        private static bool IsNotTransientStatusCode(HttpStatusCode statusCode)
        {
            return ((int)statusCode >= 200 && (int)statusCode < 500 && statusCode != HttpStatusCode.RequestTimeout)
                              || statusCode == HttpStatusCode.NotImplemented
                                || statusCode == HttpStatusCode.HttpVersionNotSupported;
        }

        /// <summary>
        /// Determines whether the operation should be retried and the interval until the next retry.
        /// </summary>
        /// <param name="currentRetryCount">An integer specifying the number of retries for the given operation. A value of zero signifies this is the first error encountered.</param>
        /// <param name="lastException">An <see cref="Exception"/> object that represents the last exception encountered.</param>
        /// <param name="retryInterval">A <see cref="TimeSpan"/> indicating the interval to wait until the next retry.</param>
        /// <returns><c>true</c> if the operation should be retried; otherwise, <c>false</c>.</returns>
        public bool ShouldRetry(uint currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            retryInterval = TimeSpan.Zero;
            if (!IsTransient(lastException))
                return false;

            if (currentRetryCount < _maximumAttempts)
            {
                retryInterval = CalculateRetryInterval(currentRetryCount);
                return true;
            }

            return false;
        }

        private static readonly WebExceptionStatus[] webExceptionStatus = new []
        {
            WebExceptionStatus.ConnectionClosed,
            WebExceptionStatus.Timeout,
            WebExceptionStatus.RequestCanceled,
            WebExceptionStatus.KeepAliveFailure,
            WebExceptionStatus.PipelineFailure,
            WebExceptionStatus.ReceiveFailure,
            WebExceptionStatus.ConnectFailure,
            WebExceptionStatus.SendFailure
        };
        private static readonly SocketError[] socketErrorCodes = new [] { SocketError.ConnectionRefused, SocketError.TimedOut };

        private bool IsTransient(Exception ex)
        {
            var httpRequestException = ex as HttpRequestException;
            WebException webException;
            if (httpRequestException != null)
                webException = (WebException)httpRequestException.InnerException;
            else
                webException = (WebException)ex;

            if (webException != null)
            {
                if (webExceptionStatus.Contains(webException.Status))
                    return true;

                return false;
            }

            SocketException socketException;
            if (httpRequestException != null)
                socketException = (SocketException)httpRequestException.InnerException;
            else
                socketException = (SocketException)ex;

            if (socketException != null)
            {
                // This section handles the following transient faults:
                //
                // Exception Type: System.Net.Sockets.SocketException
                //         Error Code: 10061
                //         Message: No connection could be made because the target machine actively refused it XXX.XXX.XXX.XXX:443
                //         Socket Error Code: ConnectionRefused
                // Exception Type: System.Net.Sockets.SocketException
                //      Error Code: 10060
                //      Message: A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 168.62.128.143:443
                //      Socket Error Code: TimedOut
                if (socketErrorCodes.Contains(socketException.SocketErrorCode))
                    return true;
            }

            if (ex is TimeoutException)
                return true;

            // This may be System.IO.IOException: "Unable to read data from the transport connection: The connection was closed" which could manifest itself under extremely high load.
            // Do not return true if ex is a subtype of IOException (such as FileLoadException, when it cannot load a required assembly)
            if (ex.GetType() == typeof(IOException) && ex.InnerException == null)
                return true;

            return false;
        }

        protected abstract TimeSpan CalculateRetryInterval(uint currentRetryCount);

        /// <summary>
        /// Generates a new retry policy for the current request attempt.
        /// </summary>
        /// <returns>An <see cref="IRetryPolicy"/> object that represents the retry policy for the current request attempt.</returns>
        public abstract IRetryPolicy CreateInstance();
    }
}
