using System;

namespace BarionClientLibrary.Operations.Common
{
    public class PayerAccountInformation
    {
        public string AccountId { get; set; }
        public DateTime? AccountCreated { get; set; }
        public AccountCreationIndicator? AccountCreationIndicator { get; set; }
        public DateTime? AccountLastChanged { get; set; }
        public AccountChangeIndicator? AccountChangeIndicator { get; set; }
        public DateTime? PasswordLastChanged { get; set; }
        public PasswordChangeIndicator? PasswordChangeIndicator { get; set; }
        public int? PurchasesInTheLast6Months { get; set; }
        public DateTime? ShippingAddressAdded { get; set; }
        public ShippingAddressUsageIndicator? ShippingAddressUsageIndicator { get; set; }
        public int? ProvisionAttempts { get; set; }
        public int? TransactionalActivityPerDay { get; set; }
        public int? TransactionalActivityPerYear { get; set; }
        public DateTime? PaymentMethodAdded { get; set; }
        public SuspiciousActivityIndicator? SuspiciousActivityIndicator { get; set; }
    }
}
