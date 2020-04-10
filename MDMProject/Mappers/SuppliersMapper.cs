using MDMProject.Models;
using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MDMProject.Mappers
{
    public static class SuppliersMapper
    {
        public static IEnumerable<SupplierViewModel> ToSupplierViewModels(this IEnumerable<User> collection, Dictionary<int, string> equipmentTypes, Dictionary<int, string> helpTypes)
        {
            var result = collection.Select(x => x.ToSupplierViewModel(equipmentTypes, helpTypes));
            return result;
        }

        public static SupplierViewModel ToSupplierViewModel(this User user, Dictionary<int, string> equipmentTypes, Dictionary<int, string> helpTypes)
        {
            var viewModel = new SupplierViewModel();
            viewModel.Id = user.Id;
            viewModel.Name = user.Name;
            viewModel.Email = user.Email;
            viewModel.PhoneNumber = user.PhoneNumber;
            viewModel.AdditionalComment = user.AdditionalComment;
            viewModel.Address = user.Address.ToAddressViewModel();
            viewModel.OfferedEquipment = user.OfferedEquipment.ToProtectiveEquipmentViewModels(equipmentTypes);
            viewModel.OfferedHelp = user.OfferedHelp.ToOfferedHelpViewModels(helpTypes);

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
    }
}