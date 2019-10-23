using System;

namespace BarionClientLibrary.Operations.Common
{
    public class PurchaseInformation
    {
        public DeliveryTimeframeType DeliveryTimeframe { get; set; }
        public string DeliveryEmailAddress { get; set; }
        public DateTime PreOrderDate { get; set; }
        public AvailabilityIndicator AvailabilityIndicator { get; set; }
        public ReOrderIndicator ReOrderIndicator { get; set; }
        public ShippingAddressIndicator ShippingAddressIndicator { get; set; }
        public DateTime RecurringExpiry { get; set; }
        public int RecurringFrequency { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public GiftCardPurchase GiftCardPurchase { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
