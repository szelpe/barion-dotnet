using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarionClientLibrary;
using BarionClientLibrary.Operations.Capture;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.FinishReservation;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;

namespace BarionClientTester
{
  public class CapturePayment
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
        var startPaymentOperation = new StartPaymentOperation
        {
          GuestCheckOut = true,
          PaymentType = PaymentType.DelayedCapture,
          DelayedCapturePeriod = TimeSpan.FromDays(1),
          FundingSources = new[] { FundingSourceType.All },
          PaymentRequestId = "P1",
          OrderNumber = "1_0",
          Currency = Currency.EUR,
          CallbackUrl = "http://index.sk",
          Locale = CultureInfo.CurrentCulture,
          RedirectUrl = "http://index.sk"
        };

        var transaction = new PaymentTransaction
        {
          Payee = settings.Payee,
          POSTransactionId = "T1",
          Total = new decimal(50),
          Comment = "comment"
        };

        var item = new Item
        {
          Name = "Test",
          Description = "Test",
          ItemTotal = new decimal(50),
          Quantity = 1,
          Unit = "piece",
          UnitPrice = new decimal(50),
          SKU = "SKU"
        };

        transaction.Items = new[] { item };
        startPaymentOperation.Transactions = new[] { transaction };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Sending StartPayment...");
        var result = barionClient.ExecuteAsync<StartPaymentOperationResult>(startPaymentOperation).Result;
        Console.ResetColor();
        Console.WriteLine("StartPayment result:");
        Console.ForegroundColor = result.IsOperationSuccessful ? ConsoleColor.Green : ConsoleColor.Red;
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

        var statusOperation = new GetPaymentStateOperation
        {
          PaymentId = result.PaymentId
        };

        var result2 = barionClient.ExecuteAsync<GetPaymentStateOperationResult>(statusOperation).Result;

        Console.ResetColor();
        Console.WriteLine("GetPaymentState result:");
        Console.ForegroundColor = result.IsOperationSuccessful ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"\tSuccess: {result2.IsOperationSuccessful}");
        Console.WriteLine($"\tPaymentId: {result2.PaymentId}");
        Console.WriteLine($"\tStatus: {result2.Status}");

        var capturePayment = new CaptureOperation
        {
          PaymentId = result2.PaymentId
        };

        var transactionToFinish = new TransactionToFinish
        {
          TransactionId = result2.Transactions.Single(t => t.POSTransactionId == "T1").TransactionId,
          Total = 50
        };

        capturePayment.Transactions = new[] { transactionToFinish };

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Sending capture...");

        var result3 = barionClient.ExecuteAsync<CaptureOperationResult>(capturePayment).Result;

        Console.ResetColor();
        Console.WriteLine("Capture result:");
        Console.ForegroundColor = result.IsOperationSuccessful ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"\tSuscess: {result3.IsOperationSuccessful}");
        Console.WriteLine($"\tPaymentId: {result3.PaymentId}");
        Console.WriteLine($"\tStatus: {result3.Status}");
      }
    }
  }
}
