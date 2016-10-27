using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Utube.Startup))]
namespace Utube
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
