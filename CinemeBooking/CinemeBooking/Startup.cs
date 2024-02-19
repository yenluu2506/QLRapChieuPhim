using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CinemeBooking.Startup))]
namespace CinemeBooking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
