using System;
using System.Net.Http;

namespace BarionClientLibrary.Operations.PaymentState
{
    /// <summary>
    /// Represents a get payment state operation.
    /// </summary>
    /// <remarks>
    /// View the full documentation of the operation here: https://doksi.barion.com/Payment-GetPaymentState-v2
    /// </remarks>
    public class GetPaymentStateOperation : BarionOperation
    {
        public Guid PaymentId { get; set; }

        public override Uri RelativeUri => new Uri($"/v2/Payment/GetPaymentState?PaymentId={PaymentId}&POSKey={POSKey}", UriKind.Relative);
        public override HttpMethod Method => HttpMethod.Get;
        public override Type ResultType => typeof(GetPaymentStateOperationResult);
    }
}
