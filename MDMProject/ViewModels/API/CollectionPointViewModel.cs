using MDMProject.Models;

namespace MDMProject.ViewModels
{
    public class CollectionPointViewModel
    {
        public int Id { get; set; }
        public UserTypeEnum UserType { get; internal set; }
        public string CompanyName { get; internal set; }
        public string PersonName { get; internal set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AddressViewModel Address { get; set; }
    }
}