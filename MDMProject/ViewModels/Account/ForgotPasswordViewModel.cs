using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.Common_Email))]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.EmailIsIncorrect))]
        [MaxLength(ValidationConstants.User.MAX_EMAIL_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string Email { get; set; }
    }
}
