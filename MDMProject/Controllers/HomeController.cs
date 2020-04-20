using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Rodo()
        {

            return View();
        }

    }
}