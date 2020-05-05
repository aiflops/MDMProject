using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class CoordinatorListViewModel
    {
        public int Id { get; set; }

        /* Basic info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_UserType))]
        public UserTypeEnum UserType { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_CompanyName))]
        public string CompanyName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_ContactName))]
        public string ContactPersonName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_IndividualName))]
        public string IndividualName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_Email))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_PhoneNumber))]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Coordinator_CoordinatedRegion))]
        public string CoordinatedRegion { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Coordinator_CoordinatedPeopleCount))]
        public int CoordinatedPeopleCount { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_AdditionalComment))]
        public string AdditionalComment { get; set; }

        public bool IsCollectionPoint { get; set; }

        public bool IsAdmin { get; set; }
    }
}