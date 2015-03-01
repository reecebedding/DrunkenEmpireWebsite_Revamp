using Microsoft.Owin;
using Ninject;
using Owin;
using System.Reflection;

[assembly: OwinStartupAttribute(typeof(HoppsWebPlatform_Revamp.Startup))]
namespace HoppsWebPlatform_Revamp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}