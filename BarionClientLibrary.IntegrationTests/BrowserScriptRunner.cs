using BarionClientLibrary.Operations.StartPayment;
using BarionClientTester;
using NReco.PhantomJS;
using System;
using System.IO;
using System.Reflection;

namespace BarionClientLibrary.IntegrationTests
{
    class BrowserScriptRunner
    {
        public static void RunPaymentScript(StartPaymentOperationResult result)
        {
            var phantomJS = new PhantomJS();

            phantomJS.OutputReceived += (sender, e) =>
            {
                if (e.Data != null)
                    throw new Exception($"Payment script failed: {e.Data}");
            };

            using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BarionClientLibrary.IntegrationTests.PaymentScript.js"))
            using (var streamReader = new StreamReader(fileStream))
            {
                var paymentScript = streamReader.ReadToEnd();

                phantomJS.RunScript(paymentScript, new[] { result.GatewayUrl, AppSettings.BarionPayer, AppSettings.BarionPayerPassword });
            }
        }
    }
}
