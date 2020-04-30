using MDMProject.Data;
using System.Linq;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public abstract class AdminControllerBase : ControllerBase
    {
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
            }

            base.OnResultExecuting(filterContext);
        }
    }
}