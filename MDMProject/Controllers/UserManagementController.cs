using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.Resources;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    //[Authorize(Roles = Constants.ADMIN_ROLE_NAME + "," + Constants.COORDINATOR_ROLE_NAME)]
    public class UserManagementController : AdminControllerBase
    {
        [HttpGet]
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

        [HttpGet]
        public ActionResult Delete(int id)
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

                return View("Delete", userViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveDelete(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Where(x => x.Id == id).FirstOrDefault();

                db.Users.Remove(user);
                db.SaveChanges();

                await SendEmail(user.Email, "Potwierdzenie usunięcia konta", "Twoje konto w serwisie " + GetSiteUrl() + " zostało usunięte!");

                return Json(new { success = true, message = "Usunięto użytkownika!" });
            }
        }

        private async Task SendEmail(string email, string title, string message)
        {
            var emailService = new EmailService();
            await emailService.SendAsync(new IdentityMessage
            {
                Subject = title,
                Body = message + "<br>Pozdrawiamy, " + EmailResources.EmailFrom,
                Destination = email
            });
        }

        private string GetSiteUrl()
        {
            string baseUrl = Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            return baseUrl;
        }
    }
}