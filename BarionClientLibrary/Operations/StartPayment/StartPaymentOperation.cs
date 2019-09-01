using BarionClientLibrary.Operations.Common;
using System;
using System.Globalization;
using System.Net.Http;

namespace BarionClientLibrary.Operations.StartPayment
{
    /// <summary>
    /// Represents a start payment operation.
    /// </summary>
    /// <remarks>
    /// View the full documentation of the operation here: https://docs.barion.com/Payment-Start-v2-3DS
    /// </remarks>
    public class StartPaymentOperation : BarionOperation
    {
        public PaymentType PaymentType { get; set; }
        public TimeSpan? ReservationPeriod { get; set; }
        public TimeSpan? PaymentWindow { get; set; }
        public bool GuestCheckOut { get; set; }
        public bool InitiateRecurrence { get; set; }
        public string RecurrenceId { get; set; }
        public FundingSourceType[] FundingSources { get; set; }
        public string PaymentRequestId { get; set; }
        public string PayerHint { get; set; }
        public string CardHolderNameHint { get; set; }
        public RecurrenceType? RecurrenceType { get; set; }
        public string RedirectUrl { get; set; }
        public string CallbackUrl { get; set; }
        public PaymentTransaction[] Transactions { get; set; }
        public string OrderNumber { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public CultureInfo Locale { get; set; }
        public Currency Currency { get; set; }
        public string PayerPhoneNumber { get; set; }
        public string PayerWorkPhoneNumber { get; set; }
        public string PayerHomeNumber { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public PayerAccountInformation PayerAccount { get; set; }
        public PurchaseInformation PurchaseInformation { get; set; }
        public ChallengePreference ChallengePreference { get; set; }

        public override Uri RelativeUri => new Uri("/v2/Payment/Start", UriKind.Relative);
        public override HttpMethod Method => HttpMethod.Post;
        public override Type ResultType => typeof(StartPaymentOperationResult);
    }
}
