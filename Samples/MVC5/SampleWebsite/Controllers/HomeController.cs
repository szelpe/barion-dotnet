using System.Web.Mvc;

namespace SampleWebsite.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentFinished()
        {
            return View();
        }
    }
}