using BarionClientLibrary;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.Refund;
using BarionClientLibrary.Operations.StartPayment;
using BarionClientLibrary.RetryPolicies;
using System;
using System.Globalization;

namespace BarionClientTester
{
    public class Refund
    {
        public static void Run()
        {
            var settings = new BarionSettings
            {
                BaseUrl = new Uri(AppSettings.BarionBaseAddress),
                POSKey = Guid.Parse(AppSettings.BarionPOSKey),
                Payee = AppSettings.BarionPayee
            };

            using (var barionClient = new BarionClient(settings))
            {
                barionClient.RetryPolicy = new NoRetry();
                var startPaymentOperation = new StartPaymentOperation
                {
                    GuestCheckOut = true,
                    PaymentType = PaymentType.Immediate,
                    FundingSources = new[] { FundingSourceType.All },
                    PaymentRequestId = "P1",
                    OrderNumber = "1_0",
                    Currency = Currency.HUF,
                    CallbackUrl = "http://index.hu",
                    Locale = CultureInfo.CurrentCulture,
                    RedirectUrl = "http://index.hu"
                };

                var transaction = new PaymentTransaction
                {
                    Payee = settings.Payee,
                    POSTransactionId = "T1",
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

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sending StartPayment...");
                var result = barionClient.ExecuteAsync<StartPaymentOperationResult>(startPaymentOperation).Result;
                Console.ResetColor();
                Console.WriteLine("StartPayment result:");
                if (result.IsOperationSuccessful)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\tSuccess: {result.IsOperationSuccessful}");
                Console.WriteLine($"\tPaymentId: {result.PaymentId}");
                Console.WriteLine($"\tStatus: {result.Status}");

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Starting the browser with the barion pay page.");

                System.Diagnostics.Process.Start(result.GatewayUrl);

                Console.WriteLine("Press any key to continue the flow...");
                Console.ReadKey();

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sending GetPaymentState...");

                var statusOperation = new GetPaymentStateOperation();
                statusOperation.PaymentId = result.PaymentId;

                var result2 = barionClient.ExecuteAsync<GetPaymentStateOperationResult>(statusOperation).Result;

                Console.ResetColor();
                Console.WriteLine("GetPaymentState result:");
                if (result.IsOperationSuccessful)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\tSuccess: {result2.IsOperationSuccessful}");
                Console.WriteLine($"\tPaymentId: {result2.PaymentId}");
                Console.WriteLine($"\tStatus: {result2.Status}");

                var refundOpertation = new RefundOperation();

                refundOpertation.PaymentId = result.PaymentId;

                var transactionToRefund = new TransactionToRefund();
                transactionToRefund.TransactionId = result.Transactions[0].TransactionId;
                transactionToRefund.AmountToRefund = new decimal(50);
                refundOpertation.TransactionsToRefund = new[] { transactionToRefund };

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sending Refund...");

                var result3 = barionClient.ExecuteAsync<RefundOperationResult>(refundOpertation).Result;

                Console.ResetColor();
                Console.WriteLine("Refund result:");
                if (result.IsOperationSuccessful)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\tSuccess: {result3.IsOperationSuccessful}");
                Console.WriteLine($"\tPaymentId: {result3.PaymentId}");
            }
        }
    }
}
