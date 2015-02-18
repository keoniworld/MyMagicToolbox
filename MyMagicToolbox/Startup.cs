using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyMagicToolbox.Startup))]
namespace MyMagicToolbox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
