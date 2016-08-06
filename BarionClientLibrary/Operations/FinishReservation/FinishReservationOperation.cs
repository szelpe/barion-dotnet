using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations.FinishReservation
{
    public class FinishReservationOperation : BarionOperation
    {
        public Guid PaymentId { get; set; }
        public TransactionToFinish[] Transactions { get; set; }

        public override HttpMethod Method => HttpMethod.Post;
        public override Uri RelativeUri => new Uri("/v2/Payment/FinishReservation", UriKind.Relative);
        public override Type ResultType => typeof(FinishReservationOperationResult);
    }
}
