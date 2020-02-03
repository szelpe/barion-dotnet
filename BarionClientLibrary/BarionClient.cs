using BarionClientLibrary.Operations;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.RetryPolicies;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using BarionClientLibrary.Helpers;

namespace BarionClientLibrary
{
    /// <summary>
    /// Provides a base class for executing Barion operations.
    /// </summary>
    public class BarionClient : IDisposable
    {
        private HttpClient _httpClient;
        private readonly BarionSettings _settings;
        private IRetryPolicy _retryPolicy;
        private TimeSpan _timeout;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);
        private static readonly TimeSpan MaxTimeout = TimeSpan.FromMilliseconds(int.MaxValue);
        private static readonly TimeSpan InfiniteTimeout = System.Threading.Timeout.InfiniteTimeSpan;

        /// <summary>
        /// Initializes a new instance of the BarionClientLibrary.BarionClient class.
        /// </summary>
        /// <param name="settings">Barion specific settings.</param>
        public BarionClient(BarionSettings settings) : this(settings, new HttpClient()) { }

        /// <summary>
        /// Initializes a new instance of the BarionClientLibrary.BarionClient class.
        /// </summary>
        /// <param name="settings">Barion specific settings.</param>
        /// <param name="httpClient">HttpClient instance to use for sending HTTP requests.</param>
        public BarionClient(BarionSettings settings, HttpClient httpClient)
        {
#if NET45
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.BaseUrl == null)
                throw new ArgumentNullException(nameof(settings.BaseUrl));

            if (!settings.BaseUrl.IsAbsoluteUri)
                throw new ArgumentException($"BaseUrl must be an absolute Uri. Actual value: {settings.BaseUrl}", nameof(settings.BaseUrl));

            _settings = settings;

            _retryPolicy = new ExponentialRetry();

