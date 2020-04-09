using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class EditProfileViewModel
    {
        /* Basic info */
        [Display(Name = "Imię i nazwisko")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Imię i nazwisko jest wymagane.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Pole może mieć maksymalnie 256 znaków, minimalnie 3.")]
        public string UserName { get; set; }

        [Display(Name = "E-mail")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "E-mail jest wymagany.")]
        [StringLength(256, ErrorMessage = "Pole może mieć maksymalnie 256 znaków.")]
        [EmailAddress(ErrorMessage = "E-mail jest niepoprawny.")]
        public string Email { get; set; }

        [Display(Name = "Telefon")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(50, ErrorMessage = "Pole może mieć maksymalnie 50 znaków.")]
        public string PhoneNumber { get; set; }

        /* Address */
        [Display(Name = "Miasto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Miasto jest wymagane.")]
        [StringLength(256, ErrorMessage = "Pole może mieć maksymalnie 256 znaków.")]
        public string City { get; set; }

        [Display(Name = "Ulica")]
        [StringLength(256, ErrorMessage = "Pole może mieć maksymalnie 256 znaków.")]
        public string StreetName { get; set; }

        [Display(Name = "Nr domu")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nr domu jest wymagany.")]
        [StringLength(50, ErrorMessage = "Pole może mieć maksymalnie 50 znaków.")]
        public string HouseNumber { get; set; }

        [Display(Name = "Nr mieszkania")]
        [StringLength(50, ErrorMessage = "Pole może mieć maksymalnie 50 znaków.")]
        public string FlatNumber { get; set; }

        [Display(Name = "Kod pocztowy")]
        [DataType(DataType.PostalCode)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kod pocztowy jest wymagany.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pole musi mieć 6 znaków (format 00-000).")]
        [RegularExpression("[0-9]{2}-{[0-9]{3}", ErrorMessage = "Niepoprawny format kodu pocztowego (format 00-000).")]
        public string PostalCode { get; set; }

        [StringLength(256, ErrorMessage = "Pole może mieć maksymalnie 256 znaków.")]
        public string Latitude { get; set; }

        [StringLength(256, ErrorMessage = "Pole może mieć maksymalnie 256 znaków.")]
        public string Longitude { get; set; }

        /* Help offered */
        [Display(Name = "Maska")]
        public bool HasMaskAvailable { get; set; }
        [Display(Name = "Adapter")]
        public bool Has3dPrinter { get; set; }

        /* Additional info */
        [Display(Name = "Dodatkowe informacje")]
        [StringLength(256, ErrorMessage = "Pole może mieć maksymalnie 256 znaków.")]
        public string AdditionalComment { get; set; }
    }
}