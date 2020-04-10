using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
