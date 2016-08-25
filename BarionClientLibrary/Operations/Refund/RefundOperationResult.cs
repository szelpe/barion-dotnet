using System;

namespace BarionClientLibrary.Operations.Refund
{
    /// <summary>
    /// Represents the result of the refund operation.
    /// </summary>
    public class RefundOperationResult : BarionOperationResult
    {
        public Guid PaymentId { get; set; }
        public RefundedTransaction[] RefundedTransactions { get; set; }
    }
}