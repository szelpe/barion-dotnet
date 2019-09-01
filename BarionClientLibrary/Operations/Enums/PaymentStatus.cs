namespace BarionClientLibrary.Operations.Common
{
    public enum PaymentStatus
    {
        Prepared = 1,
        Started,
        InProgress,
        Reserved,
        Canceled,
        Succeeded,
        PartiallySucceeded,
        Failed,
        Deleted,
        Expired
    }
}