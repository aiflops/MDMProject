using MDMProject.Models;
using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MDMProject.Mappers
{
    public static class AdministratorListMapper
    {
        public static IEnumerable<AdminListViewModel> ToAdminListViewModels(this IEnumerable<User> collection, HashSet<int> allCollectionPointIds, HashSet<int> allCoordinatorIds)
        {
            var result = collection.Select(x => x.ToAdminListViewModel(allCollectionPointIds, allCoordinatorIds));
            return result;
        }

        public static AdminListViewModel ToAdminListViewModel(this User user, HashSet<int> collectionPointIds, HashSet<int> coordinatorIds)
        {
            var viewModel = new AdminListViewModel();
            viewModel.Id = user.Id;
            viewModel.UserType = user.UserType;
            viewModel.Email = user.Email;
            viewModel.PhoneNumber = user.PhoneNumber;

            viewModel.IndividualName = user.IndividualName;
            viewModel.CompanyName = user.CompanyName;
            viewModel.ContactPersonName = user.ContactPersonName;

            viewModel.IsCollectionPoint = collectionPointIds.Contains(user.Id);
            viewModel.IsCoordinator = coordinatorIds.Contains(user.Id);

            return viewModel;
        }
    }
}