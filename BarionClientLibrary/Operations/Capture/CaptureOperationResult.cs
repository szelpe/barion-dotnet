using System;
using BarionClientLibrary.Operations.Common;

namespace BarionClientLibrary.Operations.Capture
{
    /// <summary>
    /// Represents the result of the capture payment operation.
    /// </summary>
    public class CaptureOperationResult : BarionOperationResult
    {
        public bool IsSuccessful { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentRequestId { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentTransactionResponse[] Transactions { get; set; }
    }
}