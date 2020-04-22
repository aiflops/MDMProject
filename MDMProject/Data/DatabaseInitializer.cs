using MDMProject.Models;
using MDMProject.Models.Identity;
using MDMProject.Services.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebGrease.Css.Extensions;

namespace MDMProject.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            AddRequiredData(context);
            //AddFakeData(context);
        }

        private static void AddRequiredData(ApplicationDbContext context)
        {
            context.HelpTypes.Add(new HelpType { Name = Constants.ADAPTER_NAME });
            context.HelpTypes.Add(new HelpType { Name = Constants.MASK_COLLECTION_POINT_NAME });
            context.EquipmentTypes.Add(new EquipmentType { Name = Constants.MASK_NAME });
            context.Roles.Add(new Role { Name = "Admin" });
            context.SaveChanges();
        }

        public static void AddFakeData(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore(context));

            var adapterHelpType = context.HelpTypes.First(x => x.Name == Constants.ADAPTER_NAME);
            var maskCollectionPointType = context.HelpTypes.First(x => x.Name == Constants.MASK_COLLECTION_POINT_NAME);
            var maskEquipmentType = context.EquipmentTypes.First(x => x.Name == Constants.MASK_NAME);

            List<User> data = GetUsersToAdd(adapterHelpType, maskEquipmentType, maskCollectionPointType);
            var duplicatedEmails = data.GroupBy(x => x.Email.Trim().ToLower()).Where(x => x.Count() > 1).Select(x => x.Key);
            if (duplicatedEmails.Any())
                throw new System.Exception("Duplicated emails in data source!");

            foreach (var user in data)
            {
                var userToRegister = new User { UserName = user.Email, Email = user.Email };
                var identityResult = userManager.CreateAsync(userToRegister, "asd123ASD").Result;

                var usersToUpdate = context.Users
                    .Where(x => x.Email == user.Email)
                    .Include(x => x.OfferedEquipment)
                    .Include(x => x.OfferedHelp)
                    .ToList();
                var userToUpdate = usersToUpdate.First();
                UpdateUser(userToUpdate, user);
            }

            context.SaveChanges();
        }

        private static void UpdateUser(User userToUpdate, User user)
        {
            userToUpdate.IsProfileFinished = user.IsProfileFinished;
            userToUpdate.Name = user.Name;
            userToUpdate.AdditionalComment = user.AdditionalComment;
            userToUpdate.PhoneNumber = user.PhoneNumber;
            userToUpdate.Address = user.Address;
            userToUpdate.EmailConfirmed = true;

            user.OfferedEquipment?.ForEach(x => userToUpdate.OfferedEquipment.Add(x));
            user.OfferedHelp?.ForEach(x => userToUpdate.OfferedHelp.Add(x));
        }

        public static void DropAllUsers(ApplicationDbContext context)
        {
            context.OfferedHelps.RemoveRange(context.OfferedHelps);
            context.ProtectiveEquipments.RemoveRange(context.ProtectiveEquipments);
            var usersToRemove = context.Users.ToArray();
            foreach (var user in usersToRemove)
            {
                context.Users.Remove(user);
            }

            context.SaveChanges();
        }

        private static List<User> GetUsersToAdd(HelpType adapterHelpType, EquipmentType maskEquipmentType, HelpType collectionPointHelpType)
        {
            List<User> data = new List<User>();
            data.Add(new User
            {
                Email = "admin@admin.com"
            });

            data.Add(new User
            {
                Name = "Krzysztof Kowalkiewicz",
                Email = "krzysztof@kowalkiewicz.pl",
                PhoneNumber = "+48 77 12 50 321",
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Krakowska",
                    PostalCode = "45-075",
                    HouseNumber = "44",
                    FlatNumber = null,
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = HasMask(maskEquipmentType),
                OfferedHelp = null,
                AdditionalComment = "Tylko po 17:00!",
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Sebastian Smykała",
                Email = "sebastian@onet.pl",
                PhoneNumber = "77 99 99 999",
                Address = new Address
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
                OfferedHelp = HasAdapter(adapterHelpType),
                AdditionalComment = null,
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Donata Dubielewicz",
                Email = "donata@dubielewicz.pl",
                PhoneNumber = "+48-77-99-99-999",
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Odrowążów",
                    PostalCode = "46-020",
                    HouseNumber = "1",
                    FlatNumber = null,
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = null,
                OfferedHelp = HasCollectionPoint(collectionPointHelpType),
                AdditionalComment = null,
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Elżbieta Elokwentnicz",
                Email = "elokwentnicz@onet.pl",
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Księdza Józefa Londzina",
                    PostalCode = "46-020",
                    HouseNumber = "18",
                    FlatNumber = null,
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = HasMask(maskEquipmentType),
                OfferedHelp = null,
                AdditionalComment = null,
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Florian Faktowicz",
                Email = "Florian@gmail.pl",
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Stefana Okrzei",
                    PostalCode = "45-713",
                    HouseNumber = "7",
                    FlatNumber = "1A",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = HasMask(maskEquipmentType),
                OfferedHelp = HasAdapter(adapterHelpType),
                IsProfileFinished = true
            });

            // 2x Wrocław
            data.Add(new User
            {
                Name = "Example Mask",
                Email = "example@email1.com",
                PhoneNumber = "900 789 789",
                Address = new Address
                {
                    City = "Wrocław",
                    StreetName = "Klimasa",
                    HouseNumber = "37D",
                    FlatNumber = "1",
                    PostalCode = "50-515",
                    Latitude = "51.07946",
                    Longitude = "17.06339"
                },
                OfferedEquipment = HasMask(maskEquipmentType),
                OfferedHelp = null,
                AdditionalComment = "Proszę o kontakt przed 18:00.",
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Example Print",
                Email = "print@email2.com",
                PhoneNumber = "800 800 800",
                Address = new Address
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
                OfferedHelp = HasAdapter(adapterHelpType),
                AdditionalComment = "Kontakt do 15:00",
                IsProfileFinished = true
            });

            // 3x Warszawa
            data.Add(new User
            {
                Name = "Jakub Jeromy",
                Email = "example@email3.com",
                PhoneNumber = "900 789 789",
                Address = new Address
                {
                    City = "Warszawa",
                    StreetName = "Jaktorowska",
                    HouseNumber = "4",
                    FlatNumber = null,
                    PostalCode = "01-202",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = HasMask(maskEquipmentType),
                OfferedHelp = null,
                AdditionalComment = null,
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Cyprian Czmychała",
                Email = "example@email4.com",
                PhoneNumber = "900 789 789",
                Address = new Address
                {
                    City = "Warszawa",
                    StreetName = "Magistracka",
                    HouseNumber = " 29",
                    FlatNumber = null,
                    PostalCode = "01-413",
                    Latitude = null,
                    Longitude = null
                },
                OfferedEquipment = HasMask(maskEquipmentType),
                OfferedHelp = null,
                AdditionalComment = null,
                IsProfileFinished = true
            });

            data.Add(new User
            {
                Name = "Bernard Print S.A.",
                Email = "print@email5.com",
                PhoneNumber = "800 800 800",
                Address = new Address
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
                OfferedHelp = HasAdapter(adapterHelpType),
                AdditionalComment = null,
                IsProfileFinished = true
            });
            return data;
        }

        private static ICollection<ProtectiveEquipment> HasMask(EquipmentType maskEquipmentType)
        {
            var result = new List<ProtectiveEquipment>
            {
                new ProtectiveEquipment
                {
                    EquipmentType = maskEquipmentType
                }
            };
            return result;
        }

        private static ICollection<OfferedHelp> HasAdapter(HelpType adapterHelpType)
        {
            var result = new List<OfferedHelp>
            {
                new OfferedHelp
                {
                    HelpType = adapterHelpType
                }
            };
            return result;
        }

        private static ICollection<OfferedHelp> HasCollectionPoint(HelpType collectionPointHelpType)
        {
            var result = new List<OfferedHelp>
            {
                new OfferedHelp
                {
                    HelpType = collectionPointHelpType
                }
            };
            return result;
        }
    }
}