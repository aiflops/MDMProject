using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MDMProject.Models
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
    }
}