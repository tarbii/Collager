using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Collager.Startup))]
namespace Collager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
