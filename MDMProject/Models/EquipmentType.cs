using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class EquipmentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.EquipmentType.MAX_NAME_LENGTH)]
        public string Name { get; set; }
    }
}