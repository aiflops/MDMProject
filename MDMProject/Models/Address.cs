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
        [StringLength(256)]
        public string City { get; set; }

        [StringLength(256)]
        public string StreetName { get; set; }

        [Required]
        [StringLength(50)]
        public string HouseNumber { get; set; }

        [StringLength(50)]
        public string FlatNumber { get; set; }

        [Required]
        [StringLength(6)]
        public string PostalCode { get; set; }

        [StringLength(256)]
        public string Latitude { get; set; }

        [StringLength(256)]
        public string Longitude { get; set; }
    }
}