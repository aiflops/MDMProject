using MDMProject.Data;
using MDMProject.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public abstract class AdminControllerBase : ControllerBase
    {
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().Get<ApplicationUserManager>();

        public User CurrentUser => UserManager.FindByEmailAsync(User.Identity.Name).Result;

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                var viewResult = (ViewResult)filterContext.Result;
                using (var db = new ApplicationDbContext())
                {
                    viewResult.ViewBag.ActiveUsersCount = db.GetAllCollectionPoints().Count(x => x.ProfileFinishedDate != null && x.ApprovedBy != null);
                    viewResult.ViewBag.UnverifiedUsersCount = db.GetAllCollectionPoints().Count(x => x.ProfileFinishedDate != null && x.ApprovedBy == null);
                    viewResult.ViewBag.UnfinishedUsersCount = db.GetAllCollectionPoints().Count(x => x.ProfileFinishedDate == null);
                }

                var userManager = filterContext.RequestContext.HttpContext.GetOwinContext().Get<ApplicationUserManager>();
                viewResult.ViewBag.CurrentUser = userManager.FindByNameAsync(User.Identity.Name).Result;
            }

            base.OnResultExecuting(filterContext);
        }
    }
}