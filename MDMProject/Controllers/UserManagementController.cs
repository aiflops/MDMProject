using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.Models;
using MDMProject.Resources;
using MDMProject.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Constants = MDMProject.Models.Constants;

namespace MDMProject.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE_NAME + "," + Constants.COORDINATOR_ROLE_NAME)]
    public class UserManagementController : AdminControllerBase
    {
        [HttpGet]
        public ActionResult Details(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return View("Details", userViewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return PartialView("Delete", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveDelete(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = GetUserById(id, db);

                db.Users.Remove(user);
                db.SaveChanges();

                await SendEmail(user.Email, "Potwierdzenie usunięcia konta", "Twoje konto w serwisie " + GetSiteUrl() + " zostało usunięte!");

                return Json(new { success = true, message = "Usunięto użytkownika!" });
            }
        }

        [HttpGet]
        public ActionResult Approve(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return PartialView("Approve", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveApprove(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = GetUserById(id, db);
                var approvingUser = GetUserById(CurrentUser.Id, db);

                user.ApprovedBy = approvingUser;
                user.ApprovedDate = DateTime.Now;

                db.SaveChanges();

                await SendEmail(user.Email, "Twoje konto zostało zweryfikowane", "Twoje konto w serwisie " + GetSiteUrl() + " zostało zweryfikowane!<br>Twój profil będzie teraz widoczny na mapie.");

                return Json(new { success = true, message = "Zweryfikowano użytkownika!" });
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public ActionResult AddAsCoordinator(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return PartialView("AddAsCoordinator", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public async Task<ActionResult> SaveAddAsCoordinator(int id, string coordinatedRegion)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = GetUserById(id, db);

                user.CoordinatedRegion = coordinatedRegion;

                db.SaveChanges();
            }

            if (!string.IsNullOrWhiteSpace(coordinatedRegion))
            {
                var result = UserManager.AddToRole(id, Constants.COORDINATOR_ROLE_NAME);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Dodano użytkownika do Koordynatorów!" });
                }
                else
                {
                    return Json(new { success = false, message = string.Join("<br>", result.Errors) });
                }
            }
            return Json(new { success = false, message = "Błąd! Pole koordynowany region nie zostało uzupełnione!" });
        }

        [HttpGet]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public ActionResult RemoveAsCoordinator(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return PartialView("RemoveAsCoordinator", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public async Task<ActionResult> SaveRemoveAsCoordinator(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var coordinatedPeopleCount = db
                    .GetAllCollectionPoints()
                    .Count(x => x.CoordinatorId == id);

                if (coordinatedPeopleCount > 0)
                    return Json(new { success = false, message = "Nie można usunąć z roli, ponieważ ma koordynowane osoby. <br>Zmień kordynatora dla koordynowanych osób i spróbuj ponownie." });
            }

            var result = UserManager.RemoveFromRole(id, Constants.COORDINATOR_ROLE_NAME);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Usunięto użytkownika z Koordynatorów!" });
            }
            else
            {
                return Json(new { success = false, message = string.Join("<br>", result.Errors) });
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public ActionResult AddAsAdmin(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return PartialView("AddAsAdmin", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public async Task<ActionResult> SaveAddAsAdmin(int id)
        {
            var result = UserManager.AddToRole(id, Constants.ADMIN_ROLE_NAME);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Dodano użytkownika do Administratorów!" });
            }
            else
            {
                return Json(new { success = false, message = string.Join("<br>", result.Errors) });
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        public ActionResult RemoveAsAdmin(int id)
        {
            var userViewModel = GetUserViewModelById(id);
            return PartialView("RemoveAsAdmin", userViewModel);
        }

        [HttpPost]
        [Authorize(Roles = Constants.ADMIN_ROLE_NAME)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveRemoveAsAdmin(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var otherAdministratorsCount = db
                    .GetAllAdministrators()
                    .Count(x => x.Id != id);

                if (otherAdministratorsCount == 0)
                    return Json(new { success = false, message = "Nie można usunąć z roli, ponieważ w systemie musi pozostać chociaż 1 administrator." });
            }

            var result = UserManager.RemoveFromRole(id, Constants.ADMIN_ROLE_NAME);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Usunięto użytkownika z Administratorów!" });
            }
            else
            {
                return Json(new { success = false, message = string.Join("<br>", result.Errors) });
            }
        }

        private UserListViewModel GetUserViewModelById(int id)
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
                return userViewModel;
            }
        }

        private User GetUserById(int id, ApplicationDbContext db)
        {
            return db.Users.Where(x => x.Id == id).FirstOrDefault();
        }

        private async Task SendEmail(string email, string title, string message)
        {
            var emailService = new EmailService();
            await emailService.SendAsync(new IdentityMessage
            {
                Subject = title,
                Body = message + "<br><br>Pozdrawiamy, <br>" + EmailResources.EmailFrom,
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