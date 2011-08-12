using System.Web.Mvc;

namespace Website.Controllers
{
    public class IntegrationTestsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViaRenderAction()
        {
            return PartialView();
        }
    }
}