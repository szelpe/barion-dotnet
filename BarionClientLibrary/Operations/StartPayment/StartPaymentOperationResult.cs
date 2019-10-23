using BarionClientLibrary.Operations.Common;
using System;

namespace BarionClientLibrary.Operations.StartPayment
{
    /// <summary>
    /// Represents the result of the start payment operation.
    /// </summary>
    public class StartPaymentOperationResult : BarionOperationResult
    {
        public Guid PaymentId { get; set; }
        public string? PaymentRequestId { get; set; }
        public PaymentStatus Status { get; set; }
        public string? QRUrl { get; set; }
        public RecurrenceResult RecurrenceResult { get; set; }
        public string? GatewayUrl { get; set; }
        public string? CallbackUrl { get; set; }
        public string? RedirectUrl { get; set; }
        public PaymentTransactionResponse[] Transactions { get; set; }
    }
}
