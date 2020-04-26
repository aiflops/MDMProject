using MDMProject.Models;
using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;

namespace MDMProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public ApplicationDbContext()
            : base("ApplicationDbConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");

            // one-to-zero or one relationship between ApplicationUser and Customer
            // UserId column in Customers table will be foreign key
            modelBuilder.Entity<User>()
                .HasOptional(t => t.Coordinator)
                .WithMany()
                .HasForeignKey(t => t.CoordinatorId);
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<HelpType> HelpTypes { get; set; }
        public DbSet<OfferedHelp> OfferedHelps { get; set; }
        public DbSet<ProtectiveEquipment> ProtectiveEquipments { get; set; }

        public IQueryable<User> GetAllCoordinators()
        {
            var coordinatorRole = Roles.First(x => x.Name == Constants.COORDINATOR_ROLE_NAME);
            var coordinators = Users.Where(x => x.Roles.Any(role => role.RoleId == coordinatorRole.Id));

            return coordinators;
        }

        public IQueryable<User> GetAllCollectionPoints()
        {
            var collectionPointRole = Roles.First(x => x.Name == Constants.COLLECTION_POINT_ROLE_NAME);
            var coordinators = Users.Where(x => x.Roles.Any(role => role.RoleId == collectionPointRole.Id));

            return coordinators;
        }
    }
}