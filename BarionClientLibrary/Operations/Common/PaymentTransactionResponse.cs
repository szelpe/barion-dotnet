using System;

namespace BarionClientLibrary.Operations.Common
{
    public class PaymentTransactionResponse
    {
        public string? POSTransactionId { get; set; }
        public Guid TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime TransactionTime { get; set; }
        public Currency Currency { get; set; }
    }
}