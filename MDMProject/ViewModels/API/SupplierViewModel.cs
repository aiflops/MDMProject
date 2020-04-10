using System.Collections.Generic;

namespace MDMProject.ViewModels
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AddressViewModel Address { get; set; }
        public IEnumerable<ProtectiveEquipmentViewModel> OfferedEquipment { get; set; }
        public IEnumerable<OfferedHelpViewModel> OfferedHelp { get; set; }
        public string AdditionalComment { get; set; }
    }
}