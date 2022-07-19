namespace BarionClientLibrary.Operations.StartPayment
{
    public enum RecurrenceResult
    {
        None,
        Successful,
        Failed,
        NotFound,
        ThreeDSAuthenticationRequired,
    }
}