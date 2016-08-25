using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations.FinishReservation
{
    /// <summary>
    /// Represents a finish reservation operation.
    /// </summary>
    /// <remarks>
    /// View the full documentation of the operation here: https://doksi.barion.com/Payment-FinishReservation-v2
    /// </remarks>
    public class FinishReservationOperation : BarionOperation
    {
        public Guid PaymentId { get; set; }
        public TransactionToFinish[] Transactions { get; set; }

        public override HttpMethod Method => HttpMethod.Post;
        public override Uri RelativeUri => new Uri("/v2/Payment/FinishReservation", UriKind.Relative);
        public override Type ResultType => typeof(FinishReservationOperationResult);
    }
}
