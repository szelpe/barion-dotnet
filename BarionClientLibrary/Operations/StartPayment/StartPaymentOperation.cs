using BarionClientLibrary.Operations.Common;
using System;
using System.Globalization;
using System.Net.Http;

namespace BarionClientLibrary.Operations.StartPayment
{
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
        public string RedirectUrl { get; set; }
        public string CallbackUrl { get; set; }
        public PaymentTransaction[] Transactions { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingAddress { get; set; }
        public CultureInfo Locale { get; set; }
        public Currency Currency { get; set; }

        public override Uri RelativeUri => new Uri("/v2/Payment/Start", UriKind.Relative);
        public override HttpMethod Method => HttpMethod.Post;
        public override Type ResultType => typeof(StartPaymentOperationResult);
    }
}
