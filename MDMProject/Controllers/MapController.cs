using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public class MapController : ControllerBase
    {
        // GET: Map
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Map/Embedded
        [HttpGet]
        public ActionResult Embedded()
        {
            ViewBag.IsEmbedded = true;
            return View("Index");
        }

        // GET: Map/Knockout
        [HttpGet]
        public ActionResult Knockout()
        {
            return View();
        }
    }
}