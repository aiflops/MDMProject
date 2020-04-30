using MDMProject.Models;
using MDMProject.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class UserListViewModel
    {
        public int Id { get; set; }

        /* Basic info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_UserType))]
        public UserTypeEnum UserType { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_CompanyName))]
        public string CompanyName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_ContactName))]
        public string ContactPersonName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_UserName))]
        public string IndividualName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_Email))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_PhoneNumber))]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_Coordinator))]
        public string CoordinatorName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_OtherCoordinatorDetails))]
        public string OtherCoordinatorDetails { get; set; }

        /* Address */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_City))]
        public string City { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_StreetName))]
        public string StreetName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_HouseNumber))]
        public string HouseNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_FlatNumber))]
        public string FlatNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_PostalCode))]
        public string PostalCode { get; set; }

        /* Additional info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_AdditionalComment))]
        public string AdditionalComment { get; set; }

        public DateTime? ProfileFinishedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsCoordinator { get; internal set; }
        public bool IsAdmin { get; internal set; }
    }
}