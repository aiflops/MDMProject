using Microsoft.AspNet.Identity.EntityFramework;

namespace MDMProject.Models.Identity
{
    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) { Name = name; }
    }
}