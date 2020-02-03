using System;
using System.Net.Http;
using BarionClientLibrary.Operations.FinishReservation;
using BarionClientLibrary.Operations.StartPayment;

namespace BarionClientLibrary.Operations.Capture
{
    /// <summary>
    /// Represents a capture payment operation.
    /// </summary>
    /// <remarks>
    /// View the full documentation of the operation here: https://docs.barion.com/Payment-Capture-v2
    /// </remarks>
    public class CaptureOperation : BarionOperation
    {
        public Guid PaymentId { get; set; }
        public TransactionToFinish[] Transactions { get; set; }
        
        public override Uri RelativeUri => new Uri("/v2/Payment/Capture", UriKind.Relative);
        public override HttpMethod Method => HttpMethod.Post;
        public override Type ResultType => typeof(StartPaymentOperationResult);
    }
}