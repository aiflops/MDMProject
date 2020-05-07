using MDMProject.Models;
using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MDMProject.Mappers
{
    public static class CollectionPointMapper
    {
        public static IEnumerable<CollectionPointViewModel> ToCollectionPointViewModels(this IEnumerable<User> collection)
        {
            var result = collection.Select(x => x.ToCollectionPointViewModel());
            return result;
        }

        public static CollectionPointViewModel ToCollectionPointViewModel(this User user)
        {
            var viewModel = new CollectionPointViewModel();
            viewModel.Id = user.Id;
            viewModel.UserType = user.UserType;
            viewModel.CompanyName = user.CompanyName;
            viewModel.PersonName = user.UserType == UserTypeEnum.Individual ? user.IndividualName : user.ContactPersonName;
            viewModel.Email = user.Email;
            viewModel.PhoneNumber = user.PhoneNumber;
            viewModel.Address = user.Address.ToAddressViewModel();

            return viewModel;
        }

        public static IEnumerable<CollectionPointListViewModel> ToCollectionPointListViewModels(this IEnumerable<User> collection)
        {
            var result = collection.Select(x => x.ToCollectionPointListViewModel());
            return result;
        }

        public static CollectionPointListViewModel ToCollectionPointListViewModel(this User user)
        {
            var viewModel = new CollectionPointListViewModel();
            viewModel.Id = user.Id;
            viewModel.Name = user.UserType == UserTypeEnum.Individual ? GetIndividualShortName(user.IndividualName) : GetCompanyShortName(user.CompanyName, user.ContactPersonName);
            viewModel.City = user.Address.City;
            viewModel.Latitude = user.Address.Latitude;
            viewModel.Longitude = user.Address.Longitude;
           
            return viewModel;
        }

        private static AddressViewModel ToAddressViewModel(this Address address)
        {
            if (address == null) return null;

            var viewModel = new AddressViewModel();
            viewModel.City = address.City;
            viewModel.StreetName = address.StreetName;
            viewModel.HouseNumber = address.HouseNumber;
            viewModel.FlatNumber = address.FlatNumber;
            viewModel.PostalCode = address.PostalCode;
            viewModel.Latitude = address.Latitude;
            viewModel.Longitude = address.Longitude;

            return viewModel;
        }

        public static IEnumerable<ProtectiveEquipmentViewModel> ToProtectiveEquipmentViewModels(this IEnumerable<ProtectiveEquipment> collection, Dictionary<int, string> equipmentTypes)
        {
            var result = collection.Select(x => x.ToProtectiveEquipmentViewModel(equipmentTypes));
            return result.Any() ? result : null;
        }

        public static ProtectiveEquipmentViewModel ToProtectiveEquipmentViewModel(this ProtectiveEquipment item, Dictionary<int, string> equipmentTypes)
        {
            var viewModel = new ProtectiveEquipmentViewModel();
            viewModel.Name = item.EquipmentTypeId.HasValue ? equipmentTypes[item.EquipmentTypeId.Value] : item.Name;
            viewModel.Amount = item.Amount;
            viewModel.Comment = item.Comment;
            return viewModel;
        }

        public static IEnumerable<OfferedHelpViewModel> ToOfferedHelpViewModels(this IEnumerable<OfferedHelp> collection, Dictionary<int, string> helpTypes)
        {
            var result = collection.Select(x => x.ToOfferedHelpViewModel(helpTypes));
            return result.Any() ? result : null;
        }

        public static OfferedHelpViewModel ToOfferedHelpViewModel(this OfferedHelp item, Dictionary<int, string> helpTypes)
        {
            var viewModel = new OfferedHelpViewModel();
            viewModel.Name = item.HelpTypeId.HasValue ? helpTypes[item.HelpTypeId.Value] : item.Name;
            viewModel.Description = item.Description;
            return viewModel;
        }

        private static string GetCompanyShortName(string companyName, string contactPersonName)
        {
            return companyName + (!string.IsNullOrWhiteSpace(contactPersonName) ? " - " + GetIndividualShortName(contactPersonName) : "");
        }

        // Get only first name or full name if all is provided
        private static string GetIndividualShortName(string fullIndividualPersonName)
        {
            if (fullIndividualPersonName == null) return null;
            return fullIndividualPersonName.Contains(" ") ? fullIndividualPersonName.Split(' ')[0] : fullIndividualPersonName;
        }
    }
}