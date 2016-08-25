using BarionClientLibrary.Operations.Common;
using System;

namespace BarionClientLibrary.Operations.FinishReservation
{
    /// <summary>
    /// Represents the result of the finish reservation operation.
    /// </summary>
    public class FinishReservationOperationResult : BarionOperationResult
    {
        public bool IsSuccessful { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentRequestId { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentTransactionResponse[] Transactions { get; set; }
    }
}