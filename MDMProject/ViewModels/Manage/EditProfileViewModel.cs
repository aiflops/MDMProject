using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MDMProject.ViewModels
{
    public class EditProfileViewModel
    {
        /* Basic info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_Name))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_USER_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_USER_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Name { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_Email))]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.EmailIsIncorrect))]
        [MaxLength(ValidationConstants.User.MAX_EMAIL_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_PhoneNumber))]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(ValidationConstants.User.MAX_PHONE_NUMBER_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string PhoneNumber { get; set; }

        /* Address */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_City))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MaxLength(ValidationConstants.Address.MAX_CITY_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string City { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_StreetName))]
        [MaxLength(ValidationConstants.Address.MAX_STREET_NAME_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string StreetName { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_HouseNumber))]
        [MaxLength(ValidationConstants.Address.MAX_HOUSE_NUMBER_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string HouseNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_FlatNumber))]
        [MaxLength(ValidationConstants.Address.MAX_FLAT_NUMBER_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string FlatNumber { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_PostalCode))]
        [DataType(DataType.PostalCode)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [StringLength(ValidationConstants.Address.POSTAL_CODE_LENGTH, MinimumLength = ValidationConstants.Address.POSTAL_CODE_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.ExactFieldLength))]
        [RegularExpression("[0-9]{2}-[0-9]{3}", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.PostCodeIncorrect))]
        public string PostalCode { get; set; }

        [MaxLength(ValidationConstants.Address.MAX_LATITUDE_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Latitude { get; set; }

        [MaxLength(ValidationConstants.Address.MAX_LONGITUDE_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Longitude { get; set; }

        /* Help offered */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_HasMaskAvailable))]
        public bool HasMaskAvailable { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_HasAdapterAvailable))]
        public bool HasAdapterAvailable { get; set; }

        /* Additional info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.EditProfileViewModel_AdditionalComment))]
        [MaxLength(ValidationConstants.User.MAX_ADDITIONAL_COMMENT_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        [AllowHtml] // TODO: PREVENT SENDING HTML! Instead of sending and encoding
        public string AdditionalComment { get; set; }
        public bool HasMaskCollectionPoint { get; internal set; }
    }
}