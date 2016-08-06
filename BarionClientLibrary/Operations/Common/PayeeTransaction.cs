namespace BarionClientLibrary.Operations.Common
{
    public class PayeeTransaction
    {
        public string POSTransactionId { get; set; }
        public string Payee { get; set; }
        public decimal Total { get; set; }
        public string Comment { get; set; }
    }
}