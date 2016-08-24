using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BarionClientLibrary.Tests
{
    internal class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage HttpRequestMessage { get; set; }
        public string HttpRequestBody { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        public int SendAsyncCallCount { get; private set; }

        public Exception SendAsyncException { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SendAsyncCallCount++;
            if (SendAsyncException != null)
                throw SendAsyncException;

            HttpRequestMessage = request;
            if (request.Content != null)
            {
                HttpRequestBody = await request.Content.ReadAsStringAsync();
            }
            CancellationToken = cancellationToken;

            return HttpResponseMessage;
        }
    }
}