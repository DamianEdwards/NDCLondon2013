using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRAndNancy.Startup))]

namespace SignalRAndNancy
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();

            app.MapSignalR();

            app.UseNancy();
        }
    }
}
