namespace BarionClientLibrary.Operations.PaymentState
{
    public class BankCard
    {
        public string MaskedPan { get; set; }
        public string BankCardType { get; set; }
        public string ValidThru_Year { get; set; }
        public string ValidThru_Month { get; set; }
    }
}