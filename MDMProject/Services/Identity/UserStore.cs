using MDMProject.Data;
using MDMProject.Models;
using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MDMProject.Services.Identity
{
    public class UserStore : UserStore<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}