using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class CoordinatorListViewModel
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

        public string CoordinatedRegion { get; set; }

        public int CoordinatedPeopleCount { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_AdditionalComment))]
        public string AdditionalComment { get; set; }

        public bool IsCollectionPoint { get; set; }

        public bool IsAdmin { get; set; }
    }
}