using System;

namespace BarionClientLibrary.Operations.Refund
{
    public class TransactionToRefund
    {
        public Guid TransactionId { get; set; }
        public string? POSTransactionId { get; set; }
        public decimal AmountToRefund { get; set; }
        public string? Comment { get; set; }
    }
}