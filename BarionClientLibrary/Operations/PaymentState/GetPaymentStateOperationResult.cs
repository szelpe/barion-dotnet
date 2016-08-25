using BarionClientLibrary.Operations.Common;
using System;
using System.Globalization;

namespace BarionClientLibrary.Operations.PaymentState
{
    /// <summary>
    /// Represents the result of get payment state operation.
    /// </summary>
    public class GetPaymentStateOperationResult : BarionOperationResult
    {
        public Guid PaymentId { get; set; }
        public string PaymentRequestId { get; set; }
        public Guid POSId { get; set; }
        public string POSName { get; set; }
        public string POSOwnerEmail { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentType PaymentType { get; set; }
        public FundingSourceType? FundingSource { get; set; }
        public FundingInformation FundingInformation { get; set; }
        public FundingSourceType[] AllowedFundingSources { get; set; }
        public bool GuestCheckout { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ReservedUntil { get; set; }
        public decimal Total { get; set; }
        public TransactionDetail[] Transactions { get; set; }
        public Currency Currency { get; set; }
        public CultureInfo SuggestedLocale { get; set; }
        public double? FraudRiskScore { get; set; }
        public string CallbackUrl { get; set; }
        public string RedirectUrl { get; set; }
    }
}