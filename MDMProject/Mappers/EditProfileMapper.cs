using MDMProject.Data;
using MDMProject.Models;
using MDMProject.ViewModels;
using System.Linq;
using System.Web;

namespace MDMProject.Mappers
{
    public static class EditProfileMapper
    {
        public static EditProfileViewModel ToEditProfileViewModel(this User user)
        {
            EditProfileViewModel viewModel = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                HasMaskAvailable = HasMaskAvailable(user),
                HasAdapterAvailable = HasAdapterAvailable(user),
                AdditionalComment = user.AdditionalComment,

                City = user.Address?.City,
                StreetName = user.Address?.StreetName,
                HouseNumber = user.Address?.HouseNumber,
                FlatNumber = user.Address?.FlatNumber,
                PostalCode = user.Address?.PostalCode,
                Latitude = user.Address?.Latitude,
                Longitude = user.Address?.Longitude
            };

            return viewModel;
        }

        public static void UpdateWith(this User user, EditProfileViewModel viewModel, ApplicationDbContext db)
        {
            /* UPDATE BASIC INFO */
            user.Name = GetString(viewModel.Name);
            user.Email = GetString(viewModel.Email);
            user.UserName = user.Email; /* UserName (login credentials) will always be the same as email */
            user.PhoneNumber = GetString(viewModel.PhoneNumber);
            user.AdditionalComment = GetString(viewModel.AdditionalComment);
            user.IsProfileFinished = true; /* If any change to profile is made, mark it as finished */

            /* UPDATE MASK */
            if (viewModel.HasMaskAvailable && !HasMaskAvailable(user))
            {
                // Add mask
                var maskType = GetMaskType(db);
                user.OfferedEquipment.Add(new ProtectiveEquipment { EquipmentType = maskType });
            }
            else if (!viewModel.HasMaskAvailable && HasMaskAvailable(user))
            {
                // Remove mask
                var offeredMask = GetMask(user);
                db.ProtectiveEquipments.Remove(offeredMask);
            }

            /* UPDATE ADAPTER */
            if (viewModel.HasAdapterAvailable && !HasAdapterAvailable(user))
            {
                // Add adapter
                var adapterType = GetAdapterType(db);
                user.OfferedHelp.Add(new OfferedHelp { HelpType = adapterType });
            }
            else if (!viewModel.HasAdapterAvailable && HasAdapterAvailable(user))
            {
                // Remove adapter
                var offeredAdapter = GetAdapter(user);
                db.OfferedHelps.Remove(offeredAdapter);
            }

            /* UPDATE ADDRESS */
            // Create new address if not exists
            user.Address = user.Address ?? new Address();

            // Update address properties
            user.Address.City = GetString(viewModel.City);
            user.Address.StreetName = GetString(viewModel.StreetName);
            user.Address.HouseNumber = GetString(viewModel.HouseNumber);
            user.Address.FlatNumber = GetString(viewModel.FlatNumber);
            user.Address.PostalCode = GetString(viewModel.PostalCode);
            user.Address.Latitude = GetString(viewModel.Latitude);
            user.Address.Longitude = GetString(viewModel.Longitude);
        }

        private static string GetString(string value)
        {
            var result = value?.Trim();
            return result;
        }

        #region Helpers
        private static bool HasMaskAvailable(User user)
        {
            var offeredEquipment = GetMask(user);
            return offeredEquipment != null;
        }

        private static ProtectiveEquipment GetMask(User user)
        {
            return user.OfferedEquipment.FirstOrDefault(x => x.EquipmentType != null && x.EquipmentType.Name == Constants.MASK_NAME);
        }

        private static EquipmentType GetMaskType(ApplicationDbContext db)
        {
            return db.EquipmentTypes.First(x => x.Name == Constants.MASK_NAME);
        }

        private static bool HasAdapterAvailable(User user)
        {
            var offered3dPrinter = GetAdapter(user);
            return offered3dPrinter != null;
        }

        private static OfferedHelp GetAdapter(User user)
        {
            return user.OfferedHelp.FirstOrDefault(x => x.HelpType != null && x.HelpType.Name == Constants.ADAPTER_NAME);
        }

        private static HelpType GetAdapterType(ApplicationDbContext db)
        {
            return db.HelpTypes.First(x => x.Name == Constants.ADAPTER_NAME);
        }
        #endregion
    }
}