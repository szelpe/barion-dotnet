using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations
{
    /// <summary>
    /// Defines a base class for Barion operations.
    /// </summary>
    public abstract class BarionOperation<TResult>
        where TResult: BarionOperationResult
    {
        /// <summary>
        /// The HTTP method of the operation.
        /// </summary>
        [JsonIgnore]
        public abstract HttpMethod Method { get; }

        /// <summary>
        /// The relative URI of the operation.
        /// </summary>
        [JsonIgnore]
        public abstract Uri RelativeUri { get; }
        
        /// <summary>
        /// The private key of the shop.
        /// </summary>
        public Guid POSKey { get; internal set; }
    }
}