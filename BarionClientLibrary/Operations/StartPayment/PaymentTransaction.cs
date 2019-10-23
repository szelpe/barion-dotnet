using BarionClientLibrary.Operations.Common;

namespace BarionClientLibrary.Operations.StartPayment
{
    public class PaymentTransaction
    {
        public string? POSTransactionId { get; set; }
        public string? Payee { get; set; }
        public decimal Total { get; set; }
        public string? Comment { get; set; }
        public Item[] Items { get; set; }
        public PayeeTransaction[] PayeeTransactions { get; set; }
    }
}
