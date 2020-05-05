using MDMProject.Models;
using MDMProject.Models.Identity;
using MDMProject.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MDMProject.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            AddRequiredData(context);
            AddFakeData(context);
        }

        private static void AddRequiredData(ApplicationDbContext context)
        {
            context.Roles.Add(new Role { Name = Constants.ADMIN_ROLE_NAME });
            context.Roles.Add(new Role { Name = Constants.COORDINATOR_ROLE_NAME });
            context.Roles.Add(new Role { Name = Constants.COLLECTION_POINT_ROLE_NAME });
            context.SaveChanges();
        }

        public static void AddFakeData(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore(context));

            List<User> data = GetUsersToAdd(context);
            var duplicatedEmails = data.GroupBy(x => x.Email.Trim().ToLower()).Where(x => x.Count() > 1).Select(x => x.Key);
            if (duplicatedEmails.Any())
                throw new Exception("Duplicated emails in data source!");

            foreach (var user in data)
            {
                var userToRegister = new User { UserName = user.Email, Email = user.Email };
                var identityResult = userManager.CreateAsync(userToRegister, "asd123ASD").Result;

                var usersToUpdate = context.Users
                    .Where(x => x.Email == user.Email)
                    .Include(x => x.Coordinator)
                    .ToList();
                var userToUpdate = usersToUpdate.First();

                UpdateUser(userToUpdate, user);
                UpdateUserRoles(userToUpdate, user, userManager, context);
            }

            var addedUsers = context.Users.ToArray();
            foreach (var user in data)
            {
                var usersToUpdate = context.Users
                    .Where(x => x.Email == user.Email)
                    .Include(x => x.Coordinator)
                    .ToList();
                var userToUpdate = usersToUpdate.First();

                userToUpdate.ApprovedBy = user.ApprovedBy;
                userToUpdate.ApprovedDate = user.ApprovedDate;
                userToUpdate.Coordinator = user.Coordinator != null ? addedUsers.First(x => x.Email == user.Coordinator.Email) : null;
                userToUpdate.OtherCoordinatorDetails = user.OtherCoordinatorDetails;
            }

            context.SaveChanges();
        }

        private static void UpdateUser(User userToUpdate, User user)
        {
            userToUpdate.UserType = user.UserType;
            userToUpdate.PhoneNumber = user.PhoneNumber;
            userToUpdate.AdditionalComment = user.AdditionalComment;

            userToUpdate.IndividualName = user.IndividualName;
            userToUpdate.CompanyName = user.CompanyName;
            userToUpdate.ContactPersonName = user.ContactPersonName;

            userToUpdate.CoordinatorId = user.CoordinatorId;
            userToUpdate.OtherCoordinatorDetails = user.OtherCoordinatorDetails;

            userToUpdate.CoordinatedRegion = user.CoordinatedRegion;

            userToUpdate.ProfileFinishedDate = user.ProfileFinishedDate;
            userToUpdate.IndividualName = user.IndividualName;
            userToUpdate.Address = user.Address;
            userToUpdate.EmailConfirmed = true;
        }

        private static void UpdateUserRoles(User userToUpdate, User user, ApplicationUserManager userManager, ApplicationDbContext db)
        {
            var admin = db.Roles.First(x => x.Name == Constants.ADMIN_ROLE_NAME);
            var coordinator = db.Roles.First(x => x.Name == Constants.COORDINATOR_ROLE_NAME);
            var collectionPoint = db.Roles.First(x => x.Name == Constants.COLLECTION_POINT_ROLE_NAME);

            if (user.Roles.Any(x => x.RoleId == admin.Id))
            {
                var addToRoleResult = userManager.AddToRoleAsync(userToUpdate.Id, admin.Name).Result;
            }
            if (user.Roles.Any(x => x.RoleId == coordinator.Id))
            {
                var addToRoleResult = userManager.AddToRoleAsync(userToUpdate.Id, coordinator.Name).Result;
            }
            if (user.Roles.Any(x => x.RoleId == collectionPoint.Id))
            {
                var addToRoleResult = userManager.AddToRoleAsync(userToUpdate.Id, collectionPoint.Name).Result;
            }
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

        private static List<User> GetUsersToAdd(ApplicationDbContext db)
        {
            var admin = db.Roles.First(x => x.Name == Constants.ADMIN_ROLE_NAME);
            var coordinator = db.Roles.First(x => x.Name == Constants.COORDINATOR_ROLE_NAME);
            var collectionPoint = db.Roles.First(x => x.Name == Constants.COLLECTION_POINT_ROLE_NAME);

            List<User> data = new List<User>();

            // Add admin
            var adminUser = new User
            {
                UserType = UserTypeEnum.Individual,
                Email = "admin@admin.com",
                ProfileFinishedDate = DateTime.Now
            }.WithRole(admin);
            data.Add(adminUser);

            // Add coordinators
            var koordynatorWroclaw = new User
            {
                UserType = UserTypeEnum.Company,
                CompanyName = "Firma sp. z o.o.",
                ContactPersonName = "Karol Kontaktowy",
                Email = "coordinator@coordinator.com",
                CoordinatedRegion = "Wrocław Krzyki",
                ProfileFinishedDate = DateTime.Now,
                UserAccountState = UserAccountState.UsingTempPassword
            }.WithRole(coordinator);
            data.Add(koordynatorWroclaw);

            var koordynatorOpole = new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Maciej Maleński",
                Email = "malenski@coordinator.com",
                CoordinatedRegion = "Opole"
            }.WithRole(coordinator);
            data.Add(koordynatorOpole);

            var koordynatorWarszawa = new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Maria Mieciuga",
                Email = "mieciuga@coordinator.com",
                CoordinatedRegion = "Warszawa"
            }.WithRole(coordinator);
            data.Add(koordynatorWarszawa);

            // Add collection points
            // 5x Opole
            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Krzysztof Kowalkiewicz",
                Email = "krzysztof@kowalkiewicz.pl",
                PhoneNumber = "+48 77 12 50 321",
                ApprovedBy = GetApprovingUserInfo(koordynatorOpole),
                ApprovedDate = DateTime.Now,
                Coordinator = koordynatorOpole,
                OtherCoordinatorDetails = null,
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Krakowska",
                    PostalCode = "45-075",
                    HouseNumber = "44",
                    FlatNumber = null,
                    Latitude = "50.6633741",
                    Longitude = "17.925381243554725"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Sebastian Smykała",
                Email = "sebastian@onet.pl",
                PhoneNumber = "77 99 99 999",
                ApprovedBy = GetApprovingUserInfo(koordynatorOpole),
                ApprovedDate = DateTime.Now,
                Coordinator = koordynatorOpole,
                OtherCoordinatorDetails = null,
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Krakowska",
                    PostalCode = "45-018",
                    HouseNumber = "41a",
                    FlatNumber = "9",
                    Latitude = "50.66551335",
                    Longitude = "17.92546319716522"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Donata Dubielewicz",
                Email = "donata@dubielewicz.pl",
                PhoneNumber = "+48-77-99-99-999",
                ApprovedBy = GetApprovingUserInfo(adminUser),
                ApprovedDate = DateTime.Now,
                Coordinator = koordynatorOpole,
                OtherCoordinatorDetails = null,
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Odrowążów",
                    PostalCode = "46-020",
                    HouseNumber = "1",
                    FlatNumber = null,
                    Latitude = "50.6637987",
                    Longitude = "17.915931537338913"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Elżbieta Elokwentnicz",
                Email = "elokwentnicz@onet.pl",
                ApprovedBy = null,
                ApprovedDate = null,
                Coordinator = koordynatorOpole,
                OtherCoordinatorDetails = null,
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Księdza Józefa Londzina",
                    PostalCode = "46-020",
                    HouseNumber = "18",
                    FlatNumber = null,
                    Latitude = "50.6541815",
                    Longitude = "17.951389184873687"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Florian Faktowicz",
                Email = "Florian@gmail.pl",
                ApprovedBy = null,
                ApprovedDate = null,
                Coordinator = null,
                OtherCoordinatorDetails = "Tomasz Wójtowicz,\ntel. 780 530 220",
                Address = new Address
                {
                    City = "Opole",
                    StreetName = "Stefana Okrzei",
                    PostalCode = "45-713",
                    HouseNumber = "7",
                    FlatNumber = "1A",
                    Latitude = "51.108615",
                    Longitude = "17.09772361596369"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            // 2x Wrocław
            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Example Mask",
                Email = "example@email1.com",
                PhoneNumber = "900 789 789",
                ApprovedBy = GetApprovingUserInfo(adminUser),
                ApprovedDate = DateTime.Now,
                Coordinator = koordynatorWroclaw,
                OtherCoordinatorDetails = null,
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
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Company,
                CompanyName = "Example Print",
                ContactPersonName = "Piotrek Print",
                Email = "print@email2.com",
                PhoneNumber = "800 800 800",
                ApprovedBy = null,
                ApprovedDate = null,
                Coordinator = koordynatorWroclaw,
                OtherCoordinatorDetails = null,
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
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            // 3x Warszawa
            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Jakub Jeromy",
                Email = "example@email3.com",
                PhoneNumber = "900 789 789",
                ApprovedBy = GetApprovingUserInfo(koordynatorWarszawa),
                ApprovedDate = DateTime.Now,
                Coordinator = koordynatorWarszawa,
                OtherCoordinatorDetails = null,
                Address = new Address
                {
                    City = "Warszawa",
                    StreetName = "Jaktorowska",
                    HouseNumber = "4",
                    FlatNumber = null,
                    PostalCode = "01-202",
                    Latitude = "52.234579",
                    Longitude = "20.979369966980165"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Individual,
                IndividualName = "Cyprian Czmychała",
                Email = "example@email4.com",
                PhoneNumber = "900 789 789",
                ApprovedBy = null,
                ApprovedDate = null,
                Coordinator = koordynatorWarszawa,
                OtherCoordinatorDetails = null,
                Address = new Address
                {
                    City = "Warszawa",
                    StreetName = "Magistracka",
                    HouseNumber = " 29",
                    FlatNumber = null,
                    PostalCode = "01-413",
                    Latitude = "52.2454704",
                    Longitude = "20.952813428535354"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));

            data.Add(new User
            {
                UserType = UserTypeEnum.Company,
                CompanyName = "Bernard Print S.A.",
                ContactPersonName = "Barnard Stachłowicz",
                Email = "print@email5.com",
                PhoneNumber = "800 800 800",
                ApprovedBy = null,
                ApprovedDate = null,
                Coordinator = null,
                OtherCoordinatorDetails = "Tomasz Mazowiecki, tel. 900 900 900, email: tomasz@mazowiecki.pl",
                Address = new Address
                {
                    City = "Warszawa",
                    StreetName = "Ciołka",
                    HouseNumber = "35",
                    FlatNumber = "74",
                    PostalCode = "01-445",
                    Latitude = "52.6854155",
                    Longitude = "21.1111435"
                },
                ProfileFinishedDate = DateTime.Now
            }.WithRole(collectionPoint));
            return data;
        }

        private static string GetApprovingUserInfo(User approvingUser)
        {
            return $"{approvingUser.FullUserName}({approvingUser.Email})";
        }
    }

    public static class UserRoleHelper
    {
        public static User WithRole(this User user, Role role)
        {
            user.Roles.Add(new UserRole
            {
                RoleId = role.Id
            });

            return user;
        }
    }
}