using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(testingv.Startup))]
namespace testingv
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
