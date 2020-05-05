using MDMProject.Models;
using MDMProject.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class UserDetailsViewModel
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

        public int? CoordinatorId { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.CollectionPoint_Coordinator))]
        public string CoordinatorName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.CollectionPoint_OtherCoordinatorDetails))]
        public string OtherCoordinatorDetails { get; set; }

        /* Address */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_City))]
        public string City { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_StreetName))]
        public string StreetName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_HouseNumber))]
        public string HouseNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_FlatNumber))]
        public string FlatNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_PostalCode))]
        public string PostalCode { get; set; }

        /* Coordinator details - when coordinator */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Coordinator_CoordinatedRegion))]
        public string CoordinatedRegion { get; set; }

        /* Additional info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_AdditionalComment))]
        public string AdditionalComment { get; set; }

        public DateTime? ProfileFinishedDate { get; set; }
                
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool IsCollectionPoint { get; set; }

        public bool IsCoordinator { get; set; }

        public bool IsAdmin { get; set; }
    }
}