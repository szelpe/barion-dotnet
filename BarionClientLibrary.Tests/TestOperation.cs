using System;
using System.Net.Http;
using BarionClientLibrary.Operations;
using Newtonsoft.Json;
using System.Globalization;

namespace BarionClientLibrary.Tests
{
    internal class TestOperation : BarionOperation<TestOperationResult>
    {
        public override HttpMethod Method => MethodReturns;

        [JsonIgnore]
        public HttpMethod MethodReturns { get; set; }

        public override Uri RelativeUri => RelativeUriReturns;

        [JsonIgnore]
        public Uri RelativeUriReturns { get; set; }

        [JsonIgnore]
        public Type ResultTypeReturns { get; set; }

        public CultureInfo TestCultureInfo { get; set; }
    }

    internal class TestOperationWithEnum : TestOperation
    {
        public ConsoleColor Color { get; set; }
    }
}