using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class ProtectiveEquipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(ValidationConstants.ProtectiveEquipment.MAX_NAME_LENGTH)]
        public string Name { get; set; }

        public int? Amount { get; set; }

        [StringLength(ValidationConstants.ProtectiveEquipment.MAX_COMMENT_LENGTH)]
        public string Comment { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey(nameof(EquipmentType))]
        public int? EquipmentTypeId { get; set; }

        public virtual EquipmentType EquipmentType { get; set; }
    }
}