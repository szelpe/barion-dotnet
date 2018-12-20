using System;
using System.Net.Http;
using BarionClientLibrary.Operations.StartPayment;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using Newtonsoft.Json;
using Xunit;
using System.Threading;

namespace BarionClientLibrary.Tests
{
    public class BarionClientTests
    {
        private readonly BarionClient _barionClient;
        private readonly HttpClient _httpClient;
        private readonly TestHttpMessageHandler _httpMessageHandler;
        private BarionSettings _barionClientSettings;
        private readonly TestRetryPolicy _retryPolicy;

        public BarionClientTests()
        {
            _httpMessageHandler = new TestHttpMessageHandler();
            _httpClient = new HttpClient(_httpMessageHandler);
            _retryPolicy = new TestRetryPolicy();

            _barionClientSettings = new BarionSettings
            {
                BaseUrl = new Uri("https://api.barion.com"),
                POSKey = Guid.NewGuid()
            };
            _barionClient = new BarionClient(_barionClientSettings, _httpClient)
            {
                RetryPolicy = _retryPolicy
            };
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSendHttpRequest()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            var result = await _barionClient.ExecuteAsync(new StartPaymentOperation());

            Assert.Equal(1, _httpMessageHandler.SendAsyncCallCount);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRetrySending_IfPolicyIndicates()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.RequestTimeout;

            _retryPolicy.ShouldRetryMock =
                (retryCount, statusCode) => {
                    if (retryCount < 3)
                        return true;
                    
                    return false;
                };

            var result = await _barionClient.ExecuteAsync(new StartPaymentOperation());

            Assert.Equal(4, _httpMessageHandler.SendAsyncCallCount);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotRetrySending_IfCancellationRequested()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.RequestTimeout;
            var cts = new CancellationTokenSource();
            cts.Cancel();
            
            _retryPolicy.ShouldRetryMock =
                (retryCount, statusCode) => {
                    if (retryCount < 3)
                        return true;

                    return false;
                };

            var result = await _barionClient.ExecuteAsync(new StartPaymentOperation(), cts.Token);

            Assert.Equal(1, _httpMessageHandler.SendAsyncCallCount);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotRetrySending_IfTimeoutReached()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.RequestTimeout;
            _retryPolicy.RetryInterval = TimeSpan.FromMilliseconds(75);
            _retryPolicy.ShouldRetryMock =
                (retryCount, statusCode) => {
                    if (retryCount < 3)
                        return true;

                    return false;
                };

            _barionClient.Timeout = TimeSpan.FromMilliseconds(50);
            var result = await _barionClient.ExecuteAsync(new StartPaymentOperation());

            Assert.Equal(1, _httpMessageHandler.SendAsyncCallCount);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRetrySending_IfPolicyIndicates_AfterAnException()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.SendAsyncException = new Exception();
            
            _retryPolicy.ShouldRetryExceptionMock =
                (retryCount, exception) => {
                    if (retryCount < 3)
                        return true;

                    return false;
                };

            await Assert.ThrowsAsync<Exception>(async () => await _barionClient.ExecuteAsync(new StartPaymentOperation()));

            Assert.Equal(4, _httpMessageHandler.SendAsyncCallCount);
        }

        public static object[][] GetHttpMethods = { new [] { HttpMethod.Post }, new [] { HttpMethod.Put } };

