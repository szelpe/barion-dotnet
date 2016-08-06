using BarionClientLibrary;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;
using System;
using System.Globalization;

namespace BarionClientTester
{
    public class ReservationPayment
    {
        public static void Run()
        {
            var settings = new BarionSettings
            {
                BaseUrl = new Uri("https://api.test.barion.com/"),
                POSKey = Guid.Parse("00000000000000000000000000000000"),
                Payee = "user@example.com"
            };

            using (var barionClient = new BarionClient(settings))
            {
                var startPaymentOperation = new StartPaymentOperation
                {
                    GuestCheckOut = true,
                    PaymentType = PaymentType.Reservation,
                    ReservationPeriod = TimeSpan.FromDays(1),
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
                var result = barionClient.ExecuteAsync(startPaymentOperation).Result as StartPaymentOperationResult;
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

                var result2 = barionClient.ExecuteAsync(statusOperation).Result as GetPaymentStateOperationResult;

                Console.ResetColor();
                Console.WriteLine("GetPaymentState result:");
                if (result.IsOperationSuccessful)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\tSuccess: {result2.IsOperationSuccessful}");
                Console.WriteLine($"\tPaymentId: {result2.PaymentId}");
                Console.WriteLine($"\tStatus: {result2.Status}");
            }
        }
    }
}
