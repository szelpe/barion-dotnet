namespace BarionClientLibrary.Operations.Common
{
    public enum AccountChangeIndicator
    {
        ChangedDuringThisTransaction = 0,
        LessThan30Days = 10,
        Between30And60Days = 20,
        MoreThan60Days = 30
    }
}
