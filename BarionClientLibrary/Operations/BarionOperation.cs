using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations
{
    public abstract class BarionOperation
    {
        [JsonIgnore]
        public abstract HttpMethod Method { get; }

        [JsonIgnore]
        public abstract Uri RelativeUri { get; }

        [JsonIgnore]
        public abstract Type ResultType { get; }

        public Guid POSKey { get; internal set; }
    }
}