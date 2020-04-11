using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public class MapController : ControllerBase
    {
        // GET: Map
        public ActionResult MapIndex()
        {
            return View();
        }
    }
}