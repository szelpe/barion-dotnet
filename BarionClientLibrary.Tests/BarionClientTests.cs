using System;
using System.Net.Http;
using BarionClientLibrary.Operations.StartPayment;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using Newtonsoft.Json;
using NSubstitute;
using BarionClientLibrary.RetryPolicies;

namespace BarionClientLibrary.Tests
{
    public class BarionClientTests
    {
        private BarionClient _barionClient;
        private HttpClient _httpClient;
        private TestHttpMessageHandler _httpMessageHandler;
        private BarionSettings _barionClientSettings;
        private IRetryPolicy _retryPolicy;

        [SetUp]
        public void Initialize()
        {
            _httpMessageHandler = new TestHttpMessageHandler();
            _httpClient = new HttpClient(_httpMessageHandler);
            _retryPolicy = Substitute.For<IRetryPolicy>();
            _retryPolicy.CreateInstance().Returns(_retryPolicy);

            _barionClientSettings = new BarionSettings
            {
                BaseUrl = new Uri("https://api.barion.com"),
                POSKey = Guid.NewGuid(),
                RetryPolicy = _retryPolicy
            };
            _barionClient = new BarionClient(_barionClientSettings, _httpClient);
        }

        [Test]
        public async Task ExecuteAsync_ShouldSendHttpRequest()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            var result = await _barionClient.ExecuteAsync(new StartPaymentOperation());

