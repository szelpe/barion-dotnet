using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations.PaymentState
{
    public class GetPaymentStateOperation : BarionOperation
    {
        public Guid PaymentId { get; set; }

        public override Uri RelativeUri => new Uri($"/v2/Payment/GetPaymentState?PaymentId={PaymentId}&POSKey={POSKey}", UriKind.Relative);
        public override HttpMethod Method => HttpMethod.Get;
        public override Type ResultType => typeof(GetPaymentStateOperationResult);
    }
}
