using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SBP_Reporting.Startup))]
namespace SBP_Reporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
