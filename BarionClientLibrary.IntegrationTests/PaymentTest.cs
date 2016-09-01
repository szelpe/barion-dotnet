using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;
using BarionClientTester;
using System;
using Xunit;

namespace BarionClientLibrary.IntegrationTests
{
    public class PaymentTests
    {
        private BarionSettings _settings;

        public PaymentTests()
        {
            _settings = new BarionSettings
            {
                BaseUrl = new Uri(AppSettings.BarionBaseAddress),
                POSKey = Guid.Parse(AppSettings.BarionPOSKey),
                Payee = AppSettings.BarionPayee
            };
        }

        [Fact]
        public void ImmediatePayment()
        {
            var barionClient = new BarionClient(_settings);

            var paymentResult = Operations.StartPayment(barionClient, _settings, PaymentType.Immediate);

            BrowserScriptRunner.RunPaymentScript(paymentResult);

            GetPaymentStateOperationResult statusresult = Operations.GetPaymentState(barionClient, paymentResult);

            Assert.Equal(PaymentStatus.Succeeded, statusresult.Status);
        }

        [Fact]
        public void Refund()
        {
            var barionClient = new BarionClient(_settings);

            var paymentResult = Operations.StartPayment(barionClient, _settings, PaymentType.Immediate);

            BrowserScriptRunner.RunPaymentScript(paymentResult);

            GetPaymentStateOperationResult beforeRefundState = Operations.GetPaymentState(barionClient, paymentResult);

            var refundResult = Operations.Refund(barionClient, paymentResult);

            Assert.Equal(1, refundResult.RefundedTransactions.Length);
            Assert.Equal("Succeeded", refundResult.RefundedTransactions[0].Status);

            GetPaymentStateOperationResult afterRefundState = Operations.GetPaymentState(barionClient, paymentResult);

            Assert.Equal(beforeRefundState.Total - refundResult.RefundedTransactions[0].Total, afterRefundState.Total);
        }

        [Fact]
        public void ReserveThenFinishReservation()
        {
            var barionClient = new BarionClient(_settings);

            var paymentResult = Operations.StartPayment(barionClient, _settings, PaymentType.Reservation, TimeSpan.FromDays(1));

            BrowserScriptRunner.RunPaymentScript(paymentResult);

            GetPaymentStateOperationResult beforeFinishReservationState = Operations.GetPaymentState(barionClient, paymentResult);

            Assert.Equal(PaymentStatus.Reserved, beforeFinishReservationState.Status);

            var finishReservationResult = Operations.FinishReservation(barionClient, beforeFinishReservationState);
            
            Assert.Equal(PaymentStatus.Succeeded, finishReservationResult.Status);
        }

        [Fact]
        public void Recurring()
        {
            var barionClient = new BarionClient(_settings);

            var paymentResult = Operations.StartPayment(barionClient, _settings, PaymentType.Immediate, initiateRecurrence: true, recurrenceId: "R");

            BrowserScriptRunner.RunPaymentScript(paymentResult);

            Assert.Equal(RecurrenceResult.Successful, paymentResult.RecurrenceResult);

            var paymentResult2 = Operations.StartPayment(barionClient, _settings, PaymentType.Immediate, initiateRecurrence: false, recurrenceId: "R");

            Assert.Equal(PaymentStatus.Succeeded, paymentResult2.Status);
        }
    }
}
