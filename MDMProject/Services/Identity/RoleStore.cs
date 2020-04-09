using MDMProject.Data;
using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MDMProject.Services.Identity
{
    public class RoleStore : RoleStore<Role, int, UserRole>
    {
        public RoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}