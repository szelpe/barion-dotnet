using System;

namespace BarionClientLibrary.Operations.Refund
{
    public class RefundedTransaction
    {
        public Guid TransactionId { get; set; }
        public decimal Total { get; set; }
        public string? POSTransactionId { get; set; }
        public string? Comment { get; set; }
        public string? Status { get; set; }
    }
}