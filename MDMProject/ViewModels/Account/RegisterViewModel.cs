using MDMProject.Validation;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage ="Email jest niepoprawny.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć conajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Hasła nie pasują do siebie.")]
        public string ConfirmPassword { get; set; }

        [EnforceTrue(ErrorMessage = "To pole jest wymagane.")]
        public bool AcceptTerms { get; set; }
    }
}
