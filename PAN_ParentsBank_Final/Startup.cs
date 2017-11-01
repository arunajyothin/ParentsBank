using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PAN_ParentsBank_Final.Startup))]
namespace PAN_ParentsBank_Final
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
