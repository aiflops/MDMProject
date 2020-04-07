using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MDMProject.WebAPI_Controllers
{
    public class SuppliersController : ApiController
    {
        // GET: api/Supplier
        public IEnumerable<SupplierViewModel> Get()
        {
            var data = GetMockedData();
            return data;
        }

        // GET: api/Supplier/5
        public SupplierViewModel Get(string id)
        {
            var data = GetMockedData().FirstOrDefault(x => x.Id == id);
            return data;
        }

        // POST: api/Supplier
        public void Post([FromBody]SupplierViewModel value)
        {
        }

        // PUT: api/Supplier/5
        public void Put(int id, [FromBody]SupplierViewModel value)
        {
        }

        // DELETE: api/Supplier/5
        public void Delete(string id)
        {
        }

        private IEnumerable<SupplierViewModel> GetMockedData()
        {
            var data = new List<SupplierViewModel>();
            // 5x Opole
            data.Add(new SupplierViewModel
            {
                Id = "13",
                Name = "Krzysztof Kowalkiewicz",
                Email = "krzysztof@kowalkiewicz.pl",
                PhoneNumber = "+48 77 12 50 321",
                Address = new AddressViewModel
                {
                    City = "Opole",
                    StreetName = "Krakowska",
                    PostalCode = "45-075",
                    HouseNumber = "44",
                    FlatNumber = null,
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = GetMaski(1),
                OfferedHelp = null,
                AdditionalComment = "Tylko po 17:00!"
            });

            data.Add(new SupplierViewModel
            {
                Id = "17",
                Name = "Sebastian Smykała",
                Email = "sebastian@onet.pl",
                PhoneNumber = "77 99 99 999",
                Address = new AddressViewModel
                {
                    City = "Opole",
                    StreetName = "Krakowska",
                    PostalCode = "45-018",
                    HouseNumber = "41a",
                    FlatNumber = "9",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = null,
                OfferedHelp = GetDruk3d(),
                AdditionalComment = null
            });

            data.Add(new SupplierViewModel
            {
                Id = "19",
                Name = "Donata Dubielewicz",
                Email = null,
                PhoneNumber = "+48-77-99-99-999",
                Address = new AddressViewModel
                {
                    City = "Opole",
                    StreetName = "Odrowążów",
                    PostalCode = "46-020",
                    HouseNumber = "1",
                    FlatNumber = null,
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = GetMaski(1000),
                OfferedHelp = null,
                AdditionalComment = null
            });

            data.Add(new SupplierViewModel
            {
                Id = "22",
                Name = "Elżbieta Elokwentnicz",
                Email = "elokwentnicz@onet.pl",
                Address = new AddressViewModel
                {
                    City = "Opole",
                    StreetName = "Księdza Józefa Londzina",
                    PostalCode = "46-020",
                    HouseNumber = "18",
                    FlatNumber = null,
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = GetMaski(1),
                OfferedHelp = null,
                AdditionalComment = null
            });

            data.Add(new SupplierViewModel
            {
                Id = "24",
                Name = "Florian Faktowicz",
                Email = "Florian@gmail.pl",
                Address = new AddressViewModel
                {
                    City = "Opole",
                    StreetName = "Stefana Okrzei",
                    PostalCode = "45-713",
                    HouseNumber = "7",
                    FlatNumber = "1A",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = GetMaski(2, "Trochę porysowane"),
                OfferedHelp = GetDruk3d("Drukujemy adaptery do masek, osłony"),
            });

            // 2x Wrocław
            data.Add(new SupplierViewModel
            {
                Id = "27",
                Name = "Example Mask",
                Email = "example@email.com",
                PhoneNumber = "900 789 789",
                Address = new AddressViewModel
                {
                    City = "Wrocław",
                    StreetName = "Klimasa",
                    HouseNumber = "37D",
                    FlatNumber = "1",
                    PostalCode = "50-515",
                    Latitude = "51.07946",
                    Longitude = "17.06339"
                },
                OfferedEquipment = GetMaski(2, "Trochę porysowane"),
                OfferedHelp = null,
                AdditionalComment = "Proszę o kontakt przed 18:00."
            });

            data.Add(new SupplierViewModel
            {
                Id = "30",
                Name = "Example Print",
                Email = "print@email.com",
                PhoneNumber = "800 800 800",
                Address = new AddressViewModel
                {
                    City = "Wrocław",
                    StreetName = "Klimasa",
                    HouseNumber = "8",
                    FlatNumber = null,
                    PostalCode = "50-512",
                    Latitude = "51.08002",
                    Longitude = "17.05894"
                },
                OfferedEquipment = null,
                OfferedHelp = GetDruk3d("Drukujemy adaptery do masek, osłony"),
                AdditionalComment = "Kontakt do 15:00"
            });

            // 3x Warszawa
            data.Add(new SupplierViewModel
            {
                Id = "33",
                Name = "Jakub Jeromy",
                Email = "example@email.com",
                PhoneNumber = "900 789 789",
                Address = new AddressViewModel
                {
                    City = "Warszawa",
                    StreetName = "Jaktorowska",
                    HouseNumber = "4",
                    FlatNumber = null,
                    PostalCode = "01-202",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = GetMaski(1),
                OfferedHelp = null,
                AdditionalComment = null
            });

            data.Add(new SupplierViewModel
            {
                Id = "35",
                Name = "Cyprian Czmychała",
                Email = "example@email.com",
                PhoneNumber = "900 789 789",
                Address = new AddressViewModel
                {
                    City = "Warszawa",
                    StreetName = "Magistracka",
                    HouseNumber = " 29",
                    FlatNumber = null,
                    PostalCode = "01-413",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = GetMaski(8),
                OfferedHelp = null,
                AdditionalComment = null
            });

            data.Add(new SupplierViewModel
            {
                Id = "39",
                Name = "Bernard Print S.A.",
                Email = "print@email.com",
                PhoneNumber = "800 800 800",
                Address = new AddressViewModel
                {
                    City = "Warszawa",
                    StreetName = "Ciołka",
                    HouseNumber = "35",
                    FlatNumber = "74",
                    PostalCode = "01-445",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = null,
                OfferedHelp = GetDruk3d(),
                AdditionalComment = null
            });
            return data;
        }

        private ProtectiveEquipmentViewModel[] GetMaski(int amount, string comment = null)
        {
            return new[]
            {
                new ProtectiveEquipmentViewModel
                {
                    Name = "Maska", Amount = amount, Comment = comment
                }
            };
        }

        private OfferedHelpViewModel[] GetDruk3d(string description = null)
        {
            return new[]
            {
                new OfferedHelpViewModel
                {
                    Name = "Druk 3d",
                    Description = description
                }
            };
        }
    }
}
