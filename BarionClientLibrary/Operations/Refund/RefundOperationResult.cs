using System;

namespace BarionClientLibrary.Operations.Refund
{
    public class RefundOperationResult : BarionOperationResult
    {
        public Guid PaymentId { get; set; }
        public RefundedTransaction[] RefundedTransactions { get; set; }
    }
}