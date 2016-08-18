using BarionClientLibrary.Operations;
using BarionClientLibrary.Operations.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarionClientLibrary
{
    public class BarionClient : IDisposable
    {
        private HttpClient _httpClient;
        private readonly BarionSettings _settings;

        public BarionClient(BarionSettings settings) : this(settings, new HttpClient()) {}

        public BarionClient(BarionSettings settings, HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.BaseUrl == null)
                throw new ArgumentNullException(nameof(settings.BaseUrl));

            _settings = settings;
        }

        public async Task<TResult> ExecuteAsync<TResult>(BarionOperation operation)
            where TResult : BarionOperationResult
        {
            if (typeof(TResult) != operation.ResultType)
                throw new InvalidOperationException("TResult should be equal to the ResultType of the operation.");

            return await ExecuteAsync(operation) as TResult;
        }

        public async Task<BarionOperationResult> ExecuteAsync(BarionOperation operation)
        {
            return await ExecuteAsync(operation, default(CancellationToken));
        }

        public async Task<TResult> ExecuteAsync<TResult>(BarionOperation operation, CancellationToken cancellationToken)
            where TResult : BarionOperationResult
        {
            if (typeof(TResult) != operation.ResultType)
                throw new InvalidOperationException("TResult should be equal to the ResultType of the operation.");

            return await ExecuteAsync(operation, cancellationToken) as TResult;
        }

        public async Task<BarionOperationResult> ExecuteAsync(BarionOperation operation, CancellationToken cancellationToken)
        {
            CheckDisposed();
            ValidateOperation(operation);

            operation.POSKey = _settings.POSKey;
            var message = PrepareHttpRequestMessage(operation);

            var responseMessage = await _httpClient.SendAsync(message, cancellationToken);

            return await CreateResultFromResponseMessage(responseMessage, operation);
        }

        private HttpRequestMessage PrepareHttpRequestMessage(BarionOperation operation)
        {
            var message = new HttpRequestMessage(operation.Method, new Uri(_settings.BaseUrl, operation.RelativeUri));

            if (operation.Method == HttpMethod.Post || operation.Method == HttpMethod.Put)
            {
                var body = JsonConvert.SerializeObject(operation, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                });
                message.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            return message;
        }

        private async Task<BarionOperationResult> CreateResultFromResponseMessage(HttpResponseMessage responseMessage, BarionOperation operation)
        {
            var response = await responseMessage.Content.ReadAsStringAsync();

            var operationResult = (BarionOperationResult)JsonConvert.DeserializeObject(response, operation.ResultType);

            if (!responseMessage.IsSuccessStatusCode && operationResult.Errors == null)
            {
                return new BarionOperationResult
                {
                    IsOperationSuccessful = false,
                    Errors = new[] { new Error
                        {
                            ErrorCode = responseMessage.StatusCode.ToString(),
                            Title = responseMessage.ReasonPhrase,
                            Description = response
                        }
                    }
                };
            }

            operationResult.IsOperationSuccessful = responseMessage.IsSuccessStatusCode && !operationResult.Errors.Any();
            
            return operationResult;
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

            if (!operation.ResultType.IsSubclassOf(typeof(BarionOperationResult)))
                throw new ArgumentException("ResultType should be a subclass of BarionOperationResult.", nameof(operation.ResultType));

            if (operation.Method == null)
                throw new ArgumentNullException(nameof(operation.Method));
        }

        #region IDisposable members

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BarionClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing && !_disposed)
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
