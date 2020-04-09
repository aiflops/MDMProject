using MDMProject.Models;
using MDMProject.Models.Identity;
using System.Data.Entity;

namespace MDMProject.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.HelpTypes.Add(new HelpType { Name = Constants.ADAPTER_NAME });
            context.EquipmentTypes.Add(new EquipmentType { Name = Constants.MASK_NAME });
            context.Roles.Add(new Role { Name = "Admin" });
            context.SaveChanges();
        }
    }
}