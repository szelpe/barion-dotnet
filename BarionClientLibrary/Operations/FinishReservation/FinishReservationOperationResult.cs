using BarionClientLibrary.Operations.Common;
using System;

namespace BarionClientLibrary.Operations.FinishReservation
{
    public class FinishReservationOperationResult : BarionOperationResult
    {
        public bool IsSuccessful { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentRequestId { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentTransactionResponse[] Transactions { get; set; }
    }
}