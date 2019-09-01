namespace BarionClientLibrary.Operations.Common
{
    public enum AccountCreationIndicator
    {
        NoAccount = 0,
        CreatedDuringThisTransaction = 10,
        LessThan30Days = 20,
        Between30And60Days = 30,
        MoreThan60Days = 40
    }
}
