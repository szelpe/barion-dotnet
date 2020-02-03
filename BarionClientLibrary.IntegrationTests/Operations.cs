using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.FinishReservation;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.Refund;
using BarionClientLibrary.Operations.StartPayment;
using System;
using System.Globalization;
using System.Linq;
using BarionClientLibrary.Operations.Capture;

namespace BarionClientLibrary.IntegrationTests
{
    class Operations
    {
        public const string POSTransactionId = "T1";

        public static GetPaymentStateOperationResult GetPaymentState(BarionClient barionClient, StartPaymentOperationResult result)
        {
            var paymentStateOperation = new GetPaymentStateOperation
            {
                PaymentId = result.PaymentId
            };

            var statusresult = barionClient.ExecuteAsync<GetPaymentStateOperationResult>(paymentStateOperation).Result;

            if (!statusresult.IsOperationSuccessful)
                throw new Exception("Get payment state operation was not successful.");

            return statusresult;
        }

        public static StartPaymentOperationResult StartPayment(BarionClient barionClient, BarionSettings settings, PaymentType paymentType, TimeSpan? reservationPeriod = null, bool initiateRecurrence = false, string recurrenceId = null)
        {
            var startPaymentOperation = new StartPaymentOperation
            {
                GuestCheckOut = true,
                PaymentType = paymentType,
                ReservationPeriod = reservationPeriod,
                FundingSources = new[] { FundingSourceType.All },
                PaymentRequestId = "P1",
                OrderNumber = "1_0",
                Currency = Currency.HUF,
                CallbackUrl = "http://index.hu",
                Locale = CultureInfo.CurrentCulture,
                RedirectUrl = "http://index.hu",
                InitiateRecurrence = initiateRecurrence,
                RecurrenceId = recurrenceId
            };

            var transaction = new PaymentTransaction
            {
                Payee = settings.Payee,
                POSTransactionId = POSTransactionId,
                Total = new decimal(1000),
                Comment = "comment"
            };

            var item = new Item
            {
                Name = "Test",
                Description = "Test",
                ItemTotal = new decimal(1000),
                Quantity = 1,
                Unit = "piece",
                UnitPrice = new decimal(1000),
                SKU = "SKU"
            };

            transaction.Items = new[] { item };
            startPaymentOperation.Transactions = new[] { transaction };

            Console.WriteLine("Sending StartPayment...");
            var result = barionClient.ExecuteAsync<StartPaymentOperationResult>(startPaymentOperation).Result;

            if (!result.IsOperationSuccessful)
                throw new Exception("Start payment operation was not successful.");

            return result;
        }

        public static RefundOperationResult Refund(BarionClient barionClient, StartPaymentOperationResult result)
        {
            var refundOpertation = new RefundOperation();

            refundOpertation.PaymentId = result.PaymentId;

            var transactionToRefund = new TransactionToRefund();
            transactionToRefund.TransactionId = result.Transactions.Single(t => t.POSTransactionId == POSTransactionId).TransactionId;
            transactionToRefund.AmountToRefund = new decimal(50);
            refundOpertation.TransactionsToRefund = new[] { transactionToRefund };

            Console.WriteLine("Sending Refund...");
            var refundResult = barionClient.ExecuteAsync<RefundOperationResult>(refundOpertation).Result;

            if (!refundResult.IsOperationSuccessful)
                throw new Exception("Refund operation was not successful");

            return refundResult;
        }

        public static FinishReservationOperationResult FinishReservation(BarionClient barionClient, GetPaymentStateOperationResult beforeFinishReservationState)
        {
            var finishReservation = new FinishReservationOperation();

            finishReservation.PaymentId = beforeFinishReservationState.PaymentId;

            var transactionToFinish = new TransactionToFinish();
            transactionToFinish.TransactionId = beforeFinishReservationState.Transactions.Single(t => t.POSTransactionId == POSTransactionId).TransactionId;
            transactionToFinish.Total = 500;

            finishReservation.Transactions = new[] { transactionToFinish };

            Console.WriteLine("Sending FinishReservation...");
            var finishReservationResult = barionClient.ExecuteAsync<FinishReservationOperationResult>(finishReservation).Result;

            if (!finishReservationResult.IsOperationSuccessful)
                throw new Exception("Finish reservation operation was not successful.");

            return finishReservationResult;
        }
        
        public static CaptureOperationResult CapturePayment(BarionClient barionClient, GetPaymentStateOperationResult beforeFinishReservationState)
        {
            var capturePayment = new CaptureOperation
            {
                PaymentId = beforeFinishReservationState.PaymentId
            };

            var transactionToCapture = new TransactionToFinish
            {
                TransactionId = beforeFinishReservationState.Transactions
                    .Single(t => t.POSTransactionId == POSTransactionId).TransactionId,
                Total = 500
            };

            capturePayment.Transactions = new[] { transactionToCapture };

            Console.WriteLine("Sending Capture...");
            var captureResult = barionClient.ExecuteAsync<CaptureOperationResult>(capturePayment).Result;

            if (!captureResult.IsOperationSuccessful)
                throw new Exception("Capture operation was not successful.");

            return captureResult;
        }
    }
}
