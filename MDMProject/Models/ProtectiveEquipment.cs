using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class ProtectiveEquipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? Amount { get; set; }

        [StringLength(256)]
        public string Comment { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey(nameof(EquipmentType))]
        public int? EquipmentTypeId { get; set; }

        public virtual EquipmentType EquipmentType { get; set; }
    }
}