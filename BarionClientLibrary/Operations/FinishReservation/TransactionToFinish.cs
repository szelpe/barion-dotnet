using BarionClientLibrary.Operations.Common;
using System;

namespace BarionClientLibrary.Operations.FinishReservation
{
    public class TransactionToFinish
    {
        public Guid TransactionId { get; set; }
        public decimal Total { get; set; }
        public PayeeTransaction[] PayeeTransactions { get; set; }
        public Item[] Items { get; set; }
        public string Comment { get; set; }
    }
}