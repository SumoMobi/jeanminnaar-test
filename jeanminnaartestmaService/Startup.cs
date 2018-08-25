using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(jeanminnaartestmaService.Startup))]

namespace jeanminnaartestmaService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}