            Assert.AreEqual(1, _httpMessageHandler.SendAsyncCallCount, "SendAsync should have been called once and only once.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldRetrySending_IfPolicyIndicates()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.RequestTimeout;
            TimeSpan timespan;
            _retryPolicy.ShouldRetry(0, default(HttpStatusCode), out timespan)
                .ReturnsForAnyArgs(args => {
                    var retryCount = args[0] as int?;
                    args[2] = TimeSpan.FromMilliseconds(1);

                    if (retryCount < 3)
                        return true;
                    
                    return false;
                });

            var result = await _barionClient.ExecuteAsync(new StartPaymentOperation());

            Assert.AreEqual(4, _httpMessageHandler.SendAsyncCallCount, "SendAsync should have been called three times.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetValidPOSKey()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = HttpMethod.Post;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.AreEqual($"{{\r\n  \"POSKey\": \"{_barionClientSettings.POSKey}\"\r\n}}", _httpMessageHandler.HttpRequestBody, "Request body should contain the POSKey.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetValidPOSKey_OnPutRequest()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = HttpMethod.Put;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.AreEqual($"{{\r\n  \"POSKey\": \"{_barionClientSettings.POSKey}\"\r\n}}", _httpMessageHandler.HttpRequestBody, "Request body should contain the POSKey.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetContentTypeTo_Json()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = HttpMethod.Put;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.AreEqual("application/json", _httpMessageHandler.HttpRequestMessage.Content.Headers.ContentType.MediaType, "MediaType should have been application/json.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetEncodingTo_UTF8()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = PrepareValidOperation();
            operation.MethodReturns = HttpMethod.Post;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.AreEqual("utf-8", _httpMessageHandler.HttpRequestMessage.Content.Headers.ContentType.CharSet, "CharSet should have been utf-8.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSerializeEnumsTo_String()
        {
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            var operation = new TestOperationWithEnum {
                Color = ConsoleColor.Red
            };
            PrepareValidOperation(operation);

            operation.MethodReturns = HttpMethod.Post;

            var result = await _barionClient.ExecuteAsync(operation);

            StringAssert.Contains("\"Color\": \"Red\"", _httpMessageHandler.HttpRequestBody, "Request body should have contained the Color property.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldNotSetBody_OnGetRequests()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            operation.MethodReturns = HttpMethod.Get;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.IsNull(_httpMessageHandler.HttpRequestBody, "Request body should have been null.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetValidUri()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            _barionClientSettings.BaseUrl = new Uri("https://api.barion.com/");
            operation.RelativeUriReturns = new Uri("/payment/start", UriKind.Relative);

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.AreEqual(_httpMessageHandler.HttpRequestMessage.RequestUri.ToString(), "https://api.barion.com/payment/start", "Request url should have been valid.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetOperationSuccessTrue_OnSuccessStatusCode()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.IsTrue(result.IsOperationSuccessful, "Operation should have been successful.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetOperationSuccessFalse_OnErrorStatusCode()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.BadRequest;

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.IsFalse(result.IsOperationSuccessful, "Operation should not have been successful.");
            Assert.AreEqual("BadRequest", result.Errors[0].ErrorCode, "ErrorCode should have been `BadRequest`.");
            Assert.AreEqual("Bad Request", result.Errors[0].Title, "Title should have been `Bad Request`.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetOperationSuccessFalse_OnErrorStatusCode_EvenIfTheErrorsArrayIsEmpty()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent("{\"Errors\":[]}");

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.IsFalse(result.IsOperationSuccessful, "Operation should not have been successful.");
        }

        [Test]
        public async Task ExecuteAsync_ShouldSetOperationSuccessFalse_OnErrorsArrayIsNotEmpty()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent("{\"Errors\":[{\"ErrorCode\":\"1\", \"Title\":\"Error title\", \"Description\": \"Error\"}]}");

            var result = await _barionClient.ExecuteAsync(operation);

            Assert.IsFalse(result.IsOperationSuccessful, "Operation should not have been successful.");
        }

        [Test]
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

            Assert.AreEqual(1, result.IntProperty, "Int should have been parsed correctly.");
            Assert.AreEqual(1.23, result.DecimalProperty, "Decimal should have been parsed correctly.");
            Assert.AreEqual(1.23, result.DoubleProperty, "Double should have been parsed correctly.");
            Assert.AreEqual(new DateTime(2016, 8, 20, 11, 36, 14, 333), result.DateTimeProperty, "DateTime should have been parsed correctly.");
            Assert.AreEqual("a nice string", result.StringProperty, "String should have been parsed correctly.");
            Assert.AreEqual(ConsoleColor.Red, result.EnumProperty, "Enum should have been parsed correctly.");
            Assert.AreEqual(CultureInfo.CreateSpecificCulture("hu-HU"), result.CultureInfoProperty, "CultureInfo should have been parsed correctly.");
            Assert.AreEqual(Guid.Parse("462063d5b915410cae9d3bd423583f0f"), result.GuidProperty, "Guid should have been parsed correctly.");
            Assert.AreEqual(TimeSpan.FromDays(1), result.TimeSpanProtperty, "TimeSpan should have been parsed correctly.");
        }

        [Test]
        public void ExecuteAsync_ShouldNotAllowNumberAsAnEnum()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();
            _httpMessageHandler.HttpResponseMessage.Content = new StringContent(@"{
                ""EnumProperty"": 12,
            }");

            Assert.ThrowsAsync<JsonSerializationException>(async () => await _barionClient.ExecuteAsync<TestOperationResult>(operation));
        }

        [Test]
        public void BarionClient_ShouldThrowException_IfAlreadyDisposed()
        {
            var operation = PrepareValidOperation();
            _httpMessageHandler.HttpResponseMessage = PrepareValidResponse();

            _barionClient.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(async () => await _barionClient.ExecuteAsync(operation));
        }

        [Test]
        public void BarionClient_ShouldThrowException_IfBaseUrl_IsRelative()
        {
            _barionClientSettings = new BarionSettings
            {
                BaseUrl = new Uri("/index.html", UriKind.Relative),
                POSKey = Guid.Empty
            };

            Assert.Throws<ArgumentException>(() => new BarionClient(_barionClientSettings));
        }

        [Test]
        public void BarionClient_ShouldThrowException_IfSettings_IsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BarionClient(null));
        }

        [Test]
        public void BarionClient_ShouldThrowException_IfBaseUrl_IsNull()
        {
            _barionClientSettings = new BarionSettings
            {
                POSKey = Guid.Empty
            };

            Assert.Throws<ArgumentNullException>(() => new BarionClient(_barionClientSettings));
        }

        [Test]
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
