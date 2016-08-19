namespace BarionClientLibrary.Operations.Common
{
    public enum PaymentStatus
    {
        Prepared = 1,
        Started,
        Reserved,
        Canceled,
        Succeeded,
        Failed,
        Deleted
    }
}