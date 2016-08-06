using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations.Refund
{
    public class RefundOperation : BarionOperation
    {
        public Guid PaymentId { get; set; }
        public TransactionToRefund[] TransactionsToRefund { get; set; }

        public override Uri RelativeUri => new Uri("/v2/Payment/Refund", UriKind.Relative);
        public override HttpMethod Method => HttpMethod.Post;
        public override Type ResultType => typeof(RefundOperationResult);
    }
}
