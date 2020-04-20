using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class OfferedHelp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(ValidationConstants.OfferedHelp.MAX_NAME_LENGTH)]
        public string Name { get; set; }

        [StringLength(ValidationConstants.OfferedHelp.MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey(nameof(HelpType))]
        public int? HelpTypeId { get; set; }

        public virtual HelpType HelpType { get; set; }
    }
}