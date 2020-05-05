using Foolproof;
using MDMProject.Models;
using MDMProject.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MDMProject.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        /* Basic info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_UserType))]
        public UserTypeEnum UserType { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_CompanyName))]
        [RequiredIf(nameof(UserType), Operator.EqualTo, UserTypeEnum.Company, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_COMPANY_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_COMPANY_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string CompanyName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_ContactName))]
        [RequiredIf(nameof(UserType), Operator.EqualTo, UserTypeEnum.Company, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_CONTACT_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_CONTACT_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string ContactPersonName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_UserName))]
        [RequiredIf(nameof(UserType), Operator.EqualTo, UserTypeEnum.Individual, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_CONTACT_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_CONTACT_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string IndividualName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_Email))]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.EmailIsIncorrect))]
        [MaxLength(ValidationConstants.User.MAX_EMAIL_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_PhoneNumber))]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(ValidationConstants.User.MAX_PHONE_NUMBER_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.CollectionPoint_Coordinator))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        public int? CoordinatorId { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.CollectionPoint_OtherCoordinatorDetails))]
        [RequiredIf(nameof(CoordinatorId), Operator.EqualTo, Constants.OTHER_COORDINATOR_ID, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MaxLength(ValidationConstants.User.MAX_COORDINATOR_DETAILS_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string OtherCoordinatorDetails { get; set; }

        public List<SelectListItem> CoordinatorsSelectList { get; set; }

        /* Address */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_City))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MaxLength(ValidationConstants.Address.MAX_CITY_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string City { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_StreetName))]
        [MaxLength(ValidationConstants.Address.MAX_STREET_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string StreetName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_HouseNumber))]
        [MaxLength(ValidationConstants.Address.MAX_HOUSE_NUMBER_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string HouseNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_FlatNumber))]
        [MaxLength(ValidationConstants.Address.MAX_FLAT_NUMBER_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string FlatNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Address_PostalCode))]
        [DataType(DataType.PostalCode)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [StringLength(ValidationConstants.Address.POSTAL_CODE_LENGTH, MinimumLength = ValidationConstants.Address.POSTAL_CODE_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.ExactFieldLength))]
        [RegularExpression("[0-9]{2}-[0-9]{3}", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.PostCodeIncorrect))]
        public string PostalCode { get; set; }

        [MaxLength(ValidationConstants.Address.MAX_LATITUDE_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Latitude { get; set; }

        [MaxLength(ValidationConstants.Address.MAX_LONGITUDE_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Longitude { get; set; }

        /* Additional info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_AdditionalComment))]
        [MaxLength(ValidationConstants.User.MAX_ADDITIONAL_COMMENT_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        //[AllowHtml] // TODO: PREVENT SENDING HTML! Instead of sending and encoding
        public string AdditionalComment { get; set; }
    }
}