        [Theory]
        [MemberData(nameof(GetHttpMethods))]
        public async Task ExecuteAsync_ShouldSetValidPOSKey(HttpMethod httpMethod)
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = httpMethod;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.Equal($"{{\r\n  \"POSKey\": \"{_barionClientSettings.POSKey}\"\r\n}}", _httpMessageHandler.HttpRequestBody);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetContentTypeTo_Json()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = HttpMethod.Put;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.Equal("application/json", _httpMessageHandler.HttpRequestMessage.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetEncodingTo_UTF8()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = HttpMethod.Post;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.Equal("utf-8", _httpMessageHandler.HttpRequestMessage.Content.Headers.ContentType.CharSet);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSerializeEnumsTo_String()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = new TestOperationWithEnum {
                Color = ConsoleColor.Red
            };
            PrepareValidOperation(operation);

            operation.MethodReturns = HttpMethod.Post;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.Contains("\"Color\": \"Red\"", _httpMessageHandler.HttpRequestBody);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotSetBody_OnGetRequests()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            operation.MethodReturns = HttpMethod.Get;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.Null(_httpMessageHandler.HttpRequestBody);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetValidUri()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            _barionClientSettings.BaseUrl = new Uri("https://api.barion.com/");
            operation.RelativeUriReturns = new Uri("/payment/start", UriKind.Relative);

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.Equal("https://api.barion.com/payment/start", _httpMessageHandler.HttpRequestMessage.RequestUri.ToString());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetOperationSuccessTrue_OnSuccessStatusCode()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.True(result.IsOperationSuccessful, "Operation should have been successful.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotReturnNull_EvenIfErrorOccured()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;

            var result = await _barionClient.ExecuteAsync<TestOperationResult>(operation);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetOperationSuccessFalse_OnErrorStatusCode()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.BadRequest;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.False(result.IsOperationSuccessful, "Operation should not have been successful.");
            Assert.Equal("BadRequest", result.Errors[0].ErrorCode);
            Assert.Equal("Bad Request", result.Errors[0].Title);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetOperationSuccessFalse_OnErrorStatusCode_EvenIfTheErrorsArrayIsEmpty()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent("{\"Errors\":[]}");

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.False(result.IsOperationSuccessful, "Operation should not have been successful.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSetOperationSuccessFalse_OnErrorsArrayIsNotEmpty()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent("{\"Errors\":[{\"ErrorCode\":\"1\", \"Title\":\"Error title\", \"Description\": \"Error\"}]}");

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.False(result.IsOperationSuccessful, "Operation should not have been successful.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldParseTheResponseCorrectly()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent(@"{
                ""IntProperty"": 1,
                ""DecimalProperty"": 1.23,
                ""DoubleProperty"": 1.23,
                ""DateTimeProperty"": ""2016-08-20T11:36:14.333"",
                ""StringProperty"": ""a nice string"",
                ""EnumProperty"": ""Red"",
                ""CultureInfoProperty"": ""hu-HU"",
                ""GuidProperty"": ""462063d5b915410cae9d3bd423583f0f"",
                ""TimeSpanProtperty"": ""1:00:00:00"",
            }");

            var result = await _barionClient.ExecuteAsync<TestOperationResult>(operation);

            Assert.Equal(1, result.IntProperty);
            Assert.Equal((decimal)1.23, result.DecimalProperty, 2);
            Assert.Equal(1.23, result.DoubleProperty);
            Assert.Equal(new DateTime(2016, 8, 20, 11, 36, 14, 333), result.DateTimeProperty);
            Assert.Equal("a nice string", result.StringProperty);
            Assert.Equal(ConsoleColor.Red, result.EnumProperty);
            Assert.Equal(new CultureInfo("hu-HU"), result.CultureInfoProperty);
            Assert.Equal(Guid.Parse("462063d5b915410cae9d3bd423583f0f"), result.GuidProperty);
            Assert.Equal(TimeSpan.FromDays(1), result.TimeSpanProtperty);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSerialize_CultureInfo()
        {
            var operation = PrepareValidOperation();
            operation.TestCultureInfo = new CultureInfo("hu-HU");
            operation.MethodReturns = HttpMethod.Post;
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            var result = await _barionClient.ExecuteAsync<TestOperationResult>(operation);

            Assert.Contains("\"TestCultureInfo\": \"hu-HU\"", _httpMessageHandler.HttpRequestBody);
        }

        [Fact]
        public void ExecuteAsync_ShouldNotAllowNumberAsAnEnum()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent(@"{
                ""EnumProperty"": 12,
            }");

            Assert.ThrowsAsync<JsonSerializationException>(async () => await _barionClient.ExecuteAsync<TestOperationResult>(operation));
        }

        [Fact]
        public void BarionClient_ShouldThrowException_IfAlreadyDisposed()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            _barionClient.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(async () => await _barionClient.ExecuteAsync(operation));
        }

        [Fact]
        public void BarionClient_ShouldThrowException_IfBaseUrl_IsRelative()
        {
            _barionClientSettings = new BarionSettings
            {
                BaseUrl = new Uri("/index.html", UriKind.Relative),
                POSKey = Guid.Empty
            };

            Assert.Throws<ArgumentException>(() => new BarionClient(_barionClientSettings));
        }

        [Fact]
        public void BarionClient_ShouldThrowException_IfSettings_IsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BarionClient(null));
        }

        [Fact]
        public void BarionClient_ShouldThrowException_IfBaseUrl_IsNull()
        {
            _barionClientSettings = new BarionSettings
            {
                POSKey = Guid.Empty
            };

            Assert.Throws<ArgumentNullException>(() => new BarionClient(_barionClientSettings));
        }

        [Fact]
        public void BarionClient_ShouldThrowException_IfHttpClient_IsNull()
        {
            _barionClientSettings = new BarionSettings
            {
                BaseUrl = new Uri("https://api.barion.com"),
                POSKey = Guid.Empty
            };

            Assert.Throws<ArgumentNullException>(() => new BarionClient(_barionClientSettings, null));
        }

        private HttpResponseMessage PrepareValidResponse()
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}"),
            };
        }

        private TestOperation PrepareValidOperation()
        {
            var operation = new TestOperation();

            return PrepareValidOperation(operation);
        }

        private TestOperation PrepareValidOperation(TestOperation operation)
        {
            operation.MethodReturns = HttpMethod.Get;
            operation.RelativeUriReturns = new Uri("/test", UriKind.Relative);
            operation.ResultTypeReturns = typeof(TestOperationResult);

            return operation;
        }
    }
}
