using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.Address.MAX_CITY_LENGTH)]
        public string City { get; set; }

        [StringLength(ValidationConstants.Address.MAX_STREET_NAME_LENGTH)]
        public string StreetName { get; set; }

        [Required]
        [StringLength(ValidationConstants.Address.MAX_HOUSE_NUMBER_LENGTH)]
        public string HouseNumber { get; set; }

        [StringLength(ValidationConstants.Address.MAX_FLAT_NUMBER_LENGTH)]
        public string FlatNumber { get; set; }

        [Required]
        [StringLength(ValidationConstants.Address.POSTAL_CODE_LENGTH)]
        public string PostalCode { get; set; }

        [StringLength(ValidationConstants.Address.MAX_LATITUDE_LENGTH)]
        public string Latitude { get; set; }

        [StringLength(ValidationConstants.Address.MAX_LONGITUDE_LENGTH)]
        public string Longitude { get; set; }
    }
}