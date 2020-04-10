using MDMProject.Data;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public class FakeDataController : Controller
    {
        public ActionResult Reset()
        {
            using (var context = new ApplicationDbContext())
            {
                DatabaseInitializer.DropAllUsers(context);
                DatabaseInitializer.AddFakeData(context);
            }

            return Content("Success");
        }

        public ActionResult RemoveAllUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                DatabaseInitializer.DropAllUsers(context);
            }

            return Content("Success");
        }

        public ActionResult AddFakeUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                DatabaseInitializer.AddFakeData(context);
            }

            return Content("Success");
        }
    }
}