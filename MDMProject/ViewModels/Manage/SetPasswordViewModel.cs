using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class SetPasswordViewModel
    {
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_NewPassword))]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_PASSWORD_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_PASSWORD_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string NewPassword { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_ConfirmNewPassword))]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.PasswordDoesntMatch))]
        public string ConfirmPassword { get; set; }
    }
}