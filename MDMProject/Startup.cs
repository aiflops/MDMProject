using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MDMProject.Startup))]
namespace MDMProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