            _timeout = DefaultTimeout;
        }

        /// <summary>
        /// Gets or sets the retry policy to use on transient failures.
        /// </summary>
        public IRetryPolicy RetryPolicy
        {
            get => _retryPolicy;
            set => _retryPolicy = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout
        {
            get => _timeout;
            set
            {
                if (value != InfiniteTimeout && (value <= TimeSpan.Zero || value > MaxTimeout))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _timeout = value;
            }
        }

        /// <summary>
        /// Executes a Barion operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the Barion operation.</typeparam>
        /// <param name="operation">The Barion operation to execute.</param>
        /// <returns>Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous operation.</returns>
        public async Task<TResult> ExecuteAsync<TResult>(BarionOperation operation)
            where TResult : BarionOperationResult
        {
            if (typeof(TResult) != operation.ResultType)
                throw new InvalidOperationException("TResult should be equal to the ResultType of the operation.");

            return await ExecuteAsync(operation) as TResult;
        }

        /// <summary>
        /// Executes a Barion operation.
        /// </summary>
        /// <param name="operation">The Barion operation to execute.</param>
        /// <returns>Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous operation.</returns>
        public async Task<BarionOperationResult> ExecuteAsync(BarionOperation operation)
        {
            return await ExecuteAsync(operation, default(CancellationToken));
        }

        /// <summary>
        /// Executes a Barion operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the Barion operation.</typeparam>
        /// <param name="operation">The Barion operation to execute.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous operation.</returns>
        public async Task<TResult> ExecuteAsync<TResult>(BarionOperation operation, CancellationToken cancellationToken)
            where TResult : BarionOperationResult
        {
            if (typeof(TResult) != operation.ResultType)
                throw new InvalidOperationException("TResult should be equal to the ResultType of the operation.");

            return await ExecuteAsync(operation, cancellationToken) as TResult;
        }

        /// <summary>
        /// Executes a Barion operation.
        /// </summary>
        /// <param name="operation">The Barion operation to execute.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous operation.</returns>
        public async Task<BarionOperationResult> ExecuteAsync(BarionOperation operation, CancellationToken cancellationToken)
        {
            CheckDisposed();
            ValidateOperation(operation);

            operation.POSKey = _settings.POSKey;

            return await SendWithRetry(operation, cancellationToken);
        }

        private async Task<BarionOperationResult> SendWithRetry(BarionOperation operation, CancellationToken cancellationToken)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            SetTimeout(linkedCts);

            var shouldRetry = false;
            uint currentRetryCount = 0;
            TimeSpan retryInterval = TimeSpan.Zero;
            BarionOperationResult result = null;

            do
            {
                var message = PrepareHttpRequestMessage(operation);

                try
                {
                    var responseMessage = await _httpClient.SendAsync(message, linkedCts.Token);

                    result = await CreateResultFromResponseMessage(responseMessage, operation);

                    if (!result.IsOperationSuccessful)
                        shouldRetry = _retryPolicy.CreateInstance().ShouldRetry(currentRetryCount, responseMessage.StatusCode, out retryInterval);
                }
                catch (Exception ex)
                {
                    shouldRetry = _retryPolicy.CreateInstance().ShouldRetry(currentRetryCount, ex, out retryInterval);

                    if (!shouldRetry)
                        throw;
                }

                if (shouldRetry)
                {
                    await Task.Delay(retryInterval);
                    currentRetryCount++;
                }
            } while (shouldRetry && !linkedCts.IsCancellationRequested);

            return result;
        }

        private HttpRequestMessage PrepareHttpRequestMessage(BarionOperation operation)
        {
            var message = new HttpRequestMessage(operation.Method, new Uri(_settings.BaseUrl, operation.RelativeUri));

            if (operation.Method == HttpMethod.Post || operation.Method == HttpMethod.Put)
            {
                var body = JsonConvert.SerializeObject(operation, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter> { new StringEnumConverter(), new CultureInfoJsonConverter() }
                });
                message.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            return message;
        }

        private async Task<BarionOperationResult> CreateResultFromResponseMessage(HttpResponseMessage responseMessage, BarionOperation operation)
        {
            var response = await responseMessage.Content.ReadAsStringAsync();

            var operationResult = (BarionOperationResult)JsonConvert.DeserializeObject(response, operation.ResultType, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter { AllowIntegerValues = false }, new CultureInfoJsonConverter() }
            });

            if (operationResult == null)
                return CreateFailedOperationResult(operation.ResultType, "Deserialized result was null");

            if (!responseMessage.IsSuccessStatusCode && operationResult.Errors == null)
                return CreateFailedOperationResult(operation.ResultType, responseMessage.StatusCode.ToString(), responseMessage.ReasonPhrase, response);

            operationResult.IsOperationSuccessful = responseMessage.IsSuccessStatusCode && (operationResult.Errors == null || !operationResult.Errors.Any());

            return operationResult;
        }

        private void SetTimeout(CancellationTokenSource cancellationTokenSource)
        {
            if (_timeout != InfiniteTimeout)
            {
                cancellationTokenSource.CancelAfter(_timeout);
            }
        }

        private void ValidateOperation(BarionOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (operation.RelativeUri == null)
                throw new ArgumentNullException(nameof(operation.RelativeUri));

            if (operation.RelativeUri.IsAbsoluteUri)
                throw new ArgumentException("operation.RelativeUri should be a relative Uri.", nameof(operation.RelativeUri));

            if (operation.ResultType == null)
                throw new ArgumentNullException(nameof(operation.ResultType));

            if (!operation.ResultType.GetTypeInfo().IsSubclassOf(typeof(BarionOperationResult)))
                throw new ArgumentException("ResultType should be a subclass of BarionOperationResult.", nameof(operation.ResultType));

            if (operation.Method == null)
                throw new ArgumentNullException(nameof(operation.Method));
        }

        private BarionOperationResult CreateFailedOperationResult(Type resultType, string errorCode, string title = null, string description = null)
        {
            var result = Activator.CreateInstance(resultType) as BarionOperationResult;
            result.IsOperationSuccessful = false;
            result.Errors = new[] 
            {
                new Error
                {
                    ErrorCode = errorCode,
                    Title = title,
                    Description = description
                }
            };

            return result;
        }

        #region IDisposable members

        private volatile bool _disposed;

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the BarionClient.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BarionClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the BarionClient and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                }
            }
        }

        #endregion

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }
    }
}
