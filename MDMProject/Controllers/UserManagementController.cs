using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE_NAME + "," + Constants.COORDINATOR_ROLE_NAME)]
    public class UserManagementController : AdminControllerBase
    {
        public ActionResult Details(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Where(x => x.Id == id)
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator).FirstOrDefault();

                var allCoordinatorIds = db.GetAllCoordinators().Select(x => x.Id).ToHashSet();
                var allAdminIds = db.GetAllAdministrators().Select(x => x.Id).ToHashSet();

                var userViewModel = user.ToUserListViewModel(allCoordinatorIds, allAdminIds);

                ViewBag.ListTitle = "Użytkownicy";

                if (Request.UrlReferrer != null)
                {
                    string previousPage = Request.UrlReferrer.ToString();
                    ViewBag.ReturnUrl = previousPage;
                }

                return View("Details", userViewModel);
            }    
        }
    }
}