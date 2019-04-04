using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Guitar.Startup))]
namespace Guitar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
