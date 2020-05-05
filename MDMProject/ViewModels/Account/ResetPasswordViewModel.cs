using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_Email))]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.EmailIsIncorrect))]
        [MaxLength(ValidationConstants.User.MAX_EMAIL_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Email { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.User_Password))]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_PASSWORD_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_PASSWORD_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Password { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_ConfirmPassword))]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.PasswordDoesntMatch))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
