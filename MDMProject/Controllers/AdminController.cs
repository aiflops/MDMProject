using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE_NAME + "," + Constants.COORDINATOR_ROLE_NAME)]
    public class AdminController : AdminControllerBase
    {
        public ActionResult Index()
        {
            using (var db = new ApplicationDbContext())
            {
                var allCollectionPointUsers = db.GetAllCollectionPoints()
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator)
                    .OrderBy(x => x.UserType)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactPersonName)
                    .ThenBy(x => x.IndividualName);

                var allCoordinatorIds = db.GetAllCoordinators().Select(x => x.Id).ToHashSet();
                var allAdminIds = db.GetAllAdministrators().Select(x => x.Id).ToHashSet();

                var viewModelsList = allCollectionPointUsers.ToUserListViewModels(allCoordinatorIds, allAdminIds).ToList();

                ViewBag.ListTitle = "Użytkownicy";

                return View("CollectionPointsList", viewModelsList);
            }    
        }

        public ActionResult ActiveCollectionPointsList()
        {
            using (var db = new ApplicationDbContext())
            {
                var allCollectionPointUsers = db.GetAllCollectionPoints()
                    .Where(x => x.ProfileFinishedDate != null && x.ApprovedBy != null)
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator)
                    .OrderBy(x => x.UserType)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactPersonName)
                    .ThenBy(x => x.IndividualName);

                var allCoordinatorIds = db.GetAllCoordinators().Select(x => x.Id).ToHashSet();
                var allAdminIds = db.GetAllAdministrators().Select(x => x.Id).ToHashSet();

                var viewModelsList = allCollectionPointUsers.ToUserListViewModels(allCoordinatorIds, allAdminIds).ToList();

                ViewBag.ListTitle = "Zweryfikowani użytkownicy";

                return View("CollectionPointsList", viewModelsList);
            }
        }

        public ActionResult UnfinishedCollectionPointsList()
        {
            using (var db = new ApplicationDbContext())
            {
                var allCollectionPointUsers = db.GetAllCollectionPoints()
                    .Where(x => x.ProfileFinishedDate == null)
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator)
                    .OrderBy(x => x.UserType)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactPersonName)
                    .ThenBy(x => x.IndividualName);

                var allCoordinatorIds = db.GetAllCoordinators().Select(x => x.Id).ToHashSet();
                var allAdminIds = db.GetAllAdministrators().Select(x => x.Id).ToHashSet();

                var viewModelsList = allCollectionPointUsers.ToUserListViewModels(allCoordinatorIds, allAdminIds).ToList();

                ViewBag.ListTitle = "Nieukończone profile";

                return View("CollectionPointsList", viewModelsList);
            }
        }

        public ActionResult UnverifiedCollectionPointsList()
        {
            using (var db = new ApplicationDbContext())
            {
                var allCollectionPointUsers = db.GetAllCollectionPoints()
                    .Where(x =>  x.ProfileFinishedDate != null && x.ApprovedBy == null)
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator)
                    .OrderBy(x => x.UserType)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactPersonName)
                    .ThenBy(x => x.IndividualName);

                var allCoordinatorIds = db.GetAllCoordinators().Select(x => x.Id).ToHashSet();
                var allAdminIds = db.GetAllAdministrators().Select(x => x.Id).ToHashSet();

                var viewModelsList = allCollectionPointUsers.ToUserListViewModels(allCoordinatorIds, allAdminIds).ToList();

                ViewBag.ListTitle = "Niezweryfikowani użytkownicy";

                return View("CollectionPointsList", viewModelsList);
            }
        }

        public ActionResult CoordinatorsList()
        {
            using (var db = new ApplicationDbContext())
            {
                var allCollectionPointUsers = db.GetAllCoordinators()
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator)
                    .OrderBy(x => x.UserType)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactPersonName)
                    .ThenBy(x => x.IndividualName);

                var allCollectionPoints = db.GetAllCollectionPoints().ToList();
                var allAdminIds = db.GetAllAdministrators().Select(x => x.Id).ToHashSet();
                var allCollectionPointIds = allCollectionPoints.Select(x => x.Id).ToHashSet();
                var coordinatedPeopleDict = allCollectionPoints
                    .Where(x => x.CoordinatorId.HasValue)
                    .GroupBy(x => x.CoordinatorId.Value)
                    .ToDictionary(x => x.Key, x => x.Count());

                var viewModelsList = allCollectionPointUsers.ToCoordinatorListViewModels(allCollectionPointIds, allAdminIds, coordinatedPeopleDict).ToList();

                ViewBag.ListTitle = "Koordynatorzy";

                return View("CoordinatorsList", viewModelsList);
            }
        }

        public ActionResult AdministratorsList()
        {
            using (var db = new ApplicationDbContext())
            {
                var allCollectionPointUsers = db.GetAllAdministrators()
                    .Include(x => x.Address)
                    .Include(x => x.ApprovedBy)
                    .Include(x => x.Coordinator)
                    .OrderBy(x => x.UserType)
                    .ThenBy(x => x.CompanyName)
                    .ThenBy(x => x.ContactPersonName)
                    .ThenBy(x => x.IndividualName);

                var allCoordinatorIds = db.GetAllCoordinators().Select(x => x.Id).ToHashSet();
                var allCollectionPointIds = db.GetAllCollectionPoints().Select(x => x.Id).ToHashSet();

                var viewModelsList = allCollectionPointUsers.ToAdminListViewModels(allCollectionPointIds, allCoordinatorIds).ToList();

                ViewBag.ListTitle = "Administratorzy";

                return View("AdministratorsList", viewModelsList);
            }
        }
    }
}