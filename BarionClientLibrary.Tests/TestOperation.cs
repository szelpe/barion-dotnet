using System;
using System.Net.Http;
using BarionClientLibrary.Operations;
using Newtonsoft.Json;

namespace BarionClientLibrary.Tests
{
    internal class TestOperation : BarionOperation
    {
        public override HttpMethod Method => MethodReturns;

        [JsonIgnore]
        public HttpMethod MethodReturns { get; set; }

        public override Uri RelativeUri => RelativeUriReturns;

        [JsonIgnore]
        public Uri RelativeUriReturns { get; set; }

        public override Type ResultType => ResultTypeReturns;

        [JsonIgnore]
        public Type ResultTypeReturns { get; set; }
    }

    internal class TestOperationWithEnum : TestOperation
    {
        public ConsoleColor Color { get; set; }
    }
}