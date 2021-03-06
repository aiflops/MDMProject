﻿using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    [Authorize]
    public class ManageController : ControllerBase
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/EditProfile
        public ActionResult EditProfile()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            var viewModel = user.ToEditProfileViewModel();

            return View(viewModel);
        }

        public ActionResult ShowProfile()
        {
            if (TempData["IsSuccess"] != null)
            {
                ViewBag.IsSuccess = true;
                TempData.Remove("IsSuccess");
            }

            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            var viewModel = user.ToEditProfileViewModel();

            return View(viewModel);
        }

        //
        // POST: /Manage/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userId = User.Identity.GetUserId<int>();
                try
                {
                    using (var db = new ApplicationDbContext())
                    {
                        ValidateUserEmail(model, userId, db);
                        ValidateUserCoordinates(model);

                        if (ModelState.IsValid)
                        {
                            // Get user
                            var user = db.Users.Find(userId);
                            user.UpdateWith(model, db);

                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    ModelState.AddModelError("", ex.Message);
                }

                if (ModelState.IsValid)
                {
                    TempData["IsSuccess"] = true;
                    return RedirectToAction("ShowProfile", "Manage");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void ValidateUserEmail(EditProfileViewModel model, int userId, ApplicationDbContext db)
        {
            var userWithSameMailExists = db.Users.Any(x => x.Id != userId && x.Email.ToLower() == model.Email.ToLower());
            if (userWithSameMailExists)
            {
                ModelState.AddModelError(nameof(EditProfileViewModel.Email), "Użytkownik o podanym adresie e-mail już istnieje.");
            }
        }

        private void ValidateUserCoordinates(EditProfileViewModel model)
        {
            if (model.PostalCode != null && (model.Latitude == null || model.Longitude == null))
            {
                ModelState.AddModelError(nameof(EditProfileViewModel.Latitude), "Nie oznaczono lokalizacji na mapie.");
            }
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            AddErrors(result);
            return View(model);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}