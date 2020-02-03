namespace BarionClientLibrary.Operations.Common
{
    public enum PaymentStatus
    {
        Prepared = 1,
        Started,
        InProgress,
        Waiting,
        Reserved,
        Authorized,
        Canceled,
        Succeeded,
        PartiallySucceeded,
        Expired
    }
}