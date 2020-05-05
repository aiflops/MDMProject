using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace MDMProject.ViewModels
{
    public class CreateUserViewModel
    {
        public int? Id { get; set; }

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

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Coordinator_CoordinatedRegion))]
        [RequiredIf(nameof(IsCoordinator), Operator.EqualTo, true, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_COORDINATED_REGION_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_COORDINATED_REGION_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string CoordinatedRegion { get; set; }

        /* Additional info */
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_AdditionalComment))]
        [MaxLength(ValidationConstants.User.MAX_ADDITIONAL_COMMENT_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        //[AllowHtml] // TODO: PREVENT SENDING HTML! Instead of sending and encoding
        public string AdditionalComment { get; set; }

        public bool IsCoordinator { get; set; }

        public bool IsAdmin { get; set; }
    }
}