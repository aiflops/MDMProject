using MDMProject.Models;
using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

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
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<HelpType> HelpTypes { get; set; }
        public DbSet<OfferedHelp> OfferedHelps { get; set; }
        public DbSet<ProtectiveEquipment> ProtectiveEquipments { get; set; }
    }
}