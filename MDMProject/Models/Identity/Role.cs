using Microsoft.AspNet.Identity.EntityFramework;

namespace MDMProject.Models.Identity
{
    public class Role : IdentityRole<int, UserRole>
    {
        public Role() { }
        public Role(string name) { Name = name; }
    }
}