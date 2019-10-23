using BarionClientLibrary.Operations.Common;
using System;
using System.Collections.Generic;

namespace BarionClientLibrary.Operations.FinishReservation
{
    /// <summary>
    /// Represents the result of the finish reservation operation.
    /// </summary>
    public class FinishReservationOperationResult : BarionOperationResult
    {
        public bool IsSuccessful { get; set; }
        public Guid PaymentId { get; set; }
        public string? PaymentRequestId { get; set; }
        public PaymentStatus Status { get; set; }
        public IReadOnlyList<PaymentTransactionResponse> Transactions { get; set; }

        public FinishReservationOperationResult()
        {
            Transactions = new List<PaymentTransactionResponse>();
        }
    }
}