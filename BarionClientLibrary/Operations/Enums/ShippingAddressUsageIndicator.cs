namespace BarionClientLibrary.Operations.Common
{
    public enum ShippingAddressUsageIndicator
    {
        ThisTransaction = 0,
        LessThan30Days = 10,
        Between30And60Days = 20,
        MoreThan60Days = 30
    }
}
