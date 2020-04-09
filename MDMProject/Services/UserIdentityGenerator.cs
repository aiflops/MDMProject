using MDMProject.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDMProject.Services
{
    public static class UserIdentityGenerator
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUser user, UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}