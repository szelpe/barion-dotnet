using System;
using System.Globalization;
using System.Threading.Tasks;
using BarionClientLibrary;
using BarionClientLibrary.Operations.Common;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.StartPayment;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BarionSampleApplication.Controllers
{
    public class BarionController : Controller
    {
        private readonly BarionClient _barionClient;
        private readonly BarionSettings _barionSettings;


        public BarionController(BarionClient barionClient, BarionSettings barionSettings)
        {
            _barionClient = barionClient;
            _barionSettings = barionSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> StartPayment(Product product)
        {
            var item = new Item
            {
                Name = product.Name,
                Description = product.Name,
                Quantity = 1,
                UnitPrice = product.Price,
                ItemTotal = product.Price,
                Unit = "piece",
                SKU = "SKU"
            };

            var transaction = new PaymentTransaction
            {
                Items = new[] { item },
                POSTransactionId = "T1",
                Payee = _barionSettings.Payee,
                Total = product.Price
            };

            var startPayment = new StartPaymentOperation
            {
                Transactions = new[] { transaction },
                PaymentType = PaymentType.Immediate,
                Currency = Currency.HUF,
                FundingSources = new[] { FundingSourceType.All },
                GuestCheckOut = true,
                Locale = CultureInfo.CurrentCulture,
                OrderNumber = "Order1",
                PaymentRequestId = "R1",
                CallbackUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Url.Action("Callback", "Barion")),
                RedirectUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Url.Action("PaymentFinished", "Home"))
            };

            var result = await _barionClient.ExecuteAsync(startPayment);

            if (result.IsOperationSuccessful)
            {
                var startPaymentReult = result as StartPaymentOperationResult;
                return Redirect(startPaymentReult.GatewayUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Callback(Guid PaymentId)
        {
            var getPaymentState = new GetPaymentStateOperation
            {
                PaymentId = PaymentId
            };

            var result = await _barionClient.ExecuteAsync(getPaymentState);

            if (result.IsOperationSuccessful)
            {
                var paymentState = result as GetPaymentStateOperationResult;

                // Set the order status according to paymentState.Status

                return Json(new { Success = true, Status = paymentState.Status });
            }

            return Json(new { Success = false });
        }
    }
}