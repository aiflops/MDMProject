using MDMProject.Models;
using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MDMProject.Mappers
{
    public static class CoordinatorListMapper
    {
        public static IEnumerable<CoordinatorListViewModel> ToCoordinatorListViewModels(this IEnumerable<User> collection, HashSet<int> allCollectionPointIds, HashSet<int> allAdminIds, IDictionary<int, int> coordinatorPeopleCount)
        {
            var result = collection.Select(x => x.ToCoordinatorListViewModel(allCollectionPointIds, allAdminIds, coordinatorPeopleCount));
            return result;
        }

        public static CoordinatorListViewModel ToCoordinatorListViewModel(this User user, HashSet<int> collectionPointIds, HashSet<int> allAdminIds, IDictionary<int, int> coordinatorPeopleCount)
        {
            var viewModel = new CoordinatorListViewModel();
            viewModel.Id = user.Id;
            viewModel.UserType = user.UserType;
            viewModel.Email = user.Email;
            viewModel.PhoneNumber = user.PhoneNumber;
            viewModel.AdditionalComment = user.AdditionalComment;

            viewModel.IndividualName = user.IndividualName;
            viewModel.CompanyName = user.CompanyName;
            viewModel.ContactPersonName = user.ContactPersonName;

            viewModel.CoordinatedRegion = user.CoordinatedRegion;
            viewModel.CoordinatedPeopleCount = coordinatorPeopleCount.ContainsKey(user.Id) ? coordinatorPeopleCount[user.Id] : 0;

            viewModel.IsCollectionPoint = collectionPointIds.Contains(user.Id);
            viewModel.IsAdmin = allAdminIds.Contains(user.Id);

            return viewModel;
        }
    }
}