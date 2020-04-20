using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class HelpType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.HelpType.MAX_NAME_LENGTH)]
        public string Name { get; set; }
    }
}