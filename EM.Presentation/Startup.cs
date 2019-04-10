using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EM.Presentation.Startup))]
namespace EM.Presentation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
