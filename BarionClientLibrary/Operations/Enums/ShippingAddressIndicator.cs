namespace BarionClientLibrary.Operations.Common
{
    public enum ShippingAddressIndicator
    {
        ShipToCardholdersBillingAddress = 0,
        ShipToAnotherVerifiedAddress = 10,
        ShipToDifferentAddress = 20,
        ShipToStore = 30,
        DigitalGoods = 40,
        TravelAndEventTickets = 50,
        Other = 60
    }
}
