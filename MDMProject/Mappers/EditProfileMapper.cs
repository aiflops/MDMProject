using MDMProject.Data;
using MDMProject.Models;
using MDMProject.Resources;
using MDMProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MDMProject.Mappers
{
    public static class EditProfileMapper
    {
        public static EditProfileViewModel ToEditProfileViewModel(this User user, IEnumerable<User> allCoordinators)
        {
            var coordinatorId = user.Coordinator?.Id ?? (!string.IsNullOrWhiteSpace(user.OtherCoordinatorDetails) ? Constants.OTHER_COORDINATOR_ID : default(int?));

            EditProfileViewModel viewModel = new EditProfileViewModel
            {
                UserType = user.UserType,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AdditionalComment = user.AdditionalComment,

                IndividualName = user.IndividualName,
                CompanyName = user.CompanyName,
                ContactPersonName = user.ContactPersonName,
                
                CoordinatorId = coordinatorId, // if no coordinator selected
                OtherCoordinatorDetails = user.OtherCoordinatorDetails, // if no coordinator selected
                CoordinatorsSelectList = allCoordinators.ToCoordinatorsSelectList(coordinatorId),

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
            var allCoordinators = db.GetAllCoordinators().ToList();

            /* UPDATE BASIC INFO */
            user.UserType = viewModel.UserType;
            user.Email = GetString(viewModel.Email);
            user.UserName = user.Email; /* UserName (login credentials) will always be the same as email */
            user.PhoneNumber = GetString(viewModel.PhoneNumber);
            user.ProfileFinishedDate = user.ProfileFinishedDate ?? DateTime.Now; /* If any change to profile is made, mark it as finished */
            user.AdditionalComment = GetString(viewModel.AdditionalComment);

            user.IndividualName = GetString(viewModel.IndividualName);
            user.CompanyName = GetString(viewModel.CompanyName);
            user.ContactPersonName = GetString(viewModel.ContactPersonName);

            var userCoordinator = viewModel.CoordinatorId != null ? allCoordinators.First(x => x.Id == viewModel.CoordinatorId) : null;
            user.CoordinatorId = userCoordinator.Id;
            user.Coordinator = userCoordinator;
            user.OtherCoordinatorDetails = viewModel.OtherCoordinatorDetails;

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

        public static List<SelectListItem> ToCoordinatorsSelectList(this IEnumerable<User> coordinatorsList, int? selectedId)
        {
            var coordinatorsSelectList = new List<SelectListItem>();

            // Add placeholder for "Other"
            coordinatorsSelectList.Add(new SelectListItem
            {
                Value = Constants.OTHER_COORDINATOR_ID.ToString(),
                Text = PropertyNames.EditProfileViewModel_OtherCoordinator,
                Selected = selectedId == Constants.OTHER_COORDINATOR_ID
            });

            // Add other coordinators select list
            var collection = coordinatorsList.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FullUserName,
                Selected = x.Id == selectedId
            }).OrderBy(x => x.Text);

            coordinatorsSelectList.AddRange(collection);

            return coordinatorsSelectList;
        }

        private static string GetString(string value)
        {
            var result = value?.Trim();
            return result;
        }
    }
}