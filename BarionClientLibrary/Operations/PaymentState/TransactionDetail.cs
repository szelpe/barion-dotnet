using BarionClientLibrary.Operations.Common;
using System;

namespace BarionClientLibrary.Operations.PaymentState
{
    public class TransactionDetail
    {
        public Guid TransactionId { get; set; }
        public string POSTransactionId { get; set; }
        public DateTime TransactionTime { get; set; }
        public decimal Total { get; set; }
        public Currency Currency { get; set; }
        public BarionUser Payer { get; set; }
        public BarionUser Payee { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public Item[] Items { get; set; }
        public Guid? RelatedId { get; set; }
        public Guid POSId { get; set; }
        public Guid PaymentId { get; set; }
    }
}