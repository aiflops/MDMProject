namespace MDMProject.Models
{
    public class ProtectiveEquipment
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int? Amount { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int? EquipmentTypeId { get; set; }

        public virtual EquipmentType EquipmentType { get; set; }
    }
}