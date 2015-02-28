using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyMagicCollection.mvc.Startup))]
namespace MyMagicCollection.mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
