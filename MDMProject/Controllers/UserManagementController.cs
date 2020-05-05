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
using MDMProject.Helpers;
using Constants = MDMProject.Models.Constants;

namespace MDMProject.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE_NAME + "," + Constants.COORDINATOR_ROLE_NAME)]
    public class UserManagementController : AdminControllerBase
    {
        [HttpGet]
        public ActionResult CreateAdmin()
        {
            var userViewModel = new CreateUserViewModel
            {
                IsAdmin = true,
                IsCoordinator = false
            };

            return View("Create", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveCreateAdmin(CreateUserViewModel model)
        {
            var actionResult = await CreateUser(model, Constants.ADMIN_ROLE_NAME);

            return actionResult ?? View("Create", model);
        }

        private async Task<ActionResult> CreateUser(CreateUserViewModel model, string roleName)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var tempPassword = GenerateTempPassword(3, 3, 3);
                var result = await UserManager.CreateAsync(user, tempPassword);

                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, roleName);

                    using (var db = new ApplicationDbContext())
                    {
                        var userToUpdate = db.Users.Find(user.Id);

                        /* UPDATE BASIC INFO */
                        userToUpdate.UserType = model.UserType;
                        userToUpdate.PhoneNumber = GetString(model.PhoneNumber);
                        userToUpdate.AdditionalComment = GetString(model.AdditionalComment);

                        userToUpdate.IndividualName = model.UserType == UserTypeEnum.Individual ? GetString(model.IndividualName) : null;
                        userToUpdate.CompanyName = model.UserType == UserTypeEnum.Company ? GetString(model.CompanyName) : null;
                        userToUpdate.ContactPersonName = model.UserType == UserTypeEnum.Company ? GetString(model.ContactPersonName) : null;

                        userToUpdate.CreatedDate = DateTime.Now;
                        userToUpdate.ProfileFinishedDate = DateTime.Now;
                        userToUpdate.UserAccountState = UserAccountState.UsingTempPassword;

                        userToUpdate.CoordinatedRegion = model.CoordinatedRegion;

                        userToUpdate.ApprovedBy = db.Users.Find(User.Identity.GetUserId<int>());
                        userToUpdate.ApprovedDate = DateTime.Now;

                        await db.SaveChangesAsync();
                    }

                    await SendEmail(model.Email, "Konto utworzone przez administratora",
                        "Twoje konto w serwisie " + GetSiteUrl() + " zostało utworzone przez administratora!<br>" +
                        "Po zalogowaniu się zostaniesz poproszony o zmianę hasła.<br>" +
                        "Twoje tymczasowe hasło: <strong>" + tempPassword + "</strong><br><br>" +
                        $"Przejdź do strony logowania: <a href=\"{Url.AbsoluteAction("Login", "Account")}\">ZALOGUJ SIĘ</a>"
                        );

                    TempData["Message"] = $"Konto użytkownika dla {model.Email} zostało utworzone.";
                    if (model.IsCoordinator)
                    {
                        return RedirectToAction("CoordinatorsList", "Admin");
                    }
                    return RedirectToAction("AdministratorsList", "Admin");
                }
                AddErrors(result);
            }

            return null;
        }

        [HttpGet]
        public ActionResult CreateCoordinator()
        {
            var userViewModel = new CreateUserViewModel
            {
                IsAdmin = false,
                IsCoordinator = true
            };

            return View("Create", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveCreateCoordinator(CreateUserViewModel model)
        {
            var actionResult = await CreateUser(model, Constants.COORDINATOR_ROLE_NAME);

            return actionResult ?? View("Create", model);
        }

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
            return db.Users.FirstOrDefault(x => x.Id == id);
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
            string baseUrl = Url.AbsoluteAction("Index", "Home");
            return baseUrl;
        }

        private string GenerateTempPassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private static string GetString(string value)
        {
            var result = value?.Trim();
            return result;
        }
    }